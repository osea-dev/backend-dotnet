namespace WeTalk.Common.Helper
{
	/// <summary>
	/// 常用函数基类
	/// </summary>
	public class WxErrHelper
	{

		#region 获取微信错误代码Msg
		/// <summary>
		/// 获取微信错误代码Msg
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static string GetWxErr(int code,string msg="")
		{
			switch (code)
			{
				case -1:
					msg = "系统繁忙，此时请开发者稍候再试";
					break;
				case 0:
					msg = "请求成功";
					break;
				case 1003:
					msg = "POST参数非法";
					break;
				case 20002:
					msg = "商品id不存在";
					break;
				case 40001:
					msg = "获取 access_token 时 AppSecret 错误，或者 access_token 无效。请开发者认真比对 AppSecret 的正确性，或查看是否正在为恰当的公众号调用接口";
					break;
				case 40002:
					msg = "不合法的凭证类型";
					break;
				case 40003:
					msg = "不合法的 OpenID ，请开发者确认 OpenID （该用户）是否已关注公众号，或是否是其他公众号的 OpenID";
					break;
				case 40004:
					msg = "不合法的媒体文件类型";
					break;
				case 40005:
					msg = "上传素材文件格式不对";
					break;
				case 40006:
					msg = "上传素材文件大小超出限制";
					break;
				case 40007:
					msg = "不合法的媒体文件 id";
					break;
				case 40008:
					msg = "不合法的消息类型";
					break;
				case 40009:
					msg = "图片尺寸太大";
					break;
				case 40010:
					msg = "不合法的语音文件大小";
					break;
				case 40011:
					msg = "不合法的视频文件大小";
					break;
				case 40012:
					msg = "不合法的缩略图文件大小";
					break;
				case 40013:
					msg = "不合法的appid";
					break;
				case 40014:
					msg = "不合法的 access_token ，请开发者认真比对 access_token 的有效性（如是否过期），或查看是否正在为恰当的公众号调用接口";
					break;
				case 40015:
					msg = "不合法的菜单类型";
					break;
				case 40016:
					msg = "不合法的按钮个数";
					break;
				case 40017:
					msg = "不合法的按钮类型";
					break;
				case 40018:
					msg = "不合法的按钮名字长度";
					break;
				case 40019:
					msg = "不合法的按钮 KEY 长度";
					break;
				case 40020:
					msg = "不合法的按钮 URL 长度";
					break;
				case 40021:
					msg = "不合法的菜单版本号";
					break;
				case 40022:
					msg = "不合法的子菜单级数";
					break;
				case 40023:
					msg = "不合法的子菜单按钮个数";
					break;
				case 40024:
					msg = "不合法的子菜单按钮类型";
					break;
				case 40025:
					msg = "不合法的子菜单按钮名字长度";
					break;
				case 40026:
					msg = "不合法的子菜单按钮 KEY 长度";
					break;
				case 40027:
					msg = "不合法的子菜单按钮 URL 长度";
					break;
				case 40028:
					msg = "不合法的自定义菜单使用用户";
					break;
				case 40029:
					msg = "无效的 oauth_code";
					break;
				case 40030:
					msg = "不合法的 refresh_token";
					break;
				case 40031:
					msg = "不合法的 openid 列表";
					break;
				case 40032:
					msg = "不合法的 openid 列表长度";
					break;
				case 40033:
					msg = "不合法的请求字符，不能包含 \\uxxxx 格式的字符";
					break;
				case 40034:
					msg = "invalid template size";
					break;
				case 40035:
					msg = "不合法的参数";
					break;
				case 40036:
					msg = "不合法的 template_id 长度";
					break;
				case 40037:
					msg = "不合法的 template_id";
					break;
				case 40038:
					msg = "不合法的请求格式";
					break;
				case 40039:
					msg = "不合法的 URL 长度";
					break;
				case 40040:
					msg = "invalid plugin token";
					break;
				case 40041:
					msg = "invalid plugin id";
					break;
				case 40042:
					msg = "invalid plugin session";
					break;
				case 40043:
					msg = "invalid fav type";
					break;
				case 40044:
					msg = "invalid size in link.title";
					break;
				case 40045:
					msg = "invalid size in link.description";
					break;
				case 40046:
					msg = "invalid size in link.iconurl";
					break;
				case 40047:
					msg = "invalid size in link.url";
					break;
				case 40048:
					msg = "无效的url";
					break;
				case 40049:
					msg = "invalid score report type";
					break;
				case 40050:
					msg = "不合法的分组 id";
					break;
				case 40051:
					msg = "分组名字不合法";
					break;
				case 40052:
					msg = "invalid action name";
					break;
				case 40053:
					msg = "invalid action info, please check document";
					break;
				case 40054:
					msg = "不合法的子菜单按钮 url 域名";
					break;
				case 40055:
					msg = "不合法的菜单按钮 url 域名";
					break;
				case 40056:
					msg = "invalid serial code";
					break;
				case 40057:
					msg = "invalid tabbar size";
					break;
				case 40058:
					msg = "invalid tabbar name size";
					break;
				case 40059:
					msg = "invalid msg id";
					break;
				case 40060:
					msg = "删除单篇图文时，指定的 article_idx 不合法";
					break;
				case 40062:
					msg = "invalid title size";
					break;
				case 40063:
					msg = "invalid message_ext size";
					break;
				case 40064:
					msg = "invalid app type";
					break;
				case 40065:
					msg = "invalid msg status";
					break;
				case 40066:
					msg = "不合法的 url ，递交的页面被sitemap标记为拦截";
					break;
				case 40067:
					msg = "invalid tvid";
					break;
				case 40068:
					msg = "contain mailcious url";
					break;
				case 40069:
					msg = "invalid hardware type";
					break;
				case 40070:
					msg = "invalid sku info";
					break;
				case 40071:
					msg = "invalid card type";
					break;
				case 40072:
					msg = "invalid location id";
					break;
				case 40073:
					msg = "invalid card id";
					break;
				case 40074:
					msg = "invalid pay template id";
					break;
				case 40075:
					msg = "invalid encrypt code";
					break;
				case 40076:
					msg = "invalid color id";
					break;
				case 40077:
					msg = "invalid score type";
					break;
				case 40078:
					msg = "invalid card status";
					break;
				case 40079:
					msg = "invalid time";
					break;
				case 40080:
					msg = "invalid card ext";
					break;
				case 40081:
					msg = "invalid template_id";
					break;
				case 40082:
					msg = "invalid banner picture size";
					break;
				case 40083:
					msg = "invalid banner url size";
					break;
				case 40084:
					msg = "invalid button desc size";
					break;
				case 40085:
					msg = "invalid button url size";
					break;
				case 40086:
					msg = "invalid sharelink logo size";
					break;
				case 40087:
					msg = "invalid sharelink desc size";
					break;
				case 40088:
					msg = "invalid sharelink title size";
					break;
				case 40089:
					msg = "invalid platform id";
					break;
				case 40090:
					msg = "invalid request source (bad client ip)";
					break;
				case 40091:
					msg = "invalid component ticket";
					break;
				case 40092:
					msg = "invalid remark name";
					break;
				case 40093:
					msg = "not completely ok, err_item will return location_id=-1,check your required_fields in json.";
					break;
				case 40094:
					msg = "invalid component credential";
					break;
				case 40095:
					msg = "bad source of caller";
					break;
				case 40096:
					msg = "invalid biztype";
					break;
				case 40097:
					msg = "参数错误";
					break;
				case 40098:
					msg = "invalid poiid";
					break;
				case 40099:
					msg = "invalid code, this code has consumed.";
					break;
				case 40100:
					msg = "invalid DateInfo, Make Sure OldDateInfoType==NewDateInfoType && NewBeginTime<=OldBeginTime && OldEndTime<= NewEndTime";
					break;
				case 40101:
					msg = "missing parameter";
					break;
				case 40102:
					msg = "invalid industry id";
					break;
				case 40103:
					msg = "invalid industry index";
					break;
				case 40104:
					msg = "invalid category id";
					break;
				case 40105:
					msg = "invalid view type";
					break;
				case 40106:
					msg = "invalid user name";
					break;
				case 40107:
					msg = "invalid card id! 1,card status must verify ok; 2,this card must have location_id";
					break;
				case 40108:
					msg = "invalid client version";
					break;
				case 40109:
					msg = "too many code size, must <= 100";
					break;
				case 40110:
					msg = "have empty code";
					break;
				case 40111:
					msg = "have same code";
					break;
				case 40112:
					msg = "can not set bind openid";
					break;
				case 40113:
					msg = "unsupported file type";
					break;
				case 40114:
					msg = "invalid index value";
					break;
				case 40115:
					msg = "invalid session from";
					break;
				case 40116:
					msg = "invalid code";
					break;
				case 40117:
					msg = "分组名字不合法";
					break;
				case 40118:
					msg = "media_id 大小不合法";
					break;
				case 40119:
					msg = "button 类型错误";
					break;
				case 40120:
					msg = "子 button 类型错误";
					break;
				case 40121:
					msg = "不合法的 media_id 类型";
					break;
				case 40122:
					msg = "invalid card quantity";
					break;
				case 40123:
					msg = "invalid task_id";
					break;
				case 40124:
					msg = "too many custom field!";
					break;
				case 40125:
					msg = "不合法的 AppID ，请开发者检查 AppID 的正确性，避免异常字符，注意大小写";
					break;
				case 40126:
					msg = "invalid text size";
					break;
				case 40127:
					msg = "invalid user-card status! Hint: the card was given to user, but may be deleted or expired or set unavailable !";
					break;
				case 40128:
					msg = "invalid media id! must be uploaded by api(cgi-bin/material/add_material)";
					break;
				case 40129:
					msg = "invalid scene";
					break;
				case 40130:
					msg = "invalid openid list size, at least two openid";
					break;
				case 40131:
					msg = "out of limit of ticket";
					break;
				case 40132:
					msg = "微信号不合法";
					break;
				case 40133:
					msg = "invalid encryt data";
					break;
				case 40135:
					msg = "invalid not supply bonus, can not change card_id which supply bonus to be not supply";
					break;
				case 40136:
					msg = "invalid use DepositCodeMode, make sure sku.quantity>DepositCode.quantity";
					break;
				case 40137:
					msg = "不支持的图片格式";
					break;
				case 40138:
					msg = "emphasis word can not be first neither remark";
					break;
				case 40139:
					msg = "invalid sub merchant id";
					break;
				case 40140:
					msg = "invalid sub merchant status";
					break;
				case 40141:
					msg = "invalid image url";
					break;
				case 40142:
					msg = "invalid sharecard parameters";
					break;
				case 40143:
					msg = "invalid least cost info, should be 0";
					break;
				case 40144:
					msg = "1)maybe share_card_list.num or consume_share_self_num too big; 2)maybe card_id_list also has self-card_id;3)maybe card_id_list has many different card_id;4)maybe both consume_share_self_num and share_card_list.num bigger than 0";
					break;
				case 40145:
					msg = "invalid update! Can not both set PayCell and CenterCellInfo(include: center_title, center_sub_title, center_url).";
					break;
				case 40146:
					msg = "invalid openid! card may be marked by other user!";
					break;
				case 40147:
					msg = "invalid consume! Consume time overranging restricts.";
					break;
				case 40148:
					msg = "invalid friends card type";
					break;
				case 40149:
					msg = "invalid use time limit";
					break;
				case 40150:
					msg = "invalid card parameters";
					break;
				case 40151:
					msg = "invalid card info, text/pic hit antispam";
					break;
				case 40152:
					msg = "invalid group id";
					break;
				case 40153:
					msg = "self consume cell for friends card must need verify code";
					break;
				case 40154:
					msg = "invalid voip parameters";
					break;
				case 40155:
					msg = "请勿添加其他公众号的主页链接";
					break;
				case 40156:
					msg = "invalid face recognize parameters";
					break;
				case 40157:
					msg = "invalid picture, has no face";
					break;
				case 40158:
					msg = "invalid use_custom_code, need be false";
					break;
				case 40159:
					msg = "invalid length for path, or the data is not json string";
					break;
				case 40160:
					msg = "invalid image file";
					break;
				case 40161:
					msg = "image file not match";
					break;
				case 40162:
					msg = "invalid lifespan";
					break;
				case 40163:
					msg = "oauth_code已使用";
					break;
				case 40164:
					msg = "invalid ip, not in whitelist";
					break;
				case 40165:
					msg = "invalid weapp pagepath";
					break;
				case 40166:
					msg = "invalid weapp appid";
					break;
				case 40167:
					msg = "there is no relation with plugin appid";
					break;
				case 40168:
					msg = "unlinked weapp card";
					break;
				case 40169:
					msg = "invalid length for scene, or the data is not json string";
					break;
				case 40170:
					msg = "args count exceed count limit";
					break;
				case 40171:
					msg = "product id can not empty and the length cannot exceed 32";
					break;
				case 40172:
					msg = "can not have same product id";
					break;
				case 40173:
					msg = "there is no bind relation";
					break;
				case 40174:
					msg = "not card user";
					break;
				case 40175:
					msg = "invalid material id";
					break;
				case 40176:
					msg = "invalid template id";
					break;
				case 40177:
					msg = "invalid product id";
					break;
				case 40178:
					msg = "invalid sign";
					break;
				case 40179:
					msg = "Function is adjusted, rules are not allowed to add or update";
					break;
				case 40180:
					msg = "invalid client tmp token";
					break;
				case 40181:
					msg = "invalid opengid";
					break;
				case 40182:
					msg = "invalid pack_id";
					break;
				case 40183:
					msg = "invalid product_appid, product_appid should bind with wxa_appid";
					break;
				case 40184:
					msg = "invalid url path";
					break;
				case 40185:
					msg = "invalid auth_token, or auth_token is expired";
					break;
				case 40186:
					msg = "invalid delegate";
					break;
				case 40187:
					msg = "invalid ip";
					break;
				case 40188:
					msg = "invalid scope";
					break;
				case 40189:
					msg = "invalid width";
					break;
				case 40190:
					msg = "invalid delegate time";
					break;
				case 40191:
					msg = "invalid pic_url";
					break;
				case 40192:
					msg = "invalid author in news";
					break;
				case 40193:
					msg = "invalid recommend length";
					break;
				case 40194:
					msg = "illegal recommend";
					break;
				case 40195:
					msg = "invalid show_num";
					break;
				case 40196:
					msg = "invalid smartmsg media_id";
					break;
				case 40197:
					msg = "invalid smartmsg media num";
					break;
				case 40198:
					msg = "invalid default msg article size, must be same as show_num";
					break;
				case 40199:
					msg = "运单 ID 不存在，未查到运单";
					break;
				case 40200:
					msg = "invalid account type";
					break;
				case 40201:
					msg = "invalid check url";
					break;
				case 40202:
					msg = "invalid check action";
					break;
				case 40203:
					msg = "invalid check operator";
					break;
				case 40204:
					msg = "can not delete wash or rumor article";
					break;
				case 40205:
					msg = "invalid check keywords string";
					break;
				case 40206:
					msg = "invalid check begin stamp";
					break;
				case 40207:
					msg = "invalid check alive seconds";
					break;
				case 40208:
					msg = "invalid check notify id";
					break;
				case 40209:
					msg = "invalid check notify msg";
					break;
				case 40210:
					msg = "pages 中的path参数不存在或为空";
					break;
				case 40211:
					msg = "invalid scope_data";
					break;
				case 40212:
					msg = "paegs 当中存在不合法的query，query格式遵循URL标准，即k1=v1&k2=v2";
					break;
				case 40213:
					msg = "invalid href tag";
					break;
				case 40214:
					msg = "invalid href text";
					break;
				case 40215:
					msg = "invalid image count";
					break;
				case 40216:
					msg = "invalid desc";
					break;
				case 40217:
					msg = "invalid video count";
					break;
				case 40218:
					msg = "invalid video id";
					break;
				case 40219:
					msg = "pages不存在或者参数为空";
					break;
				case 40220:
					msg = "data_list is empty";
					break;
				case 40221:
					msg = "invalid Content-Encoding";
					break;
				case 40222:
					msg = "invalid request idc domain";
					break;
				case 41001:
					msg = "缺少 access_token 参数";
					break;
				case 41002:
					msg = "缺少 appid 参数";
					break;
				case 41003:
					msg = "缺少 refresh_token 参数";
					break;
				case 41004:
					msg = "缺少 secret 参数";
					break;
				case 41005:
					msg = "缺少多媒体文件数据，传输素材无视频或图片内容";
					break;
				case 41006:
					msg = "缺少 media_id 参数";
					break;
				case 41007:
					msg = "缺少子菜单数据";
					break;
				case 41008:
					msg = "缺少 oauth code";
					break;
				case 41009:
					msg = "缺少 openid";
					break;
				case 41010:
					msg = "缺失 url 参数";
					break;
				case 41011:
					msg = "missing required fields! please check document and request json!";
					break;
				case 41012:
					msg = "missing card id";
					break;
				case 41013:
					msg = "missing code";
					break;
				case 41014:
					msg = "missing ticket_class";
					break;
				case 41015:
					msg = "missing show_time";
					break;
				case 41016:
					msg = "missing screening_room";
					break;
				case 41017:
					msg = "missing seat_number";
					break;
				case 41018:
					msg = "missing component_appid";
					break;
				case 41019:
					msg = "missing platform_secret";
					break;
				case 41020:
					msg = "missing platform_ticket";
					break;
				case 41021:
					msg = "missing component_access_token";
					break;
				case 41024:
					msg = "missing “display” field";
					break;
				case 41025:
					msg = "poi_list empty";
					break;
				case 41026:
					msg = "missing image list info, text maybe empty";
					break;
				case 41027:
					msg = "missing voip call key";
					break;
				case 41028:
					msg = "invalid form id";
					break;
				case 41029:
					msg = "form id used count reach limit";
					break;
				case 41030:
					msg = "page路径不正确，需要保证在现网版本小程序中存在，与app.json保持一致";
					break;
				case 41031:
					msg = "the form id have been blocked!";
					break;
				case 41032:
					msg = "not allow to send message with submitted form id, for punishment";
					break;
				case 41033:
					msg = "只允许通过api创建的小程序使用";
					break;
				case 41034:
					msg = "not allow to send message with submitted form id, for punishment";
					break;
				case 41035:
					msg = "not allow to send message with prepay id, for punishment";
					break;
				case 41036:
					msg = "appid ad cid";
					break;
				case 41037:
					msg = "appid ad_mch_appid";
					break;
				case 41038:
					msg = "appid pos_type";
					break;
				case 42001:
					msg = "access_token 超时，请检查 access_token 的有效期，请参考基础支持 - 获取 access_token 中，对 access_token 的详细机制说明";
					break;
				case 42002:
					msg = "refresh_token 超时";
					break;
				case 42003:
					msg = "oauth_code 超时";
					break;
				case 42004:
					msg = "plugin token expired";
					break;
				case 42005:
					msg = "api usage expired";
					break;
				case 42006:
					msg = "component_access_token expired";
					break;
				case 42007:
					msg = "用户修改微信密码， accesstoken 和 refreshtoken 失效，需要重新授权";
					break;
				case 42008:
					msg = "voip call key expired";
					break;
				case 42009:
					msg = "client tmp token expired";
					break;
				case 43001:
					msg = "需要 GET 请求";
					break;
				case 43002:
					msg = "需要 POST 请求";
					break;
				case 43003:
					msg = "需要 HTTPS 请求";
					break;
				case 43004:
					msg = "需要接收者关注";
					break;
				case 43005:
					msg = "需要好友关系";
					break;
				case 43006:
					msg = "require not block";
					break;
				case 43007:
					msg = "require bizuser authorize";
					break;
				case 43008:
					msg = "require biz pay auth";
					break;
				case 43009:
					msg = "can not use custom code, need authorize";
					break;
				case 43010:
					msg = "can not use balance, need authorize";
					break;
				case 43011:
					msg = "can not use bonus, need authorize";
					break;
				case 43012:
					msg = "can not use custom url, need authorize";
					break;
				case 43013:
					msg = "can not use shake card, need authorize";
					break;
				case 43014:
					msg = "require check agent";
					break;
				case 43015:
					msg = "require authorize by wechat team to use this function!";
					break;
				case 43016:
					msg = "小程序未认证";
					break;
				case 43017:
					msg = "require location id!";
					break;
				case 43018:
					msg = "code has no been mark!";
					break;
				case 43019:
					msg = "需要将接收者从黑名单中移除";
					break;
				case 43100:
					msg = "change template too frequently";
					break;
				case 43101:
					msg = "用户拒绝接受消息，如果用户之前曾经订阅过，则表示用户取消了订阅关系";
					break;
				case 43102:
					msg = "the tempalte is not subscriptiontype";
					break;
				case 43103:
					msg = "the api only can cancel the subscription";
					break;
				case 43104:
					msg = "this appid does not have permission";
					break;
				case 43105:
					msg = "news has no binding relation with template_id";
					break;
				case 43106:
					msg = "not allow to add template, for punishment";
					break;
				case 44001:
					msg = "多媒体文件为空";
					break;
				case 44002:
					msg = "POST 的数据包为空";
					break;
				case 44003:
					msg = "图文消息内容为空";
					break;
				case 44004:
					msg = "文本消息内容为空";
					break;
				case 44005:
					msg = "空白的列表";
					break;
				case 44006:
					msg = "empty file data";
					break;
				case 44007:
					msg = "repeated msg id";
					break;
				case 44997:
					msg = "image url size out of limit";
					break;
				case 44998:
					msg = "keyword string media size out of limit";
					break;
				case 44999:
					msg = "keywords list size out of limit";
					break;
				case 45000:
					msg = "msg_id size out of limit";
					break;
				case 45001:
					msg = "多媒体文件大小超过限制";
					break;
				case 45002:
					msg = "消息内容超过限制";
					break;
				case 45003:
					msg = "标题字段超过限制";
					break;
				case 45004:
					msg = "描述字段超过限制";
					break;
				case 45005:
					msg = "链接字段超过限制";
					break;
				case 45006:
					msg = "图片链接字段超过限制";
					break;
				case 45007:
					msg = "语音播放时间超过限制";
					break;
				case 45008:
					msg = "图文消息超过限制";
					break;
				case 45009:
					msg = "接口调用超过限制";
					break;
				case 45010:
					msg = "创建菜单个数超过限制";
					break;
				case 45011:
					msg = "API 调用太频繁，请稍候再试";
					break;
				case 45012:
					msg = "模板大小超过限制";
					break;
				case 45013:
					msg = "too many template args";
					break;
				case 45014:
					msg = "template message size out of limit";
					break;
				case 45015:
					msg = "回复时间超过限制";
					break;
				case 45016:
					msg = "系统分组，不允许修改";
					break;
				case 45017:
					msg = "分组名字过长";
					break;
				case 45018:
					msg = "分组数量超过上限";
					break;
				case 45019:
					msg = "too many openid, please input less";
					break;
				case 45020:
					msg = "too many image, please input less";
					break;
				case 45021:
					msg = "some argument may be out of length limit! please check document and request json!";
					break;
				case 45022:
					msg = "bonus is out of limit";
					break;
				case 45023:
					msg = "balance is out of limit";
					break;
				case 45024:
					msg = "rank template number is out of limit";
					break;
				case 45025:
					msg = "poiid count is out of limit";
					break;
				case 45026:
					msg = "template num exceeds limit";
					break;
				case 45027:
					msg = "template conflict with industry";
					break;
				case 45028:
					msg = "has no masssend quota";
					break;
				case 45029:
					msg = "qrcode count out of limit";
					break;
				case 45030:
					msg = "limit cardid, not support this function";
					break;
				case 45031:
					msg = "stock is out of limit";
					break;
				case 45032:
					msg = "not inner ip for special acct in white-list";
					break;
				case 45033:
					msg = "user get card num is out of get_limit";
					break;
				case 45034:
					msg = "media file count is out of limit";
					break;
				case 45035:
					msg = "access clientip is not registered, not in ip-white-list";
					break;
				case 45036:
					msg = "User receive announcement limit";
					break;
				case 45037:
					msg = "user out of time limit or never talked in tempsession";
					break;
				case 45038:
					msg = "user subscribed, cannot use tempsession api";
					break;
				case 45039:
					msg = "card_list_size out of limit";
					break;
				case 45040:
					msg = "reach max monthly quota limit";
					break;
				case 45041:
					msg = "this card reach total sku quantity limit!";
					break;
				case 45042:
					msg = "limit card type, this card type can NOT create by sub merchant";
					break;
				case 45043:
					msg = "can not set share_friends=true because has no Abstract Or Text_Img_List has no img Or image url not valid";
					break;
				case 45044:
					msg = "icon url size in abstract is out of limit";
					break;
				case 45045:
					msg = "unauthorized friends card, please contact administrator";
					break;
				case 45046:
					msg = "operate field conflict, CenterCell, PayCell, SelfConsumeCell conflict";
					break;
				case 45047:
					msg = "客服接口下行条数超过上限";
					break;
				case 45048:
					msg = "menu use invalid type";
					break;
				case 45049:
					msg = "ivr use invalid type";
					break;
				case 45050:
					msg = "custom msg use invalid type";
					break;
				case 45051:
					msg = "template msg use invalid link";
					break;
				case 45052:
					msg = "masssend msg use invalid type";
					break;
				case 45053:
					msg = "exceed consume verify code size";
					break;
				case 45054:
					msg = "below consume verify code size";
					break;
				case 45055:
					msg = "the code is not in consume verify code charset";
					break;
				case 45056:
					msg = "too many tag now, no need to add new";
					break;
				case 45057:
					msg = "can't delete the tag that has too many fans";
					break;
				case 45058:
					msg = "can't modify sys tag";
					break;
				case 45059:
					msg = "can not tagging one user too much";
					break;
				case 45060:
					msg = "media is applied in ivr or menu, can not be deleted";
					break;
				case 45061:
					msg = "maybe the update frequency is too often, please try again";
					break;
				case 45062:
					msg = "has agreement ad. please use mp.weixin.qq.com";
					break;
				case 45063:
					msg = "accesstoken is not xiaochengxu";
					break;
				case 45064:
					msg = "创建菜单包含未关联的小程序";
					break;
				case 45065:
					msg = "相同 clientmsgid 已存在群发记录，返回数据中带有已存在的群发任务的 msgid";
					break;
				case 45066:
					msg = "相同 clientmsgid 重试速度过快，请间隔1分钟重试";
					break;
				case 45067:
					msg = "clientmsgid 长度超过限制";
					break;
				case 45068:
					msg = "file size out of limit";
					break;
				case 45069:
					msg = "product list size out of limit";
					break;
				case 45070:
					msg = "the business account have been created";
					break;
				case 45071:
					msg = "business account not found";
					break;
				case 45072:
					msg = "command字段取值不对";
					break;
				case 45073:
					msg = "not inner vip for sns in white list";
					break;
				case 45074:
					msg = "material list size out of limit, you should delete the useless material";
					break;
				case 45075:
					msg = "invalid keyword id";
					break;
				case 45076:
					msg = "invalid count";
					break;
				case 45077:
					msg = "number of business account reach limit";
					break;
				case 45078:
					msg = "nickname is illegal!";
					break;
				case 45079:
					msg = "nickname is forbidden!(matched forbidden keyword)";
					break;
				case 45080:
					msg = "下发输入状态，需要之前30秒内跟用户有过消息交互";
					break;
				case 45081:
					msg = "已经在输入状态，不可重复下发";
					break;
				case 45082:
					msg = "need icp license for the url domain";
					break;
				case 45083:
					msg = "the speed out of range";
					break;
				case 45084:
					msg = "No speed message";
					break;
				case 45085:
					msg = "speed server err";
					break;
				case 45086:
					msg = "invalid attrbute 'data-miniprogram-appid'";
					break;
				case 45087:
					msg = "customer service message from this account have been blocked!";
					break;
				case 45088:
					msg = "action size out of limit";
					break;
				case 45089:
					msg = "expired";
					break;
				case 45090:
					msg = "invalid group msg ticket";
					break;
				case 45091:
					msg = "account_name is illegal!";
					break;
				case 45092:
					msg = "no voice data";
					break;
				case 45093:
					msg = "no quota to send msg";
					break;
				case 45094:
					msg = "not allow to send custom message when user enter session, for punishment";
					break;
				case 45095:
					msg = "not allow to modify stock for the advertisement batch";
					break;
				case 45096:
					msg = "invalid qrcode";
					break;
				case 45097:
					msg = "invalid qrcode prefix";
					break;
				case 45098:
					msg = "msgmenu list size is out of limit";
					break;
				case 45099:
					msg = "msgmenu item content size is out of limit";
					break;
				case 45100:
					msg = "invalid size of keyword_id_list";
					break;
				case 45101:
					msg = "hit upload limit";
					break;
				case 45102:
					msg = "this api have been blocked temporarily.";
					break;
				case 45103:
					msg = "This API has been unsupported";
					break;
				case 45104:
					msg = "reach max domain quota limit";
					break;
				case 45154:
					msg = "the consume verify code not found";
					break;
				case 45155:
					msg = "the consume verify code is existed";
					break;
				case 45156:
					msg = "the consume verify code's length not invalid";
					break;
				case 45157:
					msg = "invalid tag name";
					break;
				case 45158:
					msg = "tag name too long";
					break;
				case 45159:
					msg = "invalid tag id";
					break;
				case 45160:
					msg = "invalid category to create card";
					break;
				case 45161:
					msg = "this video id must be generated by calling upload api";
					break;
				case 45162:
					msg = "invalid type";
					break;
				case 45163:
					msg = "invalid sort_method";
					break;
				case 45164:
					msg = "invalid offset";
					break;
				case 45165:
					msg = "invalid limit";
					break;
				case 45166:
					msg = "invalid content";
					break;
				case 45167:
					msg = "invalid voip call key";
					break;
				case 45168:
					msg = "keyword in blacklist";
					break;
				case 45501:
					msg = "part or whole of the requests from the very app is temporary blocked by supervisor";
					break;
				case 46001:
					msg = "不存在媒体数据，media_id 不存在";
					break;
				case 46002:
					msg = "不存在的菜单版本";
					break;
				case 46003:
					msg = "不存在的菜单数据";
					break;
				case 46004:
					msg = "不存在的用户";
					break;
				case 46005:
					msg = "poi no exist";
					break;
				case 46006:
					msg = "voip file not exist";
					break;
				case 46007:
					msg = "file being transcoded, please try later";
					break;
				case 46008:
					msg = "result id not exist";
					break;
				case 46009:
					msg = "there is no user data";
					break;
				case 46101:
					msg = "this api have been not supported since 2020-01-11 00:00:00, please use new api(subscribeMessage)!";
					break;
				case 47001:
					msg = "解析 JSON/XML 内容错误";
					break;
				case 47002:
					msg = "data format error, do NOT use json unicode encode (\\uxxxx\\uxxxx), please use utf8 encoded text!";
					break;
				case 47003:
					msg = "模板参数不准确，可能为空或者不满足规则，errmsg会提示具体是哪个字段出错";
					break;
				case 47004:
					msg = "每次提交的页面数超过1000（备注：每次提交页面数应小于或等于1000）";
					break;
				case 47005:
					msg = "tabbar no exist";
					break;
				case 47006:
					msg = "当天提交页面数达到了配额上限，请明天再试";
					break;
				case 47101:
					msg = "搜索结果总数超过了1000条";
					break;
				case 47102:
					msg = "next_page_info参数错误";
					break;
				case 47501:
					msg = "参数 activity_id 错误";
					break;
				case 47502:
					msg = "参数 target_state 错误";
					break;
				case 47503:
					msg = "参数 version_type 错误";
					break;
				case 47504:
					msg = "activity_id";
					break;
				case 48001:
					msg = "api 功能未授权，请确认公众号已获得该接口，可以在公众平台官网 - 开发者中心页中查看接口权限";
					break;
				case 48002:
					msg = "粉丝拒收消息（粉丝在公众号选项中，关闭了 “ 接收消息 ” ）";
					break;
				case 48003:
					msg = "user not agree mass-send protocol";
					break;
				case 48004:
					msg = "api 接口被封禁，请登录 mp.weixin.qq.com 查看详情";
					break;
				case 48005:
					msg = "api 禁止删除被自动回复和自定义菜单引用的素材";
					break;
				case 48006:
					msg = "api 禁止清零调用次数，因为清零次数达到上限";
					break;
				case 48007:
					msg = "forbid to use other's voip call key";
					break;
				case 48008:
					msg = "没有该类型消息的发送权限";
					break;
				case 48009:
					msg = "this api is expired";
					break;
				case 48010:
					msg = "forbid to modify the material， please see more information on mp.weixin.qq.com";
					break;
				case 48011:
					msg = "disabled template id";
					break;
				case 48012:
					msg = "invalid token";
					break;
				case 48013:
					msg = "该视频非新接口上传，不能用于视频消息群发";
					break;
				case 48014:
					msg = "该视频审核状态异常，请检查后重试";
					break;
				case 48015:
					msg = "该账号无留言功能权限";
					break;
				case 48016:
					msg = "该账号不满足智能配置“观看更多”视频条件";
					break;
				case 49001:
					msg = "not same appid with appid of access_token";
					break;
				case 49002:
					msg = "empty openid or transid";
					break;
				case 49003:
					msg = "not match openid with appid";
					break;
				case 49004:
					msg = "not match signature";
					break;
				case 49005:
					msg = "not existed transid";
					break;
				case 49006:
					msg = "missing arg two_dim_code";
					break;
				case 49007:
					msg = "invalid two_dim_code";
					break;
				case 49008:
					msg = "invalid qrcode";
					break;
				case 49009:
					msg = "missing arg qrcode";
					break;
				case 49010:
					msg = "invalid partner id";
					break;
				case 49300:
					msg = "not existed feedbackid";
					break;
				case 49301:
					msg = "feedback exist";
					break;
				case 49302:
					msg = "feedback status already changed";
					break;
				case 50001:
					msg = "用户未授权该 api";
					break;
				case 50002:
					msg = "用户受限，可能是用户帐号被冻结或注销";
					break;
				case 50003:
					msg = "user unexpected, maybe not in white list";
					break;
				case 50004:
					msg = "user not allow to use accesstoken, maybe for punishment";
					break;
				case 50005:
					msg = "用户未关注公众号";
					break;
				case 50006:
					msg = "user has switched off friends authorization";
					break;
				case 51000:
					msg = "enterprise father account not exist";
					break;
				case 51001:
					msg = "enterprise child account not belong to the father";
					break;
				case 51002:
					msg = "enterprise verify message not correct";
					break;
				case 51003:
					msg = "invalid enterprise child list size";
					break;
				case 51004:
					msg = "not a enterprise father account";
					break;
				case 51005:
					msg = "not a enterprise child account";
					break;
				case 51006:
					msg = "invalid nick name";
					break;
				case 51007:
					msg = "not a enterprise account";
					break;
				case 51008:
					msg = "invalid email";
					break;
				case 51009:
					msg = "invalid pwd";
					break;
				case 51010:
					msg = "repeated email";
					break;
				case 51011:
					msg = "access deny";
					break;
				case 51012:
					msg = "need verify code";
					break;
				case 51013:
					msg = "wrong verify code";
					break;
				case 51014:
					msg = "need modify pwd";
					break;
				case 51015:
					msg = "user not exist";
					break;
				case 51020:
					msg = "tv info not exist";
					break;
				case 51021:
					msg = "stamp crossed";
					break;
				case 51022:
					msg = "invalid stamp range";
					break;
				case 51023:
					msg = "stamp not match date";
					break;
				case 51024:
					msg = "empty program name";
					break;
				case 51025:
					msg = "empty action url";
					break;
				case 51026:
					msg = "program name size out of limit";
					break;
				case 51027:
					msg = "action url size out of limit";
					break;
				case 51028:
					msg = "invalid program name";
					break;
				case 51029:
					msg = "invalid action url";
					break;
				case 51030:
					msg = "invalid action id";
					break;
				case 51031:
					msg = "invalid action offset";
					break;
				case 51032:
					msg = "empty action title";
					break;
				case 51033:
					msg = "action title size out of limit";
					break;
				case 51034:
					msg = "empty action icon url";
					break;
				case 51035:
					msg = "action icon url out of limit";
					break;
				case 52000:
					msg = "pic is not from cdn";
					break;
				case 52001:
					msg = "wechat price is not less than origin price";
					break;
				case 52002:
					msg = "category/sku is wrong";
					break;
				case 52003:
					msg = "product id not existed";
					break;
				case 52004:
					msg = "category id is not exist, or doesn't has sub category";
					break;
				case 52005:
					msg = "quantity is zero";
					break;
				case 52006:
					msg = "area code is invalid";
					break;
				case 52007:
					msg = "express template param is error";
					break;
				case 52008:
					msg = "express template id is not existed";
					break;
				case 52009:
					msg = "group name is empty";
					break;
				case 52010:
					msg = "group id is not existed";
					break;
				case 52011:
					msg = "mod_action is invalid";
					break;
				case 52012:
					msg = "shelf components count is greater than 20";
					break;
				case 52013:
					msg = "shelf component is empty";
					break;
				case 52014:
					msg = "shelf id is not existed";
					break;
				case 52015:
					msg = "order id is not existed";
					break;
				case 52016:
					msg = "order filter param is invalid";
					break;
				case 52017:
					msg = "order express param is invalid";
					break;
				case 52018:
					msg = "order delivery param is invalid";
					break;
				case 52019:
					msg = "brand name empty";
					break;
				case 53000:
					msg = "principal limit exceed";
					break;
				case 53001:
					msg = "principal in black list";
					break;
				case 53002:
					msg = "mobile limit exceed";
					break;
				case 53003:
					msg = "idcard limit exceed";
					break;
				case 53010:
					msg = "名称格式不合法";
					break;
				case 53011:
					msg = "名称检测命中频率限制";
					break;
				case 53012:
					msg = "禁止使用该名称";
					break;
				case 53013:
					msg = "公众号：名称与已有公众号名称重复;小程序：该名称与已有小程序名称重复";
					break;
				case 53014:
					msg = "公众号：公众号已有{名称 A+}时，需与该帐号相同主体才可申请{名称 A};小程序：小程序已有{名称 A+}时，需与该帐号相同主体才可申请{名称 A}";
					break;
				case 53015:
					msg = "公众号：该名称与已有小程序名称重复，需与该小程序帐号相同主体才可申请;小程序：该名称与已有公众号名称重复，需与该公众号帐号相同主体才可申请";
					break;
				case 53016:
					msg = "公众号：该名称与已有多个小程序名称重复，暂不支持申请;小程序：该名称与已有多个公众号名称重复，暂不支持申请";
					break;
				case 53017:
					msg = "公众号：小程序已有{名称 A+}时，需与该帐号相同主体才可申请{名称 A};小程序：公众号已有{名称 A+}时，需与该帐号相同主体才可申请{名称 A}";
					break;
				case 53018:
					msg = "名称命中微信号";
					break;
				case 53019:
					msg = "名称在保护期内";
					break;
				case 53100:
					msg = "order not found";
					break;
				case 53101:
					msg = "order already paid";
					break;
				case 53102:
					msg = "already has checking order, can not apply";
					break;
				case 53103:
					msg = "order can not do refill";
					break;
				case 53200:
					msg = "本月功能介绍修改次数已用完";
					break;
				case 53201:
					msg = "功能介绍内容命中黑名单关键字";
					break;
				case 53202:
					msg = "本月头像修改次数已用完";
					break;
				case 53203:
					msg = "can't be modified for the time being";
					break;
				case 53204:
					msg = "signature invalid";
					break;
				case 53300:
					msg = "超出每月次数限制";
					break;
				case 53301:
					msg = "超出可配置类目总数限制";
					break;
				case 53302:
					msg = "当前账号主体类型不允许设置此种类目";
					break;
				case 53303:
					msg = "提交的参数不合法";
					break;
				case 53304:
					msg = "与已有类目重复";
					break;
				case 53305:
					msg = "包含未通过IPC校验的类目";
					break;
				case 53306:
					msg = "修改类目只允许修改类目资质，不允许修改类目ID";
					break;
				case 53307:
					msg = "只有审核失败的类目允许修改";
					break;
				case 53308:
					msg = "审核中的类目不允许删除";
					break;
				case 53309:
					msg = "社交红包不允许删除";
					break;
				case 53310:
					msg = "类目超过上限，但是可以添加apply_reason参数申请更多类目";
					break;
				case 53311:
					msg = "需要提交资料信息";
					break;
				case 60005:
					msg = "empty jsapi name";
					break;
				case 60006:
					msg = "user cancel the auth";
					break;
				case 61000:
					msg = "invalid component type";
					break;
				case 61001:
					msg = "component type and component appid is not match";
					break;
				case 61002:
					msg = "the third appid is not open KF";
					break;
				case 61003:
					msg = "component is not authorized by this account";
					break;
				case 61004:
					msg = "api 功能未授权，请确认公众号/小程序已获得该接口，可以在公众平台官网 - 开发者中心页中查看接口权限";
					break;
				case 61005:
					msg = "component ticket is expired";
					break;
				case 61006:
					msg = "component ticket is invalid";
					break;
				case 61007:
					msg = "api is unauthorized to component";
					break;
				case 61008:
					msg = "component req key is duplicated";
					break;
				case 61009:
					msg = "code is invalid";
					break;
				case 61010:
					msg = "code is expired";
					break;
				case 61011:
					msg = "invalid component";
					break;
				case 61012:
					msg = "invalid option name";
					break;
				case 61013:
					msg = "invalid option value";
					break;
				case 61014:
					msg = "must use component token for component api";
					break;
				case 61015:
					msg = "must use biz account token for not component api";
					break;
				case 61016:
					msg = "function category of API need be confirmed by component";
					break;
				case 61017:
					msg = "function category is not authorized";
					break;
				case 61018:
					msg = "already confirm";
					break;
				case 61019:
					msg = "not need confirm";
					break;
				case 61020:
					msg = "err parameter";
					break;
				case 61021:
					msg = "can't confirm";
					break;
				case 61022:
					msg = "can't resubmit";
					break;
				case 61023:
					msg = "refresh_token is invalid";
					break;
				case 61024:
					msg = "must use api(api_component_token) to get token for component acct";
					break;
				case 61025:
					msg = "read-only option";
					break;
				case 61026:
					msg = "register access deny";
					break;
				case 61027:
					msg = "register limit exceed";
					break;
				case 61028:
					msg = "component is unpublished";
					break;
				case 61029:
					msg = "component need republish with base category";
					break;
				case 61030:
					msg = "component cancel authorization not allowed";
					break;
				case 61051:
					msg = "invalid realname type";
					break;
				case 61052:
					msg = "need to be certified";
					break;
				case 61053:
					msg = "realname exceed limits";
					break;
				case 61054:
					msg = "realname in black list";
					break;
				case 61055:
					msg = "exceed quota per month";
					break;
				case 61056:
					msg = "copy_wx_verify is required option";
					break;
				case 61058:
					msg = "invalid ticket";
					break;
				case 61061:
					msg = "overseas access deny";
					break;
				case 61063:
					msg = "admin exceed limits";
					break;
				case 61064:
					msg = "admin in black list";
					break;
				case 61065:
					msg = "idcard exceed limits";
					break;
				case 61066:
					msg = "idcard in black list";
					break;
				case 61067:
					msg = "mobile exceed limits";
					break;
				case 61068:
					msg = "mobile in black list";
					break;
				case 61069:
					msg = "invalid admin";
					break;
				case 61070:
					msg = "name, idcard, wechat name not in accordance";
					break;
				case 61100:
					msg = "invalid url";
					break;
				case 61101:
					msg = "invalid openid";
					break;
				case 61102:
					msg = "share relation not existed";
					break;
				case 61200:
					msg = "product wording not set";
					break;
				case 61300:
					msg = "invalid base info";
					break;
				case 61301:
					msg = "invalid detail info";
					break;
				case 61302:
					msg = "invalid action info";
					break;
				case 61303:
					msg = "brand info not exist";
					break;
				case 61304:
					msg = "invalid product id";
					break;
				case 61305:
					msg = "invalid key info";
					break;
				case 61306:
					msg = "invalid appid";
					break;
				case 61307:
					msg = "invalid card id";
					break;
				case 61308:
					msg = "base info not exist";
					break;
				case 61309:
					msg = "detail info not exist";
					break;
				case 61310:
					msg = "action info not exist";
					break;
				case 61311:
					msg = "invalid media info";
					break;
				case 61312:
					msg = "invalid buffer size";
					break;
				case 61313:
					msg = "invalid buffer";
					break;
				case 61314:
					msg = "invalid qrcode extinfo";
					break;
				case 61315:
					msg = "invalid local ext info";
					break;
				case 61316:
					msg = "key conflict";
					break;
				case 61317:
					msg = "ticket invalid";
					break;
				case 61318:
					msg = "verify not pass";
					break;
				case 61319:
					msg = "category invalid";
					break;
				case 61320:
					msg = "merchant info not exist";
					break;
				case 61321:
					msg = "cate id is a leaf node";
					break;
				case 61322:
					msg = "category id no permision";
					break;
				case 61323:
					msg = "barcode no permision";
					break;
				case 61324:
					msg = "exceed max action num";
					break;
				case 61325:
					msg = "brandinfo invalid store mgr type";
					break;
				case 61326:
					msg = "anti-spam blocked";
					break;
				case 61327:
					msg = "comment reach limit";
					break;
				case 61328:
					msg = "comment data is not the newest";
					break;
				case 61329:
					msg = "comment hit ban word";
					break;
				case 61330:
					msg = "image already add";
					break;
				case 61331:
					msg = "image never add";
					break;
				case 61332:
					msg = "warning, image quanlity too low";
					break;
				case 61333:
					msg = "warning, image simility too high";
					break;
				case 61334:
					msg = "product not exists";
					break;
				case 61335:
					msg = "key apply fail";
					break;
				case 61336:
					msg = "check status fail";
					break;
				case 61337:
					msg = "product already exists";
					break;
				case 61338:
					msg = "forbid delete";
					break;
				case 61339:
					msg = "firmcode claimed";
					break;
				case 61340:
					msg = "check firm info fail";
					break;
				case 61341:
					msg = "too many white list uin";
					break;
				case 61342:
					msg = "keystandard not match";
					break;
				case 61343:
					msg = "keystandard error";
					break;
				case 61344:
					msg = "id map not exists";
					break;
				case 61345:
					msg = "invalid action code";
					break;
				case 61346:
					msg = "invalid actioninfo store";
					break;
				case 61347:
					msg = "invalid actioninfo media";
					break;
				case 61348:
					msg = "invalid actioninfo text";
					break;
				case 61350:
					msg = "invalid input data";
					break;
				case 61351:
					msg = "input data exceed max size";
					break;
				case 61400:
					msg = "kf_account error";
					break;
				case 61401:
					msg = "kf system alredy transfer";
					break;
				case 61450:
					msg = "系统错误 (system error)";
					break;
				case 61451:
					msg = "参数错误 (invalid parameter)";
					break;
				case 61452:
					msg = "无效客服账号 (invalid kf_account)";
					break;
				case 61453:
					msg = "客服帐号已存在 (kf_account exsited)";
					break;
				case 61454:
					msg = "客服帐号名长度超过限制 ( 仅允许 10 个英文字符，不包括 @ 及 @ 后的公众号的微信号 )(invalid kf_acount length)";
					break;
				case 61455:
					msg = "客服帐号名包含非法字符 ( 仅允许英文 + 数字 )(illegal character in kf_account)";
					break;
				case 61456:
					msg = "客服帐号个数超过限制 (10 个客服账号 )(kf_account count exceeded)";
					break;
				case 61457:
					msg = "无效头像文件类型 (invalid file type)";
					break;
				case 61500:
					msg = "日期格式错误";
					break;
				case 61501:
					msg = "date range error";
					break;
				case 61502:
					msg = "this is game miniprogram, data api is not supported";
					break;
				case 61503:
					msg = "data not ready, please try later";
					break;
				case 62001:
					msg = "trying to access other's app";
					break;
				case 62002:
					msg = "app name already exists";
					break;
				case 62003:
					msg = "please provide at least one platform";
					break;
				case 62004:
					msg = "invalid app name";
					break;
				case 62005:
					msg = "invalid app id";
					break;
				case 63001:
					msg = "部分参数为空";
					break;
				case 63002:
					msg = "无效的签名";
					break;
				case 63003:
					msg = "invalid signature method";
					break;
				case 63004:
					msg = "no authroize";
					break;
				case 63149:
					msg = "gen ticket fail";
					break;
				case 63152:
					msg = "set ticket fail";
					break;
				case 63153:
					msg = "shortid decode fail";
					break;
				case 63154:
					msg = "invalid status";
					break;
				case 63155:
					msg = "invalid color";
					break;
				case 63156:
					msg = "invalid tag";
					break;
				case 63157:
					msg = "invalid recommend";
					break;
				case 63158:
					msg = "branditem out of limits";
					break;
				case 63159:
					msg = "retail_price empty";
					break;
				case 63160:
					msg = "priceinfo invalid";
					break;
				case 63161:
					msg = "antifake module num limit";
					break;
				case 63162:
					msg = "antifake native_type err";
					break;
				case 63163:
					msg = "antifake link not exists";
					break;
				case 63164:
					msg = "module type not exist";
					break;
				case 63165:
					msg = "module info not exist";
					break;
				case 63166:
					msg = "item is beding verified";
					break;
				case 63167:
					msg = "item not published";
					break;
				case 63168:
					msg = "verify not pass";
					break;
				case 63169:
					msg = "already published";
					break;
				case 63170:
					msg = "only banner or media";
					break;
				case 63171:
					msg = "card num limit";
					break;
				case 63172:
					msg = "user num limit";
					break;
				case 63173:
					msg = "text num limit";
					break;
				case 63174:
					msg = "link card user sum limit";
					break;
				case 63175:
					msg = "detail info error";
					break;
				case 63176:
					msg = "not this type";
					break;
				case 63177:
					msg = "src or secretkey or version or expired_time is wrong";
					break;
				case 63178:
					msg = "appid wrong";
					break;
				case 63179:
					msg = "openid num limit";
					break;
				case 63180:
					msg = "this app msg not found";
					break;
				case 63181:
					msg = "get history app msg end";
					break;
				case 63182:
					msg = "openid_list empty";
					break;
				case 65001:
					msg = "unknown deeplink type";
					break;
				case 65002:
					msg = "deeplink unauthorized";
					break;
				case 65003:
					msg = "bad deeplink";
					break;
				case 65004:
					msg = "deeplinks of the very type are supposed to have short-life";
					break;
				case 65104:
					msg = "invalid categories";
					break;
				case 65105:
					msg = "invalid photo url";
					break;
				case 65106:
					msg = "poi audit state must be approved";
					break;
				case 65107:
					msg = "poi not allowed modify now";
					break;
				case 65109:
					msg = "invalid business name";
					break;
				case 65110:
					msg = "invalid address";
					break;
				case 65111:
					msg = "invalid telephone";
					break;
				case 65112:
					msg = "invalid city";
					break;
				case 65113:
					msg = "invalid province";
					break;
				case 65114:
					msg = "photo list empty";
					break;
				case 65115:
					msg = "poi_id is not exist";
					break;
				case 65116:
					msg = "poi has been deleted";
					break;
				case 65117:
					msg = "cannot delete poi";
					break;
				case 65118:
					msg = "store status is invalid";
					break;
				case 65119:
					msg = "lack of qualification for relevant principals";
					break;
				case 65120:
					msg = "category info is not found";
					break;
				case 65201:
					msg = "room_name is empty, please check your input";
					break;
				case 65202:
					msg = "user_id is empty, please check your input";
					break;
				case 65203:
					msg = "invalid check ticket";
					break;
				case 65204:
					msg = "invalid check ticket opt code";
					break;
				case 65205:
					msg = "check ticket out of time";
					break;
				case 65301:
					msg = "不存在此 menuid 对应的个性化菜单";
					break;
				case 65302:
					msg = "没有相应的用户";
					break;
				case 65303:
					msg = "没有默认菜单，不能创建个性化菜单";
					break;
				case 65304:
					msg = "MatchRule 信息为空";
					break;
				case 65305:
					msg = "个性化菜单数量受限";
					break;
				case 65306:
					msg = "不支持个性化菜单的帐号";
					break;
				case 65307:
					msg = "个性化菜单信息为空";
					break;
				case 65308:
					msg = "包含没有响应类型的 button";
					break;
				case 65309:
					msg = "个性化菜单开关处于关闭状态";
					break;
				case 65310:
					msg = "填写了省份或城市信息，国家信息不能为空";
					break;
				case 65311:
					msg = "填写了城市信息，省份信息不能为空";
					break;
				case 65312:
					msg = "不合法的国家信息";
					break;
				case 65313:
					msg = "不合法的省份信息";
					break;
				case 65314:
					msg = "不合法的城市信息";
					break;
				case 65315:
					msg = "not fans";
					break;
				case 65316:
					msg = "该公众号的菜单设置了过多的域名外跳（最多跳转到 3 个域名的链接）";
					break;
				case 65317:
					msg = "不合法的 URL";
					break;
				case 65318:
					msg = "must use utf-8 charset";
					break;
				case 65319:
					msg = "not allow to create menu";
					break;
				case 65400:
					msg = "please enable new custom service, or wait for a while if you have enabled";
					break;
				case 65401:
					msg = "invalid custom service account";
					break;
				case 65402:
					msg = "the custom service account need to bind a wechat user";
					break;
				case 65403:
					msg = "illegal nickname";
					break;
				case 65404:
					msg = "illegal custom service account";
					break;
				case 65405:
					msg = "custom service account number reach limit";
					break;
				case 65406:
					msg = "custom service account exists";
					break;
				case 65407:
					msg = "the wechat user have been one of your workers";
					break;
				case 65408:
					msg = "you have already invited the wechat user";
					break;
				case 65409:
					msg = "wechat account invalid";
					break;
				case 65410:
					msg = "too many custom service accounts bound by the worker";
					break;
				case 65411:
					msg = "a effective invitation to bind the custom service account exists";
					break;
				case 65412:
					msg = "the custom service account have been bound by a wechat user";
					break;
				case 65413:
					msg = "no effective session for the customer";
					break;
				case 65414:
					msg = "another worker is serving the customer";
					break;
				case 65415:
					msg = "the worker is not online";
					break;
				case 65416:
					msg = "param invalid, please check";
					break;
				case 65417:
					msg = "it is too long from the starttime to endtime";
					break;
				case 65450:
					msg = "homepage not exists";
					break;
				case 68002:
					msg = "invalid store type";
					break;
				case 68003:
					msg = "invalid store name";
					break;
				case 68004:
					msg = "invalid store wxa path";
					break;
				case 68005:
					msg = "miss store wxa path";
					break;
				case 68006:
					msg = "invalid kefu type";
					break;
				case 68007:
					msg = "invalid kefu wxa path";
					break;
				case 68008:
					msg = "invalid kefu phone number";
					break;
				case 68009:
					msg = "invalid sub mch id";
					break;
				case 68010:
					msg = "store id has exist";
					break;
				case 68011:
					msg = "miss store name";
					break;
				case 68012:
					msg = "miss create time";
					break;
				case 68013:
					msg = "invalid status";
					break;
				case 68014:
					msg = "invalid receiver info";
					break;
				case 68015:
					msg = "invalid product";
					break;
				case 68016:
					msg = "invalid pay type";
					break;
				case 68017:
					msg = "invalid fast mail no";
					break;
				case 68018:
					msg = "invalid busi id";
					break;
				case 68019:
					msg = "miss product sku";
					break;
				case 68020:
					msg = "invalid service type";
					break;
				case 68021:
					msg = "invalid service status";
					break;
				case 68022:
					msg = "invalid service_id";
					break;
				case 68023:
					msg = "service_id has exist";
					break;
				case 68024:
					msg = "miss service wxa path";
					break;
				case 68025:
					msg = "invalid product sku";
					break;
				case 68026:
					msg = "invalid product spu";
					break;
				case 68027:
					msg = "miss product spu";
					break;
				case 68028:
					msg = "can not find product spu and spu in order list";
					break;
				case 68029:
					msg = "sku and spu duplicated";
					break;
				case 68030:
					msg = "busi_id has exist";
					break;
				case 68031:
					msg = "update fail";
					break;
				case 68032:
					msg = "busi_id not exist";
					break;
				case 68033:
					msg = "store no exist";
					break;
				case 68034:
					msg = "miss product number";
					break;
				case 68035:
					msg = "miss wxa order detail path";
					break;
				case 68036:
					msg = "there is no enough products to refund";
					break;
				case 68037:
					msg = "invalid refund info";
					break;
				case 68038:
					msg = "shipped but no fast mail info";
					break;
				case 68039:
					msg = "invalid wechat pay no";
					break;
				case 68040:
					msg = "all product has been refunded, the order can not be finished";
					break;
				case 68041:
					msg = "invalid service create time, it must bigger than the time of order";
					break;
				case 68042:
					msg = "invalid total cost, it must be smaller than the sum of product and shipping cost";
					break;
				case 68043:
					msg = "invalid role";
					break;
				case 68044:
					msg = "invalid service_available args";
					break;
				case 68045:
					msg = "invalid order type";
					break;
				case 68046:
					msg = "invalid order deliver type";
					break;
				case 68500:
					msg = "require store_id";
					break;
				case 68501:
					msg = "invalid store_id";
					break;
				case 71001:
					msg = "invalid parameter, parameter is zero or missing";
					break;
				case 71002:
					msg = "invalid orderid, may be the other parameter not fit with orderid";
					break;
				case 71003:
					msg = "coin not enough";
					break;
				case 71004:
					msg = "card is expired";
					break;
				case 71005:
					msg = "limit exe count";
					break;
				case 71006:
					msg = "limit coin count, 1 <= coin_count <= 100000";
					break;
				case 71007:
					msg = "order finish";
					break;
				case 71008:
					msg = "order time out";
					break;
				case 72001:
					msg = "no match card";
					break;
				case 72002:
					msg = "mchid is not bind appid";
					break;
				case 72003:
					msg = "invalid card type, need member card";
					break;
				case 72004:
					msg = "mchid is occupied by the other appid";
					break;
				case 72005:
					msg = "out of mchid size limit";
					break;
				case 72006:
					msg = "invald title";
					break;
				case 72007:
					msg = "invalid reduce cost, can not less than 100";
					break;
				case 72008:
					msg = "invalid least cost, most larger than reduce cost";
					break;
				case 72009:
					msg = "invalid get limit, can not over 50";
					break;
				case 72010:
					msg = "invalid mchid";
					break;
				case 72011:
					msg = "invalid activate_ticket.Maybe this ticket is not belong this AppId";
					break;
				case 72012:
					msg = "activate_ticket has been expired";
					break;
				case 72013:
					msg = "unauthorized order_id or authorization is expired";
					break;
				case 72014:
					msg = "task card share stock can not modify stock";
					break;
				case 72015:
					msg = "unauthorized create invoice";
					break;
				case 72016:
					msg = "unauthorized create member card";
					break;
				case 72017:
					msg = "invalid invoice title";
					break;
				case 72018:
					msg = "duplicate order id, invoice had inserted to user";
					break;
				case 72019:
					msg = "limit msg operation card list size, must <= 5";
					break;
				case 72020:
					msg = "limit consume in use limit";
					break;
				case 72021:
					msg = "unauthorized create general card";
					break;
				case 72022:
					msg = "user unexpected, please add user to white list";
					break;
				case 72023:
					msg = "invoice has been lock by others";
					break;
				case 72024:
					msg = "invoice status error";
					break;
				case 72025:
					msg = "invoice token error";
					break;
				case 72026:
					msg = "need set wx_activate true";
					break;
				case 72027:
					msg = "invoice action error";
					break;
				case 72028:
					msg = "invoice never set pay mch info";
					break;
				case 72029:
					msg = "invoice never set auth field";
					break;
				case 72030:
					msg = "invalid mchid";
					break;
				case 72031:
					msg = "invalid params";
					break;
				case 72032:
					msg = "pay gift card rule expired";
					break;
				case 72033:
					msg = "pay gift card rule status err";
					break;
				case 72034:
					msg = "invlid rule id";
					break;
				case 72035:
					msg = "biz reject insert";
					break;
				case 72036:
					msg = "invoice is busy, try again please";
					break;
				case 72037:
					msg = "invoice owner error";
					break;
				case 72038:
					msg = "invoice order never auth";
					break;
				case 72039:
					msg = "invoice must be lock first";
					break;
				case 72040:
					msg = "invoice pdf error";
					break;
				case 72041:
					msg = "billing_code and billing_no invalid";
					break;
				case 72042:
					msg = "billing_code and billing_no repeated";
					break;
				case 72043:
					msg = "billing_code or billing_no size error";
					break;
				case 72044:
					msg = "scan text out of time";
					break;
				case 72045:
					msg = "check_code is empty";
					break;
				case 72046:
					msg = "pdf_url is invalid";
					break;
				case 72047:
					msg = "pdf billing_code or pdf billing_no is error";
					break;
				case 72048:
					msg = "insert too many invoice, need auth again";
					break;
				case 72049:
					msg = "never auth";
					break;
				case 72050:
					msg = "auth expired, need auth again";
					break;
				case 72051:
					msg = "app type error";
					break;
				case 72052:
					msg = "get too many invoice";
					break;
				case 72053:
					msg = "user never auth";
					break;
				case 72054:
					msg = "invoices is inserting, wait a moment please";
					break;
				case 72055:
					msg = "too many invoices";
					break;
				case 72056:
					msg = "order_id repeated, please check order_id";
					break;
				case 72057:
					msg = "today insert limit";
					break;
				case 72058:
					msg = "callback biz error";
					break;
				case 72059:
					msg = "this invoice is giving to others, wait a moment please";
					break;
				case 72060:
					msg = "this invoice has been cancelled, check the reimburse_status please";
					break;
				case 72061:
					msg = "this invoice has been closed, check the reimburse_status please";
					break;
				case 72062:
					msg = "this code_auth_key is limited, try other code_auth_key please";
					break;
				case 72063:
					msg = "biz contact is empty, set contact first please";
					break;
				case 72064:
					msg = "tbc error";
					break;
				case 72065:
					msg = "tbc logic error";
					break;
				case 72066:
					msg = "the card is send for advertisement, not allow modify time and budget";
					break;
				case 72067:
					msg = "BatchInsertAuthKey_Expired";
					break;
				case 72068:
					msg = "BatchInsertAuthKey_Owner";
					break;
				case 72069:
					msg = "BATCHTASKRUN_ERROR";
					break;
				case 72070:
					msg = "BIZ_TITLE_KEY_OUT_TIME";
					break;
				case 72071:
					msg = "Discern_GaoPeng_Error";
					break;
				case 72072:
					msg = "Discern_Type_Error";
					break;
				case 72073:
					msg = "Fee_Error";
					break;
				case 72074:
					msg = "HAS_Auth";
					break;
				case 72075:
					msg = "HAS_SEND";
					break;
				case 72076:
					msg = "INVOICESIGN";
					break;
				case 72077:
					msg = "KEY_DELETED";
					break;
				case 72078:
					msg = "KEY_EXPIRED";
					break;
				case 72079:
					msg = "MOUNT_ERROR";
					break;
				case 72080:
					msg = "NO_FOUND";
					break;
				case 72081:
					msg = "No_Pull_Pdf";
					break;
				case 72082:
					msg = "PDF_CHECK_ERROR";
					break;
				case 72083:
					msg = "PULL_PDF_FAIL";
					break;
				case 72084:
					msg = "PUSH_BIZ_EMPTY";
					break;
				case 72085:
					msg = "SDK_APPID_ERROR";
					break;
				case 72086:
					msg = "SDK_BIZ_ERROR";
					break;
				case 72087:
					msg = "SDK_URL_ERROR";
					break;
				case 72088:
					msg = "Search_Title_Fail";
					break;
				case 72089:
					msg = "TITLE_BUSY";
					break;
				case 72090:
					msg = "TITLE_NO_FOUND";
					break;
				case 72091:
					msg = "TOKEN_ERR";
					break;
				case 72092:
					msg = "USER_TITLE_NOT_FOUND";
					break;
				case 72093:
					msg = "Verify_3rd_Fail";
					break;
				case 73000:
					msg = "sys error make out invoice failed";
					break;
				case 73001:
					msg = "wxopenid error";
					break;
				case 73002:
					msg = "ddh orderid empty";
					break;
				case 73003:
					msg = "wxopenid empty";
					break;
				case 73004:
					msg = "fpqqlsh empty";
					break;
				case 73005:
					msg = "not a commercial";
					break;
				case 73006:
					msg = "kplx empty";
					break;
				case 73007:
					msg = "nsrmc empty";
					break;
				case 73008:
					msg = "nsrdz empty";
					break;
				case 73009:
					msg = "nsrdh empty";
					break;
				case 73010:
					msg = "ghfmc empty";
					break;
				case 73011:
					msg = "kpr empty";
					break;
				case 73012:
					msg = "jshj empty";
					break;
				case 73013:
					msg = "hjje empty";
					break;
				case 73014:
					msg = "hjse empty";
					break;
				case 73015:
					msg = "hylx empty";
					break;
				case 73016:
					msg = "nsrsbh empty";
					break;
				case 73100:
					msg = "kaipiao plat error";
					break;
				case 73101:
					msg = "nsrsbh not cmp";
					break;
				case 73103:
					msg = "invalid wxa appid in url_cell, wxa appid is need to bind biz appid";
					break;
				case 73104:
					msg = "reach frequency limit";
					break;
				case 73105:
					msg = "Kp plat make invoice timeout, please try again with the same fpqqlsh";
					break;
				case 73106:
					msg = "Fpqqlsh exist with different ddh";
					break;
				case 73107:
					msg = "Fpqqlsh is processing, please wait and query later";
					break;
				case 73108:
					msg = "This ddh with other fpqqlsh already exist";
					break;
				case 73109:
					msg = "This Fpqqlsh not exist in kpplat";
					break;
				case 73200:
					msg = "get card detail by card id and code fail";
					break;
				case 73201:
					msg = "get cloud invoice record fail";
					break;
				case 73202:
					msg = "get appinfo fail";
					break;
				case 73203:
					msg = "get invoice category or rule kv error";
					break;
				case 73204:
					msg = "request card not exist";
					break;
				case 73205:
					msg = "朋友的券玩法升级中，当前暂停创建，请创建其他类型卡券";
					break;
				case 73206:
					msg = "朋友的券玩法升级中，当前暂停券点充值，请创建其他类型卡券";
					break;
				case 73207:
					msg = "朋友的券玩法升级中，当前暂停开通券点账户";
					break;
				case 73208:
					msg = "朋友的券玩法升级中，当前不支持修改库存";
					break;
				case 73209:
					msg = "朋友的券玩法升级中，当前不支持修改有效期";
					break;
				case 73210:
					msg = "当前批次不支持修改卡券批次库存";
					break;
				case 73211:
					msg = "不再支持配置网页链接跳转，请选择小程序替代";
					break;
				case 73212:
					msg = "unauthorized backup member";
					break;
				case 73213:
					msg = "invalid code type";
					break;
				case 73214:
					msg = "the user is already a member";
					break;
				case 73215:
					msg = "支付打通券能力已下线，请直接使用微信支付代金券API：https://pay.weixin.qq.com/wiki/doc/apiv3/wxpay/marketing/convention/chapter1_1.shtml";
					break;
				case 73216:
					msg = "不合法的按钮名字，请从中选择一个:使用礼品卡/立即使用/去点外卖";
					break;
				case 73217:
					msg = "礼品卡本身没有设置appname和path，不允许在修改接口设置";
					break;
				case 73218:
					msg = "未授权使用礼品卡落地页跳转小程序功能";
					break;
				case 74000:
					msg = "not find this wx_hotel_id info";
					break;
				case 74001:
					msg = "request some param empty";
					break;
				case 74002:
					msg = "request some param error";
					break;
				case 74003:
					msg = "request some param error";
					break;
				case 74004:
					msg = "datetime error";
					break;
				case 74005:
					msg = "checkin mode error";
					break;
				case 74007:
					msg = "carid from error";
					break;
				case 74008:
					msg = "this hotel routecode not exist";
					break;
				case 74009:
					msg = "this hotel routecode info error contract developer";
					break;
				case 74010:
					msg = "maybe not support report mode";
					break;
				case 74011:
					msg = "pic deocde not ok maybe its not good picdata";
					break;
				case 74021:
					msg = "verify sys erro";
					break;
				case 74022:
					msg = "inner police erro";
					break;
				case 74023:
					msg = "unable to detect the face";
					break;
				case 74040:
					msg = "report checkin 2 lvye sys erro";
					break;
				case 74041:
					msg = "report checkou 2 lvye sys erro";
					break;
				case 75001:
					msg = "some param emtpy please check";
					break;
				case 75002:
					msg = "param illegal please check";
					break;
				case 75003:
					msg = "sys error kv store error";
					break;
				case 75004:
					msg = "sys error kvstring store error";
					break;
				case 75005:
					msg = "product not exist please check your product_id";
					break;
				case 75006:
					msg = "order not exist please check order_id and buyer_appid";
					break;
				case 75007:
					msg = "do not allow this status to change please check this order_id status now";
					break;
				case 75008:
					msg = "product has exist please use new id";
					break;
				case 75009:
					msg = "notify order status failed";
					break;
				case 75010:
					msg = "buyer bussiness info not exist";
					break;
				case 75011:
					msg = "you had registered";
					break;
				case 75012:
					msg = "store image key to kv error, please try again";
					break;
				case 75013:
					msg = "get image fail, please check you image key";
					break;
				case 75014:
					msg = "this key is not belong to you";
					break;
				case 75015:
					msg = "this key is expired";
					break;
				case 75016:
					msg = "encrypt decode key fail";
					break;
				case 75017:
					msg = "encrypt encode key fail";
					break;
				case 75018:
					msg = "bind buyer business info fail please contact us";
					break;
				case 75019:
					msg = "this key is empty, user may not upload file";
					break;
				case 80000:
					msg = "系统错误，请稍后再试";
					break;
				case 80001:
					msg = "参数格式校验错误";
					break;
				case 80002:
					msg = "签名失败";
					break;
				case 80003:
					msg = "该日期订单未生成";
					break;
				case 80004:
					msg = "用户未绑卡";
					break;
				case 80005:
					msg = "姓名不符";
					break;
				case 80006:
					msg = "身份证不符";
					break;
				case 80007:
					msg = "获取城市信息失败";
					break;
				case 80008:
					msg = "未找到指定少儿信息";
					break;
				case 80009:
					msg = "少儿身份证不符";
					break;
				case 80010:
					msg = "少儿未绑定";
					break;
				case 80011:
					msg = "签约号不符";
					break;
				case 80012:
					msg = "该地区局方配置不存在";
					break;
				case 80013:
					msg = "调用方appid与局方配置不匹配";
					break;
				case 80014:
					msg = "获取消息账号失败";
					break;
				case 80066:
					msg = "非法的插件版本";
					break;
				case 80067:
					msg = "找不到使用的插件";
					break;
				case 80082:
					msg = "没有权限使用该插件";
					break;
				case 80101:
					msg = "商家未接入";
					break;
				case 80111:
					msg = "实名校验code不存在";
					break;
				case 80112:
					msg = "code并发冲突";
					break;
				case 80113:
					msg = "无效code";
					break;
				case 80201:
					msg = "report_type无效";
					break;
				case 80202:
					msg = "service_type无效";
					break;
				case 80300:
					msg = "申请单不存在";
					break;
				case 80301:
					msg = "申请单不属于该账号";
					break;
				case 80302:
					msg = "激活号段有重叠";
					break;
				case 80303:
					msg = "码格式错误";
					break;
				case 80304:
					msg = "该码未激活";
					break;
				case 80305:
					msg = "激活失败";
					break;
				case 80306:
					msg = "码索引超出申请范围";
					break;
				case 80307:
					msg = "申请已存在";
					break;
				case 80308:
					msg = "子任务未完成";
					break;
				case 80309:
					msg = "子任务文件过期";
					break;
				case 80310:
					msg = "子任务不存在";
					break;
				case 80311:
					msg = "获取文件失败";
					break;
				case 80312:
					msg = "加密数据失败";
					break;
				case 80313:
					msg = "加密数据密钥不存在，请联系接口人申请";
					break;
				case 81000:
					msg = "can not set page_id in AddGiftCardPage";
					break;
				case 81001:
					msg = "card_list is empty";
					break;
				case 81002:
					msg = "card_id is not giftcard";
					break;
				case 81004:
					msg = "banner_pic_url is empty";
					break;
				case 81005:
					msg = "banner_pic_url is not from cdn";
					break;
				case 81006:
					msg = "giftcard_wrap_pic_url_list is empty";
					break;
				case 81007:
					msg = "giftcard_wrap_pic_url_list is not from cdn";
					break;
				case 81008:
					msg = "address is empty";
					break;
				case 81009:
					msg = "service_phone is invalid";
					break;
				case 81010:
					msg = "biz_description is empty";
					break;
				case 81011:
					msg = "invalid page_id";
					break;
				case 81012:
					msg = "invalid order_id";
					break;
				case 81013:
					msg = "invalid TIME_RANGE, begin_time + 31day must less than end_time";
					break;
				case 81014:
					msg = "invalid count! count must equal or less than 100";
					break;
				case 81015:
					msg = "invalid category_index OR category.title is empty OR is_banner but has_category_index";
					break;
				case 81016:
					msg = "is_banner is more than 1";
					break;
				case 81017:
					msg = "order status error, please check pay status or gifting_status";
					break;
				case 81018:
					msg = "refund reduplicate, the order is already refunded";
					break;
				case 81019:
					msg = "lock order fail! the order is refunding by another request";
					break;
				case 81020:
					msg = "Invalid Args! page_id.size!=0 but all==true, or page_id.size==0 but all==false.";
					break;
				case 81021:
					msg = "Empty theme_pic_url.";
					break;
				case 81022:
					msg = "Empty theme.title.";
					break;
				case 81023:
					msg = "Empty theme.title_title.";
					break;
				case 81024:
					msg = "Empty theme.item_list.";
					break;
				case 81025:
					msg = "Empty theme.pic_item_list.";
					break;
				case 81026:
					msg = "Invalid theme.title.length .";
					break;
				case 81027:
					msg = "Empty background_pic_url.";
					break;
				case 81028:
					msg = "Empty default_gifting_msg.";
					break;
				case 81029:
					msg = "Duplicate order_id";
					break;
				case 81030:
					msg = "PreAlloc code fail";
					break;
				case 81031:
					msg = "Too many theme participate_activity";
					break;
				case 82000:
					msg = "biz_template_id not exist";
					break;
				case 82001:
					msg = "result_page_style_id not exist";
					break;
				case 82002:
					msg = "deal_msg_style_id not exist";
					break;
				case 82003:
					msg = "card_style_id not exist";
					break;
				case 82010:
					msg = "biz template not audit OK";
					break;
				case 82011:
					msg = "biz template banned";
					break;
				case 82020:
					msg = "user not use service first";
					break;
				case 82021:
					msg = "exceed long period";
					break;
				case 82022:
					msg = "exceed long period max send cnt";
					break;
				case 82023:
					msg = "exceed short period max send cnt";
					break;
				case 82024:
					msg = "exceed data size limit";
					break;
				case 82025:
					msg = "invalid url";
					break;
				case 82026:
					msg = "service disabled";
					break;
				case 82027:
					msg = "invalid miniprogram appid";
					break;
				case 82100:
					msg = "wx_cs_code should not be empty.";
					break;
				case 82101:
					msg = "wx_cs_code is invalid.";
					break;
				case 82102:
					msg = "wx_cs_code is expire.";
					break;
				case 82103:
					msg = "user_ip should not be empty.";
					break;
				case 82200:
					msg = "公众平台账号与服务id不匹配";
					break;
				case 82201:
					msg = "该停车场已存在，请勿重复添加";
					break;
				case 82202:
					msg = "该停车场信息不存在，请先导入";
					break;
				case 82203:
					msg = "停车场价格格式不正确";
					break;
				case 82204:
					msg = "appid与code不匹配";
					break;
				case 82205:
					msg = "wx_park_code字段为空";
					break;
				case 82206:
					msg = "wx_park_code无效或已过期";
					break;
				case 82207:
					msg = "电话字段为空";
					break;
				case 82208:
					msg = "关闭时间格式不正确";
					break;
				case 82300:
					msg = "该appid不支持开通城市服务插件";
					break;
				case 82301:
					msg = "添加插件失败";
					break;
				case 82302:
					msg = "未添加城市服务插件";
					break;
				case 82303:
					msg = "fileid无效";
					break;
				case 82304:
					msg = "临时文件过期";
					break;
				case 83000:
					msg = "there is some param not exist";
					break;
				case 83001:
					msg = "system error";
					break;
				case 83002:
					msg = "create_url_sence_failed";
					break;
				case 83003:
					msg = "appid maybe error or retry";
					break;
				case 83004:
					msg = "create appid auth failed or retry";
					break;
				case 83005:
					msg = "wxwebencrytoken errro";
					break;
				case 83006:
					msg = "wxwebencrytoken expired or no exist";
					break;
				case 83007:
					msg = "wxwebencrytoken expired";
					break;
				case 83008:
					msg = "wxwebencrytoken no auth";
					break;
				case 83009:
					msg = "wxwebencrytoken not the mate with openid";
					break;
				case 83200:
					msg = "no exist service";
					break;
				case 83201:
					msg = "uin has not the service";
					break;
				case 83202:
					msg = "params is not json or not json array";
					break;
				case 83203:
					msg = "params num exceed 10";
					break;
				case 83204:
					msg = "object has not key";
					break;
				case 83205:
					msg = "key is not string";
					break;
				case 83206:
					msg = "object has not value";
					break;
				case 83207:
					msg = "value is not string";
					break;
				case 83208:
					msg = "key or value is empty";
					break;
				case 83209:
					msg = "key exist repeated";
					break;
				case 84001:
					msg = "invalid identify id";
					break;
				case 84002:
					msg = "user data expired";
					break;
				case 84003:
					msg = "user data not exist";
					break;
				case 84004:
					msg = "video upload fail!";
					break;
				case 84005:
					msg = "video download fail! please try again";
					break;
				case 84006:
					msg = "name or id_card_number empty";
					break;
				case 85001:
					msg = "微信号不存在或微信号设置为不可搜索";
					break;
				case 85002:
					msg = "小程序绑定的体验者数量达到上限";
					break;
				case 85003:
					msg = "微信号绑定的小程序体验者达到上限";
					break;
				case 85004:
					msg = "微信号已经绑定";
					break;
				case 85005:
					msg = "appid not bind weapp";
					break;
				case 85006:
					msg = "标签格式错误";
					break;
				case 85007:
					msg = "页面路径错误";
					break;
				case 85008:
					msg = "当前小程序没有已经审核通过的类目，请添加类目成功后重试";
					break;
				case 85009:
					msg = "已经有正在审核的版本";
					break;
				case 85010:
					msg = "item_list 有项目为空";
					break;
				case 85011:
					msg = "标题填写错误";
					break;
				case 85012:
					msg = "无效的审核 id";
					break;
				case 85013:
					msg = "无效的自定义配置";
					break;
				case 85014:
					msg = "无效的模板编号";
					break;
				case 85015:
					msg = "该账号不是小程序账号或版本输入错误";
					break;
				case 85016:
					msg = "域名数量超过限制 ，总数不能超过1000";
					break;
				case 85017:
					msg = "没有新增域名，请确认小程序已经添加了域名或该域名是否没有在第三方平台添加";
					break;
				case 85018:
					msg = "域名没有在第三方平台设置";
					break;
				case 85019:
					msg = "没有审核版本";
					break;
				case 85020:
					msg = "审核状态未满足发布";
					break;
				case 85021:
					msg = "status not allowed";
					break;
				case 85022:
					msg = "invalid action";
					break;
				case 85023:
					msg = "审核列表填写的项目数不在 1-5 以内";
					break;
				case 85024:
					msg = "need complete material";
					break;
				case 85025:
					msg = "this phone reach bind limit";
					break;
				case 85026:
					msg = "this wechat account reach bind limit";
					break;
				case 85027:
					msg = "this idcard reach bind limit";
					break;
				case 85028:
					msg = "this contractor reach bind limit";
					break;
				case 85029:
					msg = "nickname has used";
					break;
				case 85030:
					msg = "invalid nickname size(4-30)";
					break;
				case 85031:
					msg = "nickname is forbidden";
					break;
				case 85032:
					msg = "nickname is complained";
					break;
				case 85033:
					msg = "nickname is illegal";
					break;
				case 85034:
					msg = "nickname is protected";
					break;
				case 85035:
					msg = "nickname is forbidden for different contractor";
					break;
				case 85036:
					msg = "introduction is illegal";
					break;
				case 85038:
					msg = "store has added";
					break;
				case 85039:
					msg = "store has added by others";
					break;
				case 85040:
					msg = "store has added by yourseld";
					break;
				case 85041:
					msg = "credential has used";
					break;
				case 85042:
					msg = "nearby reach limit";
					break;
				case 85043:
					msg = "模板错误";
					break;
				case 85044:
					msg = "代码包超过大小限制";
					break;
				case 85045:
					msg = "ext_json 有不存在的路径";
					break;
				case 85046:
					msg = "tabBar 中缺少 path";
					break;
				case 85047:
					msg = "pages 字段为空";
					break;
				case 85048:
					msg = "ext_json 解析失败";
					break;
				case 85049:
					msg = "reach headimg or introduction quota limit";
					break;
				case 85050:
					msg = "verifying, don't apply again";
					break;
				case 85051:
					msg = "version_desc或者preview_info超限";
					break;
				case 85052:
					msg = "app is already released";
					break;
				case 85053:
					msg = "please apply merchant first";
					break;
				case 85054:
					msg = "poi_id is null, please upgrade first";
					break;
				case 85055:
					msg = "map_poi_id is invalid";
					break;
				case 85056:
					msg = "mediaid is invalid";
					break;
				case 85057:
					msg = "invalid widget data format";
					break;
				case 85058:
					msg = "no valid audit_id exist";
					break;
				case 85059:
					msg = "overseas access deny";
					break;
				case 85060:
					msg = "invalid taskid";
					break;
				case 85061:
					msg = "this phone reach bind limit";
					break;
				case 85062:
					msg = "this phone in black list";
					break;
				case 85063:
					msg = "idcard in black list";
					break;
				case 85064:
					msg = "找不到模板";
					break;
				case 85065:
					msg = "模板库已满";
					break;
				case 85066:
					msg = "链接错误";
					break;
				case 85067:
					msg = "input data error";
					break;
				case 85068:
					msg = "测试链接不是子链接";
					break;
				case 85069:
					msg = "校验文件失败";
					break;
				case 85070:
					msg = "个人类型小程序无法设置二维码规则";
					break;
				case 85071:
					msg = "已添加该链接，请勿重复添加";
					break;
				case 85072:
					msg = "该链接已被占用";
					break;
				case 85073:
					msg = "二维码规则已满";
					break;
				case 85074:
					msg = "小程序未发布, 小程序必须先发布代码才可以发布二维码跳转规则";
					break;
				case 85075:
					msg = "个人类型小程序无法设置二维码规则";
					break;
				case 85077:
					msg = "小程序类目信息失效（类目中含有官方下架的类目，请重新选择类目）";
					break;
				case 85078:
					msg = "operator info error";
					break;
				case 85079:
					msg = "小程序没有线上版本，不能进行灰度";
					break;
				case 85080:
					msg = "小程序提交的审核未审核通过";
					break;
				case 85081:
					msg = "无效的发布比例";
					break;
				case 85082:
					msg = "当前的发布比例需要比之前设置的高";
					break;
				case 85083:
					msg = "搜索标记位被封禁，无法修改";
					break;
				case 85084:
					msg = "非法的 status 值，只能填 0 或者 1";
					break;
				case 85085:
					msg = "小程序提审数量已达本月上限，请点击查看《临时quota申请流程》";
					break;
				case 85086:
					msg = "提交代码审核之前需提前上传代码";
					break;
				case 85087:
					msg = "小程序已使用 api navigateToMiniProgram，请声明跳转 appid 列表后再次提交";
					break;
				case 85088:
					msg = "no qbase privilege";
					break;
				case 85089:
					msg = "config not found";
					break;
				case 85090:
					msg = "wait and commit for this exappid later";
					break;
				case 85091:
					msg = "小程序的搜索开关被关闭。请访问设置页面打开开关再重试";
					break;
				case 85092:
					msg = "preview_info格式错误";
					break;
				case 85093:
					msg = "preview_info 视频或者图片个数超限";
					break;
				case 85094:
					msg = "需提供审核机制说明信息";
					break;
				case 85101:
					msg = "小程序不能发送该运动类型或运动类型不存在";
					break;
				case 85102:
					msg = "数值异常";
					break;
				case 86000:
					msg = "不是由第三方代小程序进行调用";
					break;
				case 86001:
					msg = "不存在第三方的已经提交的代码";
					break;
				case 86002:
					msg = "小程序还未设置昵称、头像、简介。请先设置完后再重新提交";
					break;
				case 86003:
					msg = "component do not has category mall";
					break;
				case 86004:
					msg = "invalid wechat";
					break;
				case 86005:
					msg = "wechat limit frequency";
					break;
				case 86006:
					msg = "has no quota to send group msg";
					break;
				case 86007:
					msg = "小程序禁止提交";
					break;
				case 86008:
					msg = "服务商被处罚，限制全部代码提审能力";
					break;
				case 86009:
					msg = "服务商新增小程序代码提审能力被限制";
					break;
				case 86010:
					msg = "服务商迭代小程序代码提审能力被限制";
					break;
				case 87006:
					msg = "小游戏不能提交";
					break;
				case 87007:
					msg = "session_key is not existd or expired";
					break;
				case 87008:
					msg = "invalid sig_method";
					break;
				case 87009:
					msg = "无效的签名";
					break;
				case 87010:
					msg = "invalid buffer size";
					break;
				case 87011:
					msg = "现网已经在灰度发布，不能进行版本回退";
					break;
				case 87012:
					msg = "该版本不能回退，可能的原因：1:无上一个线上版用于回退 2:此版本为已回退版本，不能回退 3:此版本为回退功能上线之前的版本，不能回退";
					break;
				case 87013:
					msg = "撤回次数达到上限（每天5次，每个月 10 次）";
					break;
				case 87014:
					msg = "risky content";
					break;
				case 87015:
					msg = "query timeout, try a content with less size";
					break;
				case 87016:
					msg = "some key-value in list meet length exceed";
					break;
				case 87017:
					msg = "user storage size exceed, delete some keys and try again";
					break;
				case 87018:
					msg = "user has stored too much keys. delete some keys and try again";
					break;
				case 87019:
					msg = "some keys in list meet length exceed";
					break;
				case 87080:
					msg = "need friend";
					break;
				case 87081:
					msg = "invalid openid";
					break;
				case 87082:
					msg = "invalid key";
					break;
				case 87083:
					msg = "invalid operation";
					break;
				case 87084:
					msg = "invalid opnum";
					break;
				case 87085:
					msg = "check fail";
					break;
				case 88000:
					msg = "without comment privilege";
					break;
				case 88001:
					msg = "msg_data is not exists";
					break;
				case 88002:
					msg = "the article is limit for safety";
					break;
				case 88003:
					msg = "elected comment upper limit";
					break;
				case 88004:
					msg = "comment was deleted by user";
					break;
				case 88005:
					msg = "already reply";
					break;
				case 88007:
					msg = "reply content beyond max len or content len is zero";
					break;
				case 88008:
					msg = "comment is not exists";
					break;
				case 88009:
					msg = "reply is not exists";
					break;
				case 88010:
					msg = "count range error. cout <= 0 or count > 50";
					break;
				case 88011:
					msg = "the article is limit for safety";
					break;
				case 89000:
					msg = "account has bound open，该公众号/小程序已经绑定了开放平台帐号";
					break;
				case 89001:
					msg = "not same contractor，Authorizer 与开放平台帐号主体不相同";
					break;
				case 89002:
					msg = "open not exists，该公众号/小程序未绑定微信开放平台帐号。";
					break;
				case 89003:
					msg = "该开放平台帐号并非通过 api 创建，不允许操作";
					break;
				case 89004:
					msg = "该开放平台帐号所绑定的公众号/小程序已达上限（100 个）";
					break;
				case 89005:
					msg = "without add video ability, the ability was banned";
					break;
				case 89006:
					msg = "without upload video ability, the ability was banned";
					break;
				case 89007:
					msg = "wxa quota limit";
					break;
				case 89008:
					msg = "overseas account can not link";
					break;
				case 89009:
					msg = "wxa reach link limit";
					break;
				case 89010:
					msg = "link message has sent";
					break;
				case 89011:
					msg = "can not unlink nearby wxa";
					break;
				case 89012:
					msg = "can not unlink store or mall";
					break;
				case 89013:
					msg = "wxa is banned";
					break;
				case 89014:
					msg = "support version error";
					break;
				case 89015:
					msg = "has linked wxa";
					break;
				case 89016:
					msg = "reach same realname quota";
					break;
				case 89017:
					msg = "reach different realname quota";
					break;
				case 89018:
					msg = "unlink message has sent";
					break;
				case 89019:
					msg = "业务域名无更改，无需重复设置";
					break;
				case 89020:
					msg = "尚未设置小程序业务域名，请先在第三方平台中设置小程序业务域名后在调用本接口";
					break;
				case 89021:
					msg = "请求保存的域名不是第三方平台中已设置的小程序业务域名或子域名";
					break;
				case 89022:
					msg = "delete domain is not exist!";
					break;
				case 89029:
					msg = "业务域名数量超过限制，最多可以添加100个业务域名";
					break;
				case 89030:
					msg = "operation reach month limit";
					break;
				case 89031:
					msg = "user bind reach limit";
					break;
				case 89032:
					msg = "weapp bind members reach limit";
					break;
				case 89033:
					msg = "empty wx or openid";
					break;
				case 89034:
					msg = "userstr is invalid";
					break;
				case 89035:
					msg = "linking from mp";
					break;
				case 89231:
					msg = "个人小程序不支持调用 setwebviewdomain 接口";
					break;
				case 89235:
					msg = "hit black contractor";
					break;
				case 89236:
					msg = "该插件不能申请";
					break;
				case 89237:
					msg = "已经添加该插件";
					break;
				case 89238:
					msg = "申请或使用的插件已经达到上限";
					break;
				case 89239:
					msg = "该插件不存在";
					break;
				case 89240:
					msg = "only applying status can be agreed or refused";
					break;
				case 89241:
					msg = "only refused status can be deleted, please refused first";
					break;
				case 89242:
					msg = "appid is no in the apply list, make sure appid is right";
					break;
				case 89243:
					msg = "该申请为“待确认”状态，不可删除";
					break;
				case 89244:
					msg = "不存在该插件 appid";
					break;
				case 89245:
					msg = "mini program forbidden to link";
					break;
				case 89246:
					msg = "plugins with special category are used only by specific apps";
					break;
				case 89247:
					msg = "系统内部错误";
					break;
				case 89248:
					msg = "invalid code type";
					break;
				case 89249:
					msg = "task running";
					break;
				case 89250:
					msg = "内部错误";
					break;
				case 89251:
					msg = "模板消息已下发，待法人人脸核身校验";
					break;
				case 89253:
					msg = "法人&企业信息一致性校验中";
					break;
				case 89254:
					msg = "lack of some component rights";
					break;
				case 89255:
					msg = "code参数无效，请检查code长度以及内容是否正确；注意code_type的值不同需要传的code长度不一样";
					break;
				case 89256:
					msg = "token 信息有误";
					break;
				case 89257:
					msg = "该插件版本不支持快速更新";
					break;
				case 89258:
					msg = "当前小程序帐号存在灰度发布中的版本，不可操作快速更新";
					break;
				case 89259:
					msg = "zhibo plugin is not allow to delete";
					break;
				case 89300:
					msg = "订单无效";
					break;
				case 89401:
					msg = "系统不稳定，请稍后再试，如多次失败请通过社区反馈";
					break;
				case 89402:
					msg = "该小程序不在待审核队列，请检查是否已提交审核或已审完";
					break;
				case 89403:
					msg = "本单属于平台不支持加急种类，请等待正常审核流程";
					break;
				case 89404:
					msg = "本单已加速成功，请勿重复提交";
					break;
				case 89405:
					msg = "本月加急额度已用完，请提高提审质量以获取更多额度";
					break;
				case 89501:
					msg = "公众号有未处理的确认请求，请稍候重试";
					break;
				case 89502:
					msg = "请耐心等待管理员确认";
					break;
				case 89503:
					msg = "此次调用需要管理员确认，请耐心等候";
					break;
				case 89504:
					msg = "正在等管理员确认，请耐心等待";
					break;
				case 89505:
					msg = "正在等管理员确认，请稍候重试";
					break;
				case 89506:
					msg = "该IP调用求请求已被公众号管理员拒绝，请24小时后再试，建议调用前与管理员沟通确认";
					break;
				case 89507:
					msg = "该IP调用求请求已被公众号管理员拒绝，请1小时后再试，建议调用前与管理员沟通确认";
					break;
				case 90001:
					msg = "invalid order id";
					break;
				case 90002:
					msg = "invalid busi id";
					break;
				case 90003:
					msg = "invalid bill date";
					break;
				case 90004:
					msg = "invalid bill type";
					break;
				case 90005:
					msg = "invalid platform";
					break;
				case 90006:
					msg = "bill not exists";
					break;
				case 90007:
					msg = "invalid openid";
					break;
				case 90009:
					msg = "mp_sig error";
					break;
				case 90010:
					msg = "no session";
					break;
				case 90011:
					msg = "sig error";
					break;
				case 90012:
					msg = "order exist";
					break;
				case 90013:
					msg = "balance not enough";
					break;
				case 90014:
					msg = "order has been confirmed";
					break;
				case 90015:
					msg = "order has been canceled";
					break;
				case 90016:
					msg = "order is being processed";
					break;
				case 90017:
					msg = "no privilege";
					break;
				case 90018:
					msg = "invalid parameter";
					break;
				case 91001:
					msg = "不是公众号快速创建的小程序";
					break;
				case 91002:
					msg = "小程序发布后不可改名";
					break;
				case 91003:
					msg = "改名状态不合法";
					break;
				case 91004:
					msg = "昵称不合法";
					break;
				case 91005:
					msg = "昵称 15 天主体保护";
					break;
				case 91006:
					msg = "昵称命中微信号";
					break;
				case 91007:
					msg = "昵称已被占用";
					break;
				case 91008:
					msg = "昵称命中 7 天侵权保护期";
					break;
				case 91009:
					msg = "需要提交材料";
					break;
				case 91010:
					msg = "其他错误";
					break;
				case 91011:
					msg = "查不到昵称修改审核单信息";
					break;
				case 91012:
					msg = "其它错误";
					break;
				case 91013:
					msg = "占用名字过多";
					break;
				case 91014:
					msg = "+号规则 同一类型关联名主体不一致";
					break;
				case 91015:
					msg = "原始名不同类型主体不一致";
					break;
				case 91016:
					msg = "名称占用者 ≥2";
					break;
				case 91017:
					msg = "+号规则 不同类型关联名主体不一致";
					break;
				case 91018:
					msg = "组织类型小程序发布后，侵权被清空昵称，需走认证改名";
					break;
				case 91019:
					msg = "小程序正在审核中";
					break;
				case 92000:
					msg = "该经营资质已添加，请勿重复添加";
					break;
				case 92002:
					msg = "附近地点添加数量达到上线，无法继续添加";
					break;
				case 92003:
					msg = "地点已被其它小程序占用";
					break;
				case 92004:
					msg = "附近功能被封禁";
					break;
				case 92005:
					msg = "地点正在审核中";
					break;
				case 92006:
					msg = "地点正在展示小程序";
					break;
				case 92007:
					msg = "地点审核失败";
					break;
				case 92008:
					msg = "小程序未展示在该地点";
					break;
				case 93009:
					msg = "小程序未上架或不可见";
					break;
				case 93010:
					msg = "地点不存在";
					break;
				case 93011:
					msg = "个人类型小程序不可用";
					break;
				case 93012:
					msg = "非普通类型小程序（门店小程序、小店小程序等）不可用";
					break;
				case 93013:
					msg = "从腾讯地图获取地址详细信息失败";
					break;
				case 93014:
					msg = "同一资质证件号重复添加";
					break;
				case 93015:
					msg = "附近类目审核中";
					break;
				case 93016:
					msg = "服务标签个数超限制（官方最多5个，自定义最多4个）";
					break;
				case 93017:
					msg = "服务标签或者客服的名字不符合要求";
					break;
				case 93018:
					msg = "服务能力中填写的小程序appid不是同主体小程序";
					break;
				case 93019:
					msg = "申请类目之后才能添加附近地点";
					break;
				case 93020:
					msg = "qualification_list无效";
					break;
				case 93021:
					msg = "company_name字段为空";
					break;
				case 93022:
					msg = "credential字段为空";
					break;
				case 93023:
					msg = "address字段为空";
					break;
				case 93024:
					msg = "qualification_list字段为空";
					break;
				case 93025:
					msg = "服务appid对应的path不存在";
					break;
				case 94001:
					msg = "missing cert_serialno";
					break;
				case 94002:
					msg = "use not register wechat pay";
					break;
				case 94003:
					msg = "invalid sign";
					break;
				case 94004:
					msg = "user do not has real name info";
					break;
				case 94005:
					msg = "invalid user token";
					break;
				case 94006:
					msg = "appid unauthorized";
					break;
				case 94007:
					msg = "appid unbind mchid";
					break;
				case 94008:
					msg = "invalid timestamp";
					break;
				case 94009:
					msg = "invalid cert_serialno, cert_serialno's size should be 40";
					break;
				case 94010:
					msg = "invalid mch_id";
					break;
				case 94011:
					msg = "timestamp expired";
					break;
				case 94012:
					msg = "appid和商户号的绑定关系不存在";
					break;
				case 95001:
					msg = "wxcode decode fail";
					break;
				case 95002:
					msg = "wxcode recognize unautuorized";
					break;
				case 95101:
					msg = "get product by page args invalid";
					break;
				case 95102:
					msg = "get product materials by cond args invalid";
					break;
				case 95103:
					msg = "material id list size out of limit";
					break;
				case 95104:
					msg = "import product frequence out of limit";
					break;
				case 95105:
					msg = "mp is importing products, api is rejected to import";
					break;
				case 95106:
					msg = "api is rejected to import, need to set commission ratio on mp first";
					break;
				case 200002:
					msg = "入参错误";
					//msg = "服务市场购买的服务过期";
					break;
				case 200011:
					msg = "此账号已被封禁，无法操作";
					break;
				case 200012:
					msg = "个人模板数已达上限，上限25个";
					break;
				case 200013:
					msg = "此模板已被封禁，无法选用";
					break;
				case 200014:
					msg = "模板 tid 参数错误";
					break;
				case 200016:
					msg = "start 参数错误";
					break;
				case 200017:
					msg = "limit 参数错误";
					break;
				case 200018:
					msg = "类目 ids 缺失";
					break;
				case 200019:
					msg = "类目 ids 不合法";
					break;
				case 200020:
					msg = "关键词列表 kidList 参数错误";
					break;
				case 200021:
					msg = "场景描述 sceneDesc 参数错误";
					break;
				case 300001:
					msg = "禁止创建/更新商品（如商品创建功能被封禁） 或 禁止编辑&更新房间";
					break;
				case 300002:
					msg = "名称长度不符合规则";
					break;
				case 300003:
					msg = "价格输入不合规（如现价比原价大、传入价格非数字等）";
					break;
				case 300004:
					msg = "商品名称存在违规违法内容";
					break;
				case 300005:
					msg = "商品图片存在违规违法内容";
					break;
				case 300006:
					msg = "图片上传失败（如mediaID过期）";
					break;
				case 300007:
					msg = "线上小程序版本不存在该链接";
					break;
				case 300008:
					msg = "添加商品失败";
					break;
				case 300009:
					msg = "商品审核撤回失败";
					break;
				case 300010:
					msg = "商品审核状态不对（如商品审核中）";
					break;
				case 300011:
					msg = "操作非法（API不允许操作非API创建的商品）";
					break;
				case 300012:
					msg = "没有提审额度（每天500次提审额度）";
					break;
				case 300013:
					msg = "提审失败";
					break;
				case 300014:
					msg = "审核中，无法删除（非零代表失败）";
					break;
				case 300017:
					msg = "商品未提审";
					break;
				case 300021:
					msg = "商品添加成功，审核失败";
					break;
				case 300022:
					msg = "此房间号不存在";
					break;
				case 300023:
					msg = "房间状态 拦截（当前房间状态不允许此操作）";
					break;
				case 300024:
					msg = "商品不存在";
					break;
				case 300025:
					msg = "商品审核未通过";
					break;
				case 300026:
					msg = "房间商品数量已经满额";
					break;
				case 300027:
					msg = "导入商品失败";
					break;
				case 300028:
					msg = "房间名称违规";
					break;
				case 300029:
					msg = "主播昵称违规";
					break;
				case 300030:
					msg = "主播微信号不合法";
					break;
				case 300031:
					msg = "直播间封面图不合规";
					break;
				case 300032:
					msg = "直播间分享图违规";
					break;
				case 300033:
					msg = "添加商品超过直播间上限";
					break;
				case 300034:
					msg = "主播微信昵称长度不符合要求";
					break;
				case 300035:
					msg = "主播微信号不存在";
					break;
				case 300036:
					msg = "主播微信号未实名认证";
					break;
				case 600001:
					msg = "invalid file name";
					break;
				case 600002:
					msg = "no permission to upload file";
					break;
				case 600003:
					msg = "invalid size of source";
					break;
				case 928000:
					msg = "票据已存在";
					break;
				case 928001:
					msg = "票据不存在";
					break;
				case 930555:
					msg = "sysem error";
					break;
				case 930556:
					msg = "delivery timeout";
					break;
				case 930557:
					msg = "delivery system error";
					break;
				case 930558:
					msg = "delivery logic error";
					break;
				case 930559:
					msg = "沙盒环境openid无效";
					break;
				case 930560:
					msg = "shopid need bind first";
					break;
				case 930561:
					msg = "参数错误";
					break;
				case 930562:
					msg = "order already exists";
					break;
				case 930563:
					msg = "订单不存在";
					break;
				case 930564:
					msg = "沙盒环境调用无配额";
					break;
				case 930565:
					msg = "order finished";
					break;
				case 930566:
					msg = "not support, plz auth at mp.weixin.qq.com";
					break;
				case 930567:
					msg = "shop arg error";
					break;
				case 930568:
					msg = "不支持个人类型小程序";
					break;
				case 930569:
					msg = "已经开通不需要再开通";
					break;
				case 930570:
					msg = "cargo_first_class or cargo_second_class invalid";
					break;
				case 930571:
					msg = "该商户没有内测权限，请先申请权限: https://wj.qq.com/s2/7243532/fcfb/";
					break;
				case 931010:
					msg = "fee already set";
					break;
				case 6000100:
					msg = "unbind download url";
					break;
				case 6000101:
					msg = "no response data";
					break;
				case 6000102:
					msg = "response data too big";
					break;
				case 9001001:
					msg = "POST 数据参数不合法";
					break;
				case 9001002:
					msg = "远端服务不可用";
					break;
				case 9001003:
					msg = "Ticket 不合法";
					break;
				case 9001004:
					msg = "获取摇周边用户信息失败";
					break;
				case 9001005:
					msg = "获取商户信息失败";
					break;
				case 9001006:
					msg = "获取 OpenID 失败";
					break;
				case 9001007:
					msg = "上传文件缺失";
					break;
				case 9001008:
					msg = "上传素材的文件类型不合法";
					break;
				case 9001009:
					msg = "上传素材的文件尺寸不合法";
					break;
				case 9001010:
					msg = "上传失败";
					break;
				case 9001020:
					msg = "帐号不合法";
					break;
				case 9001021:
					msg = "已有设备激活率低于 50% ，不能新增设备";
					break;
				case 9001022:
					msg = "设备申请数不合法，必须为大于 0 的数字";
					break;
				case 9001023:
					msg = "已存在审核中的设备 ID 申请";
					break;
				case 9001024:
					msg = "一次查询设备 ID 数量不能超过 50";
					break;
				case 9001025:
					msg = "设备 ID 不合法";
					break;
				case 9001026:
					msg = "页面 ID 不合法";
					break;
				case 9001027:
					msg = "页面参数不合法";
					break;
				case 9001028:
					msg = "一次删除页面 ID 数量不能超过 10";
					break;
				case 9001029:
					msg = "页面已应用在设备中，请先解除应用关系再删除";
					break;
				case 9001030:
					msg = "一次查询页面 ID 数量不能超过 50";
					break;
				case 9001031:
					msg = "时间区间不合法";
					break;
				case 9001032:
					msg = "保存设备与页面的绑定关系参数错误";
					break;
				case 9001033:
					msg = "门店 ID 不合法";
					break;
				case 9001034:
					msg = "设备备注信息过长";
					break;
				case 9001035:
					msg = "设备申请参数不合法";
					break;
				case 9001036:
					msg = "查询起始值 begin 不合法";
					break;
				case 9002008:
					msg = "params invalid";
					break;
				case 9002009:
					msg = "shop id not exist";
					break;
				case 9002010:
					msg = "ssid or password should start with “WX”";
					break;
				case 9002011:
					msg = "ssid can not include chinese";
					break;
				case 9002012:
					msg = "passsword can not include chinese";
					break;
				case 9002013:
					msg = "password must be between 8 and 24 characters";
					break;
				case 9002016:
					msg = "device exist";
					break;
				case 9002017:
					msg = "device not exist";
					break;
				case 9002026:
					msg = "the size of query list reach limit";
					break;
				case 9002041:
					msg = "not allowed to modify, ensure you are an certified or component account";
					break;
				case 9002044:
					msg = "invalid ssid, can not include none utf8 characters, and should be between 1 and 32 bytes";
					break;
				case 9002052:
					msg = "shop id has not be audited, this bar type is not enable";
					break;
				case 9007003:
					msg = "protocol type is not same with the exist device";
					break;
				case 9007004:
					msg = "ssid not exist";
					break;
				case 9007005:
					msg = "device count limit";
					break;
				case 9008001:
					msg = "card info not exist";
					break;
				case 9008002:
					msg = "card expiration time is invalid";
					break;
				case 9008003:
					msg = "url size invalid, keep less than 255";
					break;
				case 9008004:
					msg = "url can not include chinese";
					break;
				case 9200001:
					msg = "order_id not exist";
					break;
				case 9200002:
					msg = "order of other biz";
					break;
				case 9200003:
					msg = "blocked";
					break;
				case 9200211:
					msg = "payment notice disabled";
					break;
				case 9200231:
					msg = "payment notice not exist";
					break;
				case 9200232:
					msg = "payment notice paid";
					break;
				case 9200233:
					msg = "payment notice canceled";
					break;
				case 9200235:
					msg = "payment notice expired";
					break;
				case 9200236:
					msg = "bank not allow";
					break;
				case 9200295:
					msg = "freq limit";
					break;
				case 9200297:
					msg = "suspend payment at current time";
					break;
				case 9200298:
					msg = "3rd resp decrypt error";
					break;
				case 9200299:
					msg = "3rd resp system error";
					break;
				case 9200300:
					msg = "3rd resp sign error";
					break;
				case 9201000:
					msg = "desc empty";
					break;
				case 9201001:
					msg = "fee not equal items'";
					break;
				case 9201002:
					msg = "payment info incorrect";
					break;
				case 9201003:
					msg = "fee is 0";
					break;
				case 9201004:
					msg = "payment expire date format error";
					break;
				case 9201005:
					msg = "appid error";
					break;
				case 9201006:
					msg = "payment order id error";
					break;
				case 9201007:
					msg = "openid error";
					break;
				case 9201008:
					msg = "return_url error";
					break;
				case 9201009:
					msg = "ip error";
					break;
				case 9201010:
					msg = "order_id error";
					break;
				case 9201011:
					msg = "reason error";
					break;
				case 9201012:
					msg = "mch_id error";
					break;
				case 9201013:
					msg = "bill_date error";
					break;
				case 9201014:
					msg = "bill_type error";
					break;
				case 9201015:
					msg = "trade_type error";
					break;
				case 9201016:
					msg = "bank_id error";
					break;
				case 9201017:
					msg = "bank_account error";
					break;
				case 9201018:
					msg = "payment_notice_no error";
					break;
				case 9201019:
					msg = "department_code error";
					break;
				case 9201020:
					msg = "payment_notice_type error";
					break;
				case 9201021:
					msg = "region_code error";
					break;
				case 9201022:
					msg = "department_name error";
					break;
				case 9201023:
					msg = "fee not equal finance's";
					break;
				case 9201024:
					msg = "refund_out_id error";
					break;
				case 9201026:
					msg = "not combined order_id";
					break;
				case 9201027:
					msg = "partial sub order is test";
					break;
				case 9201029:
					msg = "desc too long";
					break;
				case 9201031:
					msg = "sub order list size error";
					break;
				case 9201032:
					msg = "sub order repeated";
					break;
				case 9201033:
					msg = "auth_code empty";
					break;
				case 9201034:
					msg = "bank_id empty but mch_id not empty";
					break;
				case 9201035:
					msg = "sum of other fees exceed total fee";
					break;
				case 9202000:
					msg = "other user paying";
					break;
				case 9202001:
					msg = "pay process not finish";
					break;
				case 9202002:
					msg = "no refund permission";
					break;
				case 9202003:
					msg = "ip limit";
					break;
				case 9202004:
					msg = "freq limit";
					break;
				case 9202005:
					msg = "user weixin account abnormal";
					break;
				case 9202006:
					msg = "account balance not enough";
					break;
				case 9202010:
					msg = "refund request repeated";
					break;
				case 9202011:
					msg = "has refunded";
					break;
				case 9202012:
					msg = "refund exceed total fee";
					break;
				case 9202013:
					msg = "busi_id dup";
					break;
				case 9202016:
					msg = "not check sign";
					break;
				case 9202017:
					msg = "check sign failed";
					break;
				case 9202018:
					msg = "sub order error";
					break;
				case 9202020:
					msg = "order status error";
					break;
				case 9202021:
					msg = "unified order repeatedly";
					break;
				case 9203000:
					msg = "request to notification url fail";
					break;
				case 9203001:
					msg = "http request fail";
					break;
				case 9203002:
					msg = "http response data error";
					break;
				case 9203003:
					msg = "http response data RSA decrypt fail";
					break;
				case 9203004:
					msg = "http response data AES decrypt fail";
					break;
				case 9203999:
					msg = "system busy, please try again later";
					break;
				case 9204000:
					msg = "getrealname token error";
					break;
				case 9204001:
					msg = "getrealname user or token error";
					break;
				case 9204002:
					msg = "getrealname appid or token error";
					break;
				case 9205000:
					msg = "finance conf not exist";
					break;
				case 9205001:
					msg = "bank conf not exist";
					break;
				case 9205002:
					msg = "wei ban ju conf not exist";
					break;
				case 9205010:
					msg = "symmetric key conf not exist";
					break;
				case 9205101:
					msg = "out order id not exist";
					break;
				case 9205201:
					msg = "bill not exist";
					break;
				case 9206000:
					msg = "3rd resp pay_channel empty";
					break;
				case 9206001:
					msg = "3rd resp order_id empty";
					break;
				case 9206002:
					msg = "3rd resp bill_type_code empty";
					break;
				case 9206003:
					msg = "3rd resp bill_no empty";
					break;
				case 9206200:
					msg = "3rd resp empty";
					break;
				case 9206201:
					msg = "3rd resp not json";
					break;
				case 9206900:
					msg = "connect 3rd error";
					break;
				case 9206901:
					msg = "connect 3rd timeout";
					break;
				case 9206910:
					msg = "read 3rd resp error";
					break;
				case 9206911:
					msg = "read 3rd resp timeout";
					break;
				case 9207000:
					msg = "boss error";
					break;
				case 9207001:
					msg = "wechat pay error";
					break;
				case 9207002:
					msg = "boss param error";
					break;
				case 9207003:
					msg = "pay error";
					break;
				case 9207004:
					msg = "auth_code expired";
					break;
				case 9207005:
					msg = "user balance not enough";
					break;
				case 9207006:
					msg = "card not support";
					break;
				case 9207007:
					msg = "order reversed";
					break;
				case 9207008:
					msg = "user paying, need input password";
					break;
				case 9207009:
					msg = "auth_code error";
					break;
				case 9207010:
					msg = "auth_code invalid";
					break;
				case 9207011:
					msg = "not allow to reverse when user paying";
					break;
				case 9207012:
					msg = "order paid";
					break;
				case 9207013:
					msg = "order closed";
					break;
				case 9207028:
					msg = "vehicle not exists";
					break;
				case 9207029:
					msg = "vehicle request blocked";
					break;
				case 9207030:
					msg = "vehicle auth error";
					break;
				case 9207031:
					msg = "contract over limit";
					break;
				case 9207032:
					msg = "trade error";
					break;
				case 9207033:
					msg = "trade time invalid";
					break;
				case 9207034:
					msg = "channel type invalid";
					break;
				case 9207050:
					msg = "expire_time range error";
					break;
				case 9210000:
					msg = "query finance error";
					break;
				case 9291000:
					msg = "openid error";
					break;
				case 9291001:
					msg = "openid appid not match";
					break;
				case 9291002:
					msg = "app_appid not exist";
					break;
				case 9291003:
					msg = "app_appid not app";
					break;
				case 9291004:
					msg = "appid empty";
					break;
				case 9291005:
					msg = "appid not match access_token";
					break;
				case 9291006:
					msg = "invalid sign";
					break;
				case 9299999:
					msg = "backend logic error";
					break;
				case 9300001:
					msg = "begin_time can not before now";
					break;
				case 9300002:
					msg = "end_time can not before now";
					break;
				case 9300003:
					msg = "begin_time must less than end_time";
					break;
				case 9300004:
					msg = "end_time - begin_time > 1year";
					break;
				case 9300005:
					msg = "invalid max_partic_times";
					break;
				case 9300006:
					msg = "invalid activity status";
					break;
				case 9300007:
					msg = "gift_num must >0 and <=15";
					break;
				case 9300008:
					msg = "invalid tiny appid";
					break;
				case 9300009:
					msg = "activity can not finish";
					break;
				case 9300010:
					msg = "card_info_list must >= 2";
					break;
				case 9300011:
					msg = "invalid card_id";
					break;
				case 9300012:
					msg = "card_id must belong this appid";
					break;
				case 9300013:
					msg = "card_id is not swipe_card or pay.cash";
					break;
				case 9300014:
					msg = "some card_id is out of stock";
					break;
				case 9300015:
					msg = "some card_id is invalid status";
					break;
				case 9300016:
					msg = "membership or new/old tinyapp user only support one";
					break;
				case 9300017:
					msg = "invalid logic for membership";
					break;
				case 9300018:
					msg = "invalid logic for tinyapp new/old user";
					break;
				case 9300019:
					msg = "invalid activity type";
					break;
				case 9300020:
					msg = "invalid activity_id";
					break;
				case 9300021:
					msg = "invalid help_max_times";
					break;
				case 9300022:
					msg = "invalid cover_url";
					break;
				case 9300023:
					msg = "invalid gen_limit";
					break;
				case 9300024:
					msg = "card's end_time cannot early than act's end_time";
					break;
				case 9300501:
					msg = "快递侧逻辑错误，详细原因需要看 delivery_resultcode, 请先确认一下编码方式，python建议 json.dumps(b, ensure_ascii=False)，php建议 json_encode($arr, JSON_UNESCAPED_UNICODE)";
					break;
				case 9300502:
					msg = "快递公司系统错误";
					break;
				case 9300503:
					msg = "delivery_id 不存在";
					break;
				case 9300504:
					msg = "service_type 不存在";
					break;
				case 9300505:
					msg = "Shop banned";
					break;
				case 9300506:
					msg = "运单 ID 已经存在轨迹，不可取消";
					break;
				case 9300507:
					msg = "Token 不正确";
					break;
				case 9300508:
					msg = "order id has been used";
					break;
				case 9300509:
					msg = "speed limit, retry too fast";
					break;
				case 9300510:
					msg = "invalid service type";
					break;
				case 9300511:
					msg = "invalid branch id";
					break;
				case 9300512:
					msg = "模板格式错误，渲染失败";
					break;
				case 9300513:
					msg = "out of quota";
					break;
				case 9300514:
					msg = "add net branch fail, try update branch api";
					break;
				case 9300515:
					msg = "wxa appid not exist";
					break;
				case 9300516:
					msg = "wxa appid and current bizuin is not linked or not the same owner";
					break;
				case 9300517:
					msg = "update_type 不正确,请使用“bind” 或者“unbind”";
					break;
				case 9300520:
					msg = "invalid delivery id";
					break;
				case 9300521:
					msg = "the orderid is in our system, and waybill is generating";
					break;
				case 9300522:
					msg = "this orderid is repeated";
					break;
				case 9300523:
					msg = "quota is not enough; go to charge please";
					break;
				case 9300524:
					msg = "订单已取消（一般为重复取消订单）";
					break;
				case 9300525:
					msg = "bizid未绑定";
					break;
				case 9300526:
					msg = "参数字段长度不正确";
					break;
				case 9300527:
					msg = "delivery does not support quota";
					break;
				case 9300528:
					msg = "invalid waybill_id";
					break;
				case 9300529:
					msg = "账号已绑定过";
					break;
				case 9300530:
					msg = "解绑的biz_id不存在";
					break;
				case 9300531:
					msg = "bizid无效 或者密码错误";
					break;
				case 9300532:
					msg = "绑定已提交，审核中";
					break;
				case 9300533:
					msg = "invalid tagid_list";
					break;
				case 9300534:
					msg = "add_source=2时，wx_appid和当前小程序不同主体";
					break;
				case 9300535:
					msg = "shop字段商品缩略图 url、商品名称为空或者非法，或者商品数量为0";
					break;
				case 9300536:
					msg = "add_source=2时，wx_appid无效";
					break;
				case 9300537:
					msg = "freq limit";
					break;
				case 9300538:
					msg = "input task empty";
					break;
				case 9300539:
					msg = "too many task";
					break;
				case 9300540:
					msg = "task not exist";
					break;
				case 9300541:
					msg = "delivery callback error";
					break;
				case 9300601:
					msg = "id_card_no is invalid";
					break;
				case 9300602:
					msg = "name is invalid";
					break;
				case 9300603:
					msg = "plate_no is invalid";
					break;
				case 9300604:
					msg = "auth_key decode error";
					break;
				case 9300605:
					msg = "auth_key is expired";
					break;
				case 9300606:
					msg = "auth_key and appinfo not match";
					break;
				case 9300607:
					msg = "user not confirm";
					break;
				case 9300608:
					msg = "user confirm is expired";
					break;
				case 9300609:
					msg = "api exceed limit";
					break;
				case 9300610:
					msg = "car license info is invalid";
					break;
				case 9300611:
					msg = "varification type not support";
					break;
				case 9300701:
					msg = "input param error";
					break;
				case 9300702:
					msg = "this code has been used";
					break;
				case 9300703:
					msg = "invalid date";
					break;
				case 9300704:
					msg = "not currently available";
					break;
				case 9300705:
					msg = "code not exist or expired";
					break;
				case 9300706:
					msg = "code not exist or expired";
					break;
				case 9300707:
					msg = "wxpay error";
					break;
				case 9300708:
					msg = "wxpay overlimit";
					break;
				case 9300801:
					msg = "无效的微信号";
					break;
				case 9300802:
					msg = "服务号未开通导购功能";
					break;
				case 9300803:
					msg = "微信号已经绑定为导购";
					break;
				case 9300804:
					msg = "该微信号不是导购";
					break;
				case 9300805:
					msg = "微信号已经被其他账号绑定为导购";
					break;
				case 9300806:
					msg = "粉丝和导购不存在绑定关系";
					break;
				case 9300807:
					msg = "标签值无效，不是可选标签值";
					break;
				case 9300808:
					msg = "标签值不存在";
					break;
				case 9300809:
					msg = "展示标签值不存在";
					break;
				case 9300810:
					msg = "导购昵称太长，最多16个字符";
					break;
				case 9300811:
					msg = "只支持mmbiz.qpic.cn域名的图片";
					break;
				case 9300812:
					msg = "达到导购绑定个数限制";
					break;
				case 9300813:
					msg = "达到导购粉丝绑定个数限制";
					break;
				case 9300814:
					msg = "敏感词个数超过上限";
					break;
				case 9300815:
					msg = "快捷回复个数超过上限";
					break;
				case 9300816:
					msg = "文字素材个数超过上限";
					break;
				case 9300817:
					msg = "小程序卡片素材个数超过上限";
					break;
				case 9300818:
					msg = "图片素材个数超过上限";
					break;
				case 9300819:
					msg = "mediaid 有误";
					break;
				case 9300820:
					msg = "可查询标签类别超过上限";
					break;
				case 9300821:
					msg = "小程序卡片内appid不符合要求";
					break;
				case 9300822:
					msg = "标签类别的名字无效";
					break;
				case 9300823:
					msg = "查询聊天记录时间参数有误";
					break;
				case 9300824:
					msg = "自动回复字数太长";
					break;
				case 9300825:
					msg = "导购群组id错误";
					break;
				case 9300826:
					msg = "维护中";
					break;
				case 9301001:
					msg = "invalid parameter";
					break;
				case 9301002:
					msg = "call api service failed";
					break;
				case 9301003:
					msg = "internal exception";
					break;
				case 9301004:
					msg = "save data error";
					break;
				case 9301006:
					msg = "invalid appid";
					break;
				case 9301007:
					msg = "invalid api config";
					break;
				case 9301008:
					msg = "invalid api info";
					break;
				case 9301009:
					msg = "add result check failed";
					break;
				case 9301010:
					msg = "consumption failure";
					break;
				case 9301011:
					msg = "frequency limit reached";
					break;
				case 9301012:
					msg = "service timeout";
					break;
				case 9400001:
					msg = "该开发小程序已开通小程序直播权限，不支持发布版本。如需发版，请解绑开发小程序后再操作。";
					break;
				case 9401001:
					msg = "商品已存在";
					break;
				case 9401002:
					msg = "商品不存在";
					break;
				case 9401003:
					msg = "类目已存在";
					break;
				case 9401004:
					msg = "类目不存在";
					break;
				case 9401005:
					msg = "SKU已存在";
					break;
				case 9401006:
					msg = "SKU不存在";
					break;
				case 9401007:
					msg = "属性已存在";
					break;
				case 9401008:
					msg = "属性不存在";
					break;
				case 9401020:
					msg = "非法参数";
					break;
				case 9401021:
					msg = "没有商品权限";
					break;
				case 9401022:
					msg = "SPU NOT ALLOW";
					break;
				case 9401023:
					msg = "SPU_NOT_ALLOW_EDIT";
					break;
				case 9401024:
					msg = "SKU NOT ALLOW";
					break;
				case 9401025:
					msg = "SKU_NOT_ALLOW_EDIT";
					break;
				case 9402001:
					msg = "limit too large";
					break;
				case 9402002:
					msg = "single send been blocked";
					break;
				case 9402003:
					msg = "all send been blocked";
					break;
				case 9402004:
					msg = "invalid msg id";
					break;
				case 9402005:
					msg = "send msg too quick";
					break;
				case 9402006:
					msg = "send to single user too quick";
					break;
				case 9402007:
					msg = "send to all user too quick";
					break;
				case 9402008:
					msg = "send type error";
					break;
				case 9402009:
					msg = "can not send this msg";
					break;
				case 9402010:
					msg = "content too long or no content";
					break;
				case 9402011:
					msg = "path not exist";
					break;
				case 9402012:
					msg = "contain evil word";
					break;
				case 9402013:
					msg = "path need html suffix";
					break;
				case 9402014:
					msg = "not open to personal body type";
					break;
				case 9402015:
					msg = "not open to violation body type";
					break;
				case 9402016:
					msg = "not open to low quality provider";
					break;
				case 9402101:
					msg = "invalid product_id";
					break;
				case 9402102:
					msg = "device_id count more than limit";
					break;
				case 9402202:
					msg = "请勿频繁提交，待上一次操作完成后再提交";
					break;
				case 9402203:
					msg = "标准模板ext_json错误，传了不合法的参数， 如果是标准模板库的模板，则ext_json支持的参数仅为{\"extAppid\":'', \"ext\": {}, \"window\": {}}";
					break;
				case 9402301:
					msg = "user not book this ad id";
					break;
				case 9403000:
					msg = "消息类型错误!";
					break;
				case 9403001:
					msg = "消息字段的内容过长!";
					break;
				case 9403002:
					msg = "消息字段的内容违规!";
					break;
				case 9403003:
					msg = "发送的微信号太多!";
					break;
				case 9403004:
					msg = "存在错误的微信号!";
					break;
				case 9410000:
					msg = "直播间列表为空";
					break;
				case 9410001:
					msg = "获取房间失败";
					break;
				case 9410002:
					msg = "获取商品失败";
					break;
				case 9410003:
					msg = "获取回放失败";
					break;
				case 100000:
					msg = "操作无权限";
					break;
				case 100002:
					msg = "获取不存在";
					break;
				case 100003:
					msg = "更新不存在";
					break;
				case 100004:
					msg = "删除不存在";
					break;
				case 300000:
					msg = "超过频率限制（绝大多数接口为800 / 30s，后续可能会变动，请留意该文档）";
					break;
				case -30000:
					msg = "能执行该操作，请检查审核";
					break;
				case 100009: msg = "添加SPU后再添加SKU失败"; break;
				case 100010: msg = "类目和品牌不匹配"; break;
				case 108002: msg = "商品状态异常"; break;
				case 108003: msg = "锁定库存失败"; break;
				case 108004: msg = "提交订单后的商品价格有差异"; break;
				case 108005: msg = "订单支付还没过期"; break;
				case 108006: msg = "订单支付过期"; break;
				case 108007: msg = "自动确认收货还没过期"; break;
				case 108008: msg = "订单里面的售后单商品不对"; break;
				case 108009: msg = "订单在售后中，不允许发货，确认收货"; break;
				case 108010: msg = "翻页,page_num 错误，"; break;
				case 108011: msg = "订单中有sku价格变了"; break;
				case 108012: msg = "运费有变化"; break;
				case 108013: msg = "订单金额有变化"; break;
				case 108014: msg = "订单中所选优惠券不能用"; break;
				case 108015: msg = "订单事务失败"; break;
				case 108016: msg = "库存流水状态不对"; break;
				case 108017: msg = "抢购库存不足"; break;
				case 108018: msg = "抢购时间已过"; break;
				case 108019: msg = "抢购库存改变 无法回退"; break;
				case 108020: msg = "支付中 不能取消"; break;
				case 108021: msg = "改价太高"; break;
				case 108022: msg = "改价太多次"; break;
				case 108023: msg = "改价太低"; break;
				case 108024: msg = "分账未结束"; break;
				case 108025: msg = "下单没填地址"; break;
				case 108026: msg = "商品个数为0"; break;
				case 108027: msg = "已经请求过开发票"; break;
				case 108028: msg = "有售后单退款，现在不允许分钱"; break;
				case 108029: msg = "有售后单退款，永远不允许分钱"; break;
				case 108030: msg = "确认收货20天之前，现在不能分钱"; break;
				case 108031: msg = "订单状态不对，永远不允许分钱"; break;
				case 108032: msg = "分钱参数错误"; break;
				case 108036: msg = "生成商品ID失败"; break;
				case 108037: msg = "订单分佣金额为0"; break;
				case 108038: msg = "命中spu限购"; break;
				case 108039: msg = "单次下单太多sku"; break;
				case 108040: msg = "库存流水不存在"; break;
				case 109000: msg = "发货失败"; break;
				case 109001: msg = "已经发货"; break;
				case 109002: msg = "没带上商品"; break;
				case 100050: msg = "优惠券状态非法"; break;
				case 109100: msg = "优惠券状态不对"; break;
				case 109101: msg = "优惠券库存不足"; break;
				case 109102: msg = "优惠券还没过期"; break;
				case 109103: msg = "优惠券限领"; break;
				case 109104: msg = "user_coupon_id 不存在"; break;
				case 109105: msg = "提交订单的时候，优惠券不可用"; break;
				case 109106: msg = "coupon title 太长"; break;
				case 109107: msg = "校验折扣数失败"; break;
				case 109108: msg = "校验优惠价格失败"; break;
				case 109109: msg = "校验直减券是否小于最低价格"; break;
				case 109110: msg = "校验领取时间失败"; break;
				case 109111: msg = "校验有效时间失败"; break;
				case 109112: msg = "校验优惠券总发放量失败"; break;
				case 109113: msg = "校验限领失败"; break;
				case 109114: msg = "校验商户失败"; break;
				case 109115: msg = "地址不存在"; break;
				case 109116: msg = "非大陆地址"; break;
				case 109118: msg = "创建 优惠券类型 暂不支持"; break;
				case 109119: msg = "推广类型不对"; break;
				case 110000: msg = "该product_id已经存在一个抢购任务"; break;
				case 110001: msg = "抢购任务不存在"; break;
				case 110002: msg = "抢购状态不正确"; break;
				case 110003: msg = "抢购设置时间错误"; break;
				case 110004: msg = "抢购设置SKU缺少"; break;
				case 10005: msg = "传入的超管的身份证、姓名和注册的微信号不匹配"; break;
				case 100020: msg = "参数非法"; break;
				case 101000: msg = "文件为空"; break;
				case 101001: msg = "非法格式"; break;
				case 101002: msg = "超过文件大小"; break;
				case 101003: msg = "参数高宽与实际图片不符"; break;
				case 101004: msg = "上传失败"; break;
				case 102000: msg = "用户注册商店数已达上限"; break;
				case 1005: msg = "小程序昵称非法"; break;
				case 102001: msg = "渠道号非法"; break;
				case 102002: msg = "微信名非法"; break;
				case 200000: msg = "服务商登录appid非法"; break;
				case 200001: msg = "缺少第三方服务商信息"; break;
				case 260008: msg = "昵称不合法"; break;
				case 260003: msg = "名称被占用"; break;
				default:
					if(string.IsNullOrEmpty(msg))msg = "其他错误码:" + code;
					break;
			}
			return msg;
		}
		#endregion
	}
}