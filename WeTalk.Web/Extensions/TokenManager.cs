using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Extensions;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.ViewModel;

namespace WeTalk.Web.Extensions
{
	public class TokenManager
    {
        /// <summary>
        /// 缓存组件
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        private readonly IPubLogService _pubLogService;
        private readonly SqlSugarScope _context;

        /// <summary>
        /// HTTP上下文
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        private readonly string _loginPrefix;

        //全局参数
        public string ScriptStr { get; set; }

        public TokenManager(IHttpContextAccessor accessor,IMemoryCache memoryCache, SqlSugarScope context, IPubLogService pubLogService
            )
        {
            _context = context;
            _accessor = accessor;
            _memoryCache = memoryCache;
            _pubLogService = pubLogService;
            _loginPrefix = Appsettings.app("Web:LoginPrefix");
        }

        #region Session 操作
        /// <summary>
        /// 创建 Token至Redis,二个KEY一组，AdminMasterid=>token,token=>用户详细信息
        /// </summary>
        public string CreateSession(AdminMaster userInfo, int hours,string lang = "cn")
        {
            var token = Guid.NewGuid().ToString().ToUpper();
            userInfo.Token = token;
            //判断用户是否只允许登录一次
            if (userInfo.Onlyone)
			{
				RemoveAllSession(userInfo.AdminMasterid);
			}

			var expireTime = DateTime.Now.AddHours(hours);
            var timeSpan = new TimeSpan(hours, 0, 0);

            var list_result = GetFunctionids(userInfo.AdminMasterid);
			#region "写入redis"
			//将 Session 添加用户 Session 列表
			RedisServer.Token.HSet(userInfo.AdminMasterid.ToString(), token, expireTime);
            RedisServer.Token.Expire(userInfo.AdminMasterid.ToString(), timeSpan);
            //设置 Token 信息
            var adminMasterVM = new AdminMasterVM()
            {
                AdminMasterid = userInfo.AdminMasterid,
                Username = userInfo.Username,
                Nums = userInfo.Nums,
                Ip = userInfo.Ip,
                Lasttime = userInfo.Lasttime,
                AdminRoleids = userInfo.AdminRoleids,
                AdminMenuids = userInfo.AdminMenuids,
                Name = userInfo.Name,
                Email = userInfo.Email,
                Dtime = userInfo.Dtime,
                Mobile = userInfo.Mobile,
                Photo = userInfo.Photo,
                SmsCode = userInfo.SmsCode,
                SmsTime = userInfo.SmsTime,
                Loginnum = userInfo.Loginnum,
                AdminMenuFunctionids = userInfo.AdminMenuFunctionids,
                Status = userInfo.Status,
                Token = token,
                Isadmin = Appsettings.app("Web:Admins").ToLower().Split(',').Contains(userInfo.Username.ToLower()),
                Onlyone = userInfo.Onlyone,
                list_Functionid = list_result.list_functionid,
                list_Menuid = list_result.list_menuid,
                KeepHours = hours,
                Lang = lang
            };

            RedisServer.Token.HSet(token, "AdminInfo", adminMasterVM);
            RedisServer.Token.Expire(token, timeSpan);
            #endregion

            #region "写入cookie与session"
            //存SESSION 
            _accessor.HttpContext.Session.SetString(_loginPrefix + "AdminName", userInfo.Username);
            _accessor.HttpContext.Session.SetString(_loginPrefix + "AdminID", userInfo.AdminMasterid.ToString());
            _accessor.HttpContext.Session.SetString(_loginPrefix + "FunctionIDs", string.Join(',', list_result.list_functionid));
            _accessor.HttpContext.Session.SetString(_loginPrefix + "MenuIDs", string.Join(',', list_result.list_menuid));
            CookieHelper.SetCookie(_accessor.HttpContext,_loginPrefix + "Token", token, 1440);
            
            #endregion

            //添加在线记录表
            var model_Online = new AdminOnline()
            {
                Token = token,
                AdminMasterid = userInfo.AdminMasterid,
                Source = "web",
                Ip = IpHelper.GetCurrentIp(_accessor.HttpContext),
                Logintime = DateTime.Now,
                Lasttime = DateTime.Now
            };
            _context.Insertable(model_Online).ExecuteCommand();
            _context.Updateable<AdminMaster>()
                .SetColumns(u => u.Lasttime == DateTime.Now)
                .SetColumns(u => u.Loginnum == u.Loginnum + 1)
                .SetColumns(u => u.LastLang == lang)
                .Where(u => u.AdminMasterid == userInfo.AdminMasterid)
                .ExecuteCommand();

            //写入业务日志
            _pubLogService.AddLog(userInfo.Username, "用户登录后台");
            return token;
        }

        /// <summary>
        /// 更新Token的Redis过期时间
        /// </summary>
        /// <param name="userToken">用户Token</param>
        public void UpdateToken(string userToken)
        {
            if (string.IsNullOrEmpty(userToken)) return;
            //利用内存记录时间间隔只有超出2分钟，才会刷新redis和cookie;避免频繁刷
            DateTime lastUpdateTime = _memoryCache.Get<DateTime>(userToken);

            if (Convert.ToDateTime(lastUpdateTime).AddMinutes(2) < DateTime.Now)
            {
                // 记录本次更新时间
                _memoryCache.Set(userToken, DateTime.Now);

                if (!string.IsNullOrEmpty(userToken))
                {
                    //更新在线用户记录最后操作时间
                    _context.Updateable<AdminOnline>().SetColumns(u => u.Lasttime == DateTime.Now).Where(u => u.Token == userToken).ExecuteCommand();

                    //根据 token 取出 AdminInfo
                    var adminMasterVM = GetTokenItem<AdminMasterVM>(userToken, "AdminInfo");
                    if (adminMasterVM == null) return;
                    var expireTime = DateTime.Now.AddHours(adminMasterVM.KeepHours);
                    var timeSpan = new TimeSpan(adminMasterVM.KeepHours, 0, 0);

                    //更新 Session 列表中的 Session 过期时间
                    RedisServer.Token.HSet(adminMasterVM.AdminMasterid.ToString(), userToken, expireTime);
                    //更新 Session 列表过期时间
                    RedisServer.Token.Expire(adminMasterVM.AdminMasterid.ToString(), timeSpan);
                    //更新 Session 过期时间
                    RedisServer.Token.Expire(userToken, timeSpan);

                    //设置语言
                    _accessor.HttpContext.Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName, //默认 Cookie 名称是：.AspNetCore.Culture
                        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(adminMasterVM.Lang))
                        );

                }
                CookieHelper.UpdateCookie(_accessor.HttpContext,_loginPrefix + "Token", 1440);
            }
        }

        /// <summary>
        /// 刷新Redis中的用户信息(从Db中取出)
        /// </summary>
        /// <param name="AdminMasterid"></param>
        /// <returns></returns>
        public void RefreshSession(long AdminMasterid)
        {
            if (!RedisServer.Token.Exists(AdminMasterid.ToString()))
            {
                return;
            }

            //取出 Session 列表所有 Key
            var keys = RedisServer.Token.HKeys(AdminMasterid.ToString());

            if (keys.Length <= 0)
            {
                return;
            }

            var userInfo = _context.Queryable<AdminMaster>().InSingle(AdminMasterid);

            foreach (var key in keys)
            {
                if (RedisServer.Token.Exists(key))
                {
                    //根据 Session 取出 AdminInfo
                    var redisAdminInfo = GetTokenItem<AdminMasterVM>(key, "AdminInfo");
                    if (redisAdminInfo == null) return;

                    //设置 Session 信息
                    var AdminMasterVM = new AdminMasterVM()
                    {
                        AdminMasterid = userInfo.AdminMasterid,
                        Username = userInfo.Username,
                        Nums = userInfo.Nums,
                        Ip = userInfo.Ip,
                        Lasttime = userInfo.Lasttime,
                        AdminRoleids = userInfo.AdminRoleids,
                        AdminMenuids = userInfo.AdminMenuids,
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                        Dtime = userInfo.Dtime,
                        Mobile = userInfo.Mobile,
                        Photo = userInfo.Photo,
                        SmsCode = userInfo.SmsCode,
                        SmsTime = userInfo.SmsTime,
                        Loginnum = userInfo.Loginnum,
                        AdminMenuFunctionids = userInfo.AdminMenuFunctionids,
                        Status = userInfo.Status,
                        Token = userInfo.Token,
                        Isadmin = Appsettings.app("Web:Admins").ToLower().Split(',').Contains(userInfo.Username.ToLower()),
                        Onlyone = userInfo.Onlyone,
                        list_Functionid = redisAdminInfo.list_Functionid,
                        list_Menuid = redisAdminInfo.list_Menuid,
                        KeepHours = redisAdminInfo.KeepHours,
                        Lang = userInfo.LastLang
                    };

                    RedisServer.Token.HSet(key, "AdminInfo", AdminMasterVM);
                }
                else
                {
                    RedisServer.Token.HDel(AdminMasterid.ToString(), key);
                }
            }
        }


        /// <summary>
        /// 清除Redis中指定 Token
        /// </summary>
        public void RemoveSession(string userToken)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                //根据 Token 删除在线用户记录
                _context.Deleteable<AdminOnline>().Where(u => u.Token == userToken).ExecuteCommand();

                //清除Session
                _accessor.HttpContext.Session.Clear();

                //清除Cookie
                CookieHelper.DeleteCookie(_accessor.HttpContext, userToken);

                #region "清除redis"
                if (RedisServer.Token.Exists(userToken))
                {
                    //根据 Session 取出 AdminInfo
                    var AdminInfo = GetTokenItem<AdminMasterVM>(userToken, "AdminInfo");
                    if (AdminInfo == null) return;
                    //删除用户 Session 列表中的 Session
                    RedisServer.Token.HDel(AdminInfo.AdminMasterid.ToString(), userToken);

                    //删除 Session 
                    RedisServer.Token.Del(userToken);
                }
                #endregion
            }
        }

        /// <summary>
        /// 清除Redis中指定 AdminMasterid
        /// </summary>
        /// <param name="AdminMasterid"></param>
        public void RemoveAllSession(long AdminMasterid)
        {
            #region "清除redis"
            if (RedisServer.Token.Exists(AdminMasterid.ToString()))
            {
                //取出 Session 列表所有 Key
                var keys = RedisServer.Token.HKeys(AdminMasterid.ToString());

                foreach (var key in keys)
                {
                    //删除 Session 
                    RedisServer.Token.Del(key);

                    //删除用户 Session 列表中的 Session
                    RedisServer.Token.HDel(AdminMasterid.ToString(), key);
                }
            }
            #endregion

            //清除Session
            _accessor.HttpContext.Session.Clear();

            //清除Cookie
            CookieHelper.DeleteCookie(_accessor.HttpContext, GetSysToken);

            //删除在线记录
            _context.Deleteable<AdminOnline>().Where(u => u.AdminMasterid == AdminMasterid).ExecuteCommand();
        }

        #endregion

        #region Token 获取信息
        /// <summary>
        /// 从客户端 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetSysToken => GetCookie(_loginPrefix + "Token");

        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        /// <returns></returns>
        public AdminMasterVM GetAdminInfo() => GetTokenItem<AdminMasterVM>("AdminInfo");

        /// <summary>
        /// 判断用户是否登录
        /// </summary>
        /// <returns></returns>
        public long IsAuthenticated()
        {
            long _AdminMasterid = 0;
            if (_accessor.HttpContext.Session != null && !string.IsNullOrEmpty(_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")))
            {
                _AdminMasterid =  long.Parse(_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID").Trim());
            }
            else if (_accessor.HttpContext.Request.Cookies != null)
            {
                if (!RedisServer.Token.Exists(GetSysToken))
                {
                    //根据 Session 删除在线用户记录
                    _context.Deleteable<AdminOnline>().Where(u => u.Token == GetSysToken).ExecuteCommand();
                }
                else
                {
                    UpdateToken(GetSysToken);
                    AdminMasterVM adminMasterVM = GetTokenItem<AdminMasterVM>(GetSysToken, "AdminInfo");
                    if (adminMasterVM == null) return 0;
                    _AdminMasterid = adminMasterVM.AdminMasterid;
                    _accessor.HttpContext.Session.SetString(_loginPrefix + "AdminName", adminMasterVM.Username);
                    _accessor.HttpContext.Session.SetString(_loginPrefix + "AdminID", adminMasterVM.AdminMasterid.ToString());
                    _accessor.HttpContext.Session.SetString(_loginPrefix + "FunctionIDs", string.Join(',', adminMasterVM.list_Functionid));
                    _accessor.HttpContext.Session.SetString(_loginPrefix + "MenuIDs", string.Join(',', adminMasterVM.list_Menuid));
                }
            }
            return _AdminMasterid;
        }

        /// <summary>
        /// 判断用户权限
        /// 这里后继优化，没必要每次都查Db
        /// </summary>
        /// <returns></returns>
        public (bool isok, List<string> list_controls) IsAuthenticated(string Power, RouteData routeData)
        {
            //string method="", parameter="";//暂时没加这两项的权限判断
            string area = routeData.Values["area"].ToString();
            string controller = routeData.Values["controller"].ToString(); 
            string action = routeData.Values["action"].ToString(); 

            long AdminMasterid = IsAuthenticated();
            if (AdminMasterid > 0)
            {
                List<string> list_controls = new List<string>();
                if (string.IsNullOrEmpty(Power))
                {
                    AdminMasterVM adminMasterVM = new AdminMasterVM();
                    AdminMenuFunction model_menu_function = null;
                    string Admins = Appsettings.app("Web:Admins");//超管无限制
                    string _mastername = _accessor.HttpContext.Session.GetString(Appsettings.app("Web:LoginPrefix") + "AdminName");
                    var list_AdminMenuFunction = _context.Queryable<AdminMenuFunction>().Where(u => u.Status == 1).ToList();
                    if (Admins.Split(',').Contains(_mastername))
                    {
                        //超管，控件ID全加载出来
                        model_menu_function = list_AdminMenuFunction.FirstOrDefault(
                                      u => !string.IsNullOrEmpty(u.Area) && !string.IsNullOrEmpty(u.Controller) && !string.IsNullOrEmpty(u.Action) &&
                                      u.Area.ToLower() == area.ToLower() && u.Controller.ToLower() == controller.ToLower() &&
                                      (u.Action.ToLower() == action.ToLower() || (u.Action + "Data").ToLower() == action.ToLower()));
                        adminMasterVM.list_Functionid = list_AdminMenuFunction.Select(u => u.AdminMenuFunctionid).ToList();
                        //取子控件权限
                        list_controls = list_AdminMenuFunction
                            .Select(u => u.ControlId)
                            .ToList();
                    }
                    else
                    {
                        //普通用户
                        adminMasterVM = GetTokenItem<AdminMasterVM>(AdminMasterid, "Admininfo");
                        model_menu_function = list_AdminMenuFunction.FirstOrDefault(u =>
                            adminMasterVM.list_Functionid.Contains(u.AdminMenuFunctionid) &&
                             !Object.Equals(u.Area, null) && !Object.Equals(u.Controller, null) && !Object.Equals(u.Action, null) &&
                            u.Area.ToLower() == area.ToLower() && u.Controller.ToLower() == controller.ToLower() &&
                            (u.Action.ToLower() == action.ToLower() || (u.Action + "Data").ToLower() == action.ToLower()));
                        if (model_menu_function != null)
                        {
                            //取子控件权限
                            list_controls = list_AdminMenuFunction.Where(u => adminMasterVM.list_Functionid.Contains(u.AdminMenuFunctionid) && u.Fid == model_menu_function.AdminMenuFunctionid)
                                .Select(u => u.ControlId)
                                .ToList();
                            list_controls.Add(model_menu_function.ControlId);
                        }
                        else
                        {
                            return (false, null);
                        }
                    }
                    
                }
                UpdateToken(GetSysToken);
                return (true, list_controls);
            }
            else {
                return (false, null);
            }
        }

        /// <summary>
        /// 获取 Token 内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetTokenItem<T>(string key)
        {
            if (!RedisServer.Token.Exists(GetSysToken))
            {
                return default(T);
            }

            return RedisServer.Token.HGet<T>(GetSysToken, key);
        }


        /// <summary>
        /// 获取 Token 内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetTokenItem<T>(string token, string key)
        {
            if (!RedisServer.Token.Exists(token))
            {
                return default(T);
            }

            return RedisServer.Token.HGet<T>(token, key);
        }

        /// <summary>
        /// 获取 Token 内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetTokenItem<T>(long AdminMasterid,string key)
        {
            string token = "";
            if (!RedisServer.Token.Exists(AdminMasterid.ToString()))
            {
                throw new Exception($"GetSessionItem : id={AdminMasterid} has Exception");
            }
            else {
                token = RedisServer.Token.Get(AdminMasterid.ToString());
            }
            return GetTokenItem<T>(token, key);
        }

        /// <summary>
        /// 根据键获取对应的cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetCookie(string key)
        {
            var value = "";
            _accessor.HttpContext.Request.Cookies.TryGetValue(key, out value);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }
            return value;
        }
        #endregion


        /// <summary>
        /// //取用户对应的菜单ID和功能ID集
        /// </summary>
        /// <param name="admin_masterid"></param>
        /// <returns></returns>
        private  (List<long> list_functionid, List<long> list_menuid) GetFunctionids(long admin_masterid)
        {
            //取用户角色ID集
            var list_admin_masterRoles = _context.Queryable<AdminMasterRole>().Where(u => u.AdminMasterid == admin_masterid).ToList();
            var list_roleid = list_admin_masterRoles.Select(u => u.AdminRoleid).Distinct().ToList();


            //取角色对应的控件ID集
            var list_AdminRoleMenuFunctions = _context.Queryable<AdminRoleMenuFunction>().Where(u => u.Status != -1 && list_roleid.Contains(u.AdminRoleid)).ToList();
            var list_functionid = list_AdminRoleMenuFunctions.Select(u => u.AdminMenuFunctionid).Distinct().ToList();
            var list_menuid = list_AdminRoleMenuFunctions.Select(u => u.AdminMenuid).Distinct().ToList();
            if (list_menuid.Count > 0)
            {
                var list_admin_menu = _context.Queryable<AdminMenu>().Where(u => list_menuid.Contains(u.AdminMenuid)).ToList();
                for (int i = 0; i < list_admin_menu.Count; i++)
                {
                    var list = list_admin_menu[i].Path.Split('|').Where(u => !string.IsNullOrEmpty(u)).Select(u => long.Parse(u)).ToList();
                    list_menuid.AddRange(list);
                }
            }
            list_menuid = list_menuid.Distinct().ToList();
            return (list_functionid, list_menuid);
        }
    }
}
