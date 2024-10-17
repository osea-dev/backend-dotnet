using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;

namespace WeTalk.Console.CMD
{
    public class CmdHostService
    {
        private SqlSugarScope _context;
        private IConfiguration _config;

        private ILog log = LogManager.GetLogger(Program.repository.Name, typeof(CmdHostService));
        private readonly IMenkeService _menkeService;
        private readonly ISobotService _sobotService;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IOrderService _orderService;
        private readonly IMessageService _messageService;
        private readonly ILogger<CmdHostService> _logger;
        public CmdHostService(IHostApplicationLifetime applicationLifetime,SqlSugarScope dbcontext, IConfiguration configuration, IMenkeService menkeService, ISobotService sobotService, IOrderService orderService,
             IMessageService messageService, ILogger<CmdHostService> logger)
        {
            _config = configuration;
            _context = dbcontext;
            _menkeService = menkeService;
            _sobotService = sobotService;
            _orderService = orderService;
            _logger = logger;
            _messageService = messageService;
            _applicationLifetime = applicationLifetime;
        }

        public async Task Run()
        {
            if (Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") == "Development")
			{

                //await _menkeService.OmissionStudent("65519912@qq.com");
                //GetRemoteHelper.HttpWebRequestUrl("https://api.wetalk.com:8002/Api/V1/Test/Test1", "");
                //await _menkeService.ClassBeginsEmail();
                //await _orderService.PaymentReminder();
                //await _menkeService.CourseSync("1676149208");
                //var result_tmp = await _menkeService.LessonSync("1675213200");//1672484879
                //if (result_tmp.StatusCode == 0)
                //{
                //    var list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => result_tmp.Data.Contains(u.MenkeLessonId) && u.Isexe == 0).ToList();
                //    await _menkeService.LessonSplit(list_MenkeLesson);
                //}
                await _menkeService.LessonSync("1676195151");
                //await _menkeService.ChkDeleteSync(14400,10000);
                //await _menkeService.LessonSplit();
                //await _menkeService.AttendanceSync("2022-11-19");
                //await _sobotService.UpdateToken();
                //await _menkeService.RecordSync("1675837311");
                //await _menkeService.HomeworkSync("1676137740");
                //await _menkeService.HomeworkSubmitSync("1667232000");
                //await _menkeService.HomeworkRemarkSync("1668441600");
                //await _menkeService.CompleteTrialLessonEmail();
                //await _orderService.CheckPayStatus();
                //await _menkeService.TrialLessonEmail();
                //await _menkeService.LessonEmail();
                //await _menkeService.CompleteLessonEmail();
                //await _menkeService.AttendanceSync("2022-12-25");
                return;
            }
            var AppSty = _config["Web:AppSty"];
            //int s = _context.Database.ExecuteSqlRaw("update ue_task set isexe=0,lastchanged='" + DateTime.Now + "' where display=1 and isexe=1 and app_sty=" + AppSty);//DATE_ADD(lasttime,INTERVAL 2 HOUR)<NOW() and 
            int s = _context.Updateable<UeTask>()
                .SetColumns(u => new UeTask
                {
                    Isexe = 0,
                    Lastchanged = DateTime.Now
                })
                .Where(u => u.Display == 1 && u.Isexe == 1 && u.AppSty == AppSty)
                .ExecuteCommand();
            if (s > 0)
            {
                log.Info("主程序["+ AppSty +"]执行守护任务，重启" + s + "个任务");
            }
            var sql = "display=1 ";
            sql += " and (";
            sql += "(timing_sty=0 and (UNIX_TIMESTAMP(now()) - UNIX_TIMESTAMP(lasttime)) > min * 60)";//时间间隔控制
            sql += "or (timing_sty=1 and curtime()>timing and DATE_ADD(timing, INTERVAL 1 DAY)<=now()) ";// 定时每天，大于起跑时间，且间隔一天以上
            sql += "or (timing_sty=2 and dayofweek(curdate())=dayofweek(timing) and DATE_ADD(timing, INTERVAL 1 WEEK)<=now()) ";// 定时每周，大于起跑时间，且间隔一周以上
            sql += "or (timing_sty=3 and dayofweek(curdate())=dayofweek(timing) and DATE_ADD(timing, INTERVAL 1 MONTH)<=now()) ";// 定时每月，大于起跑时间，且间隔一月以上
            sql += "or (timing_sty=4 and dayofyear(curdate())=dayofyear(timing) and DATE_ADD(timing, INTERVAL 1 YEAR)<=now()) ";// 定时每年，大于起跑时间，且间隔一年以上
            sql += ")";
            if (!string.IsNullOrEmpty(AppSty))
            {
                sql += " and app_sty=" + AppSty + "";
            }
            var list_task = _context.Ado.SqlQuery<UeTask>(@"select * from ue_task where " + sql + " order by taskid");
            foreach (var model in list_task)
            {
                if (model.BlackBegintime != null && model.BlackEndtime != null && model.BlackBegintime.Value.ToFileTime() <= DateTime.Now.ToFileTime() && model.BlackEndtime.Value.ToFileTime() >= DateTime.Now.ToFileTime())
                {
                    continue;
                }
                if (model.Isexe == 0)
                {
                    model.Isexe = 1;
                    _context.Updateable(model).ExecuteCommand();

                    //执行各种任务
                    try
                    {
                        switch (model.Code.Trim())
                        {
                            case "PreStart"://预启动
                                GetRemoteHelper.HttpWebRequestUrl("https://api.wetalk.com:8002/Api/V1/Test/Test1", "");
                                break;
                            case "MenkeCourse"://增量同步拓课云中课程
                                await _menkeService.CourseSync(model.Paramenter);
                                break;
                            case "MenkeLesson"://增量同步拓课云中约好的课节
                                await _menkeService.LessonSync(model.Paramenter);
                                break;
                            case "MenkeLessonSplit"://执行拆分拓课云课节至学生已排课节中，含逻辑较多，比较耗时
                                await _menkeService.LessonSplit();
                                break;
                            case "SobotUpdateToken"://定时刷新智齿科技接口Token
                                await _sobotService.UpdateToken();
                                break;
                            case "MenkeRecord"://同步课堂回放信息
                                await _menkeService.RecordSync(model.Paramenter);
                                break;
                            case "MenkeAttendance"://同步课节考勤信息
                                await _menkeService.AttendanceSync(model.Paramenter);
                                break;
                            case "MenkeHomework"://同步作业列表
                                await _menkeService.HomeworkSync(model.Paramenter);
                                break;
                            case "MenkeHomeworkSubmit"://同步作业提交记录列表
                                await _menkeService.HomeworkSubmitSync(model.Paramenter);
                                break;
                            case "MenkeHomeworkRemark"://同步作业记录点评列表
                                await _menkeService.HomeworkRemarkSync(model.Paramenter);
                                break;
                            case "LessonEmail"://发送预约成功邮件(试听与正课)
                                await _menkeService.LessonEmail();
                                break;
                            case "CompleteTrialLessonEmail"://发送试听完成邮件(未出报告时不发MAIL)
                                await _menkeService.CompleteTrialLessonEmail();
                                break;
                            case "CompleteLesson"://发送正课完成(未出报告时不发MAIL)
                                await _menkeService.CompleteLessonEmail();
                                break;
                            case "ClassBeginsEmail"://发送课前提醒邮件
                                await _menkeService.ClassBeginsEmail();
                                break;
                            case "PaymentReminder"://付款超时提醒工单
                                await _orderService.PaymentReminder();
                                break;
                            case "CheckPayStatus"://定时检查支付状态
                                await _orderService.CheckPayStatus();
                                break;
                            case "SendEmail"://定时发生邮件
                                await _messageService.SendEmailTask();
                                break;
                            case "ChkCourseComparison"://双向检测课程比对
                                var begintime = string.IsNullOrEmpty(model.Paramenter) ? 0 : int.Parse(model.Paramenter);
                                if (begintime == 0) begintime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddDays(-1));
                                var endtime = DateHelper.ConvertDateTimeInt(DateHelper.ConvertIntToDateTime(begintime.ToString()).AddHours(1));
                                await _menkeService.ChkCourseComparison(begintime, endtime);
                                break;
                            case "OmissionStudent"://同步检测忘添学生的课节:
                                await _menkeService.OmissionStudent(model.Paramenter);
                                break;
                        }
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, $"计划任务执行失败,code={model.Code},Paramenter={model.Paramenter}");
                        continue;
                    }
                }
                model.Isexe = 0;
                switch (model.TimingSty)
                {
                    case 0://定时
                        model.Lasttime = DateTime.Now;
                        break;
                    case 1://每天
                        model.Timing = model.Timing.AddDays(1);
                        model.Lasttime = model.Timing;
                        break;
                    case 2://每周
                        model.Timing = model.Timing.AddDays(7);
                        model.Lasttime = model.Timing;
                        break;
                    case 3://每月
                        model.Timing = model.Timing.AddMonths(1);
                        model.Lasttime = model.Timing;
                        break;
                    case 4://每年
                        model.Timing = model.Timing.AddYears(1);
                        model.Lasttime = model.Timing;
                        break;
                }
                model.Lastchanged = DateTime.Now;
                _context.Updateable(model).UpdateColumns(u => new { u.Isexe, u.Lasttime, u.Timing, u.Lastchanged }).ExecuteCommand();
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //await Run();
            _applicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
