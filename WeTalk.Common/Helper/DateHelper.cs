using System;

namespace WeTalk.Common.Helper
{
	public class DateHelper
	{
		/// <summary>
		/// 10位时间戳转为时间
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static DateTime StampToDateTime(string time)
		{
			time = time.Substring(0, 10);
			double timestamp = Convert.ToInt64(time);
			System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
			dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
			return dateTime;
		}

		/// <summary>
		/// 计算时间间隔
		/// </summary>
		/// <param name="time1"></param>
		/// <param name="time2"></param>
		/// <returns>天，时，分</returns>
		public static string TimeSubTract(DateTime time1, DateTime time2)
		{
			TimeSpan subTract = time1.Subtract(time2);
			return $"{subTract.Days} 天 {subTract.Hours} 时 {subTract.Minutes} 分 ";
		}

		/// <summary>
		///  时间戳转本地时间-时间戳精确到秒
		/// </summary> 
		public static DateTime ToLocalTimeDateBySeconds(long unix)
		{
			var dto = DateTimeOffset.FromUnixTimeSeconds(unix);
			return dto.ToLocalTime().DateTime;
		}

		/// <summary>
		///  时间转时间戳Unix-时间戳精确到秒
		/// </summary> 
		public static long ToUnixTimestampBySeconds(DateTime dt)
		{
			DateTimeOffset dto = new DateTimeOffset(dt);
			return dto.ToUnixTimeSeconds();
		}


		/// <summary>
		///  时间戳转本地时间-时间戳精确到毫秒
		/// </summary> 
		public static DateTime ToLocalTimeDateByMilliseconds(long unix)
		{
			var dto = DateTimeOffset.FromUnixTimeMilliseconds(unix);
			return dto.ToLocalTime().DateTime;
		}

		/// <summary>
		///  时间转时间戳Unix-时间戳精确到毫秒
		/// </summary> 
		public static long ToUnixTimestampByMilliseconds(DateTime dt)
		{
			DateTimeOffset dto = new DateTimeOffset(dt);
			return dto.ToUnixTimeMilliseconds();
		}

        #region "时间戳转换为时间"
        /// <summary>
        /// 将普通时间转换成int时间,10位秒
        /// </summary>
        /// <returns>返回int</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (int)(time - startTime).TotalSeconds;
        }
        public static int ConvertUtcDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
            return (int)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 将普通时间转换成int时间,13位豪秒
        /// </summary>
        /// <returns>返回int</returns>
        public static int ConvertDateTimeInt13(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (int)(time - startTime).TotalMilliseconds;
        }
        public static int ConvertUtcDateTimeInt13(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
            return (int)(time - startTime).TotalMilliseconds;
        }
        /// <summary>
        /// 将普通时间转换成long类型,10位秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertDateTimeLong(System.DateTime time)
        {
            DateTime BaseTime = new DateTime(1970, 1, 1);
            return (time.Ticks - BaseTime.Ticks) / 10000000 - 8 * 60 * 60;
        }
        /// <summary>
        /// 将普通时间转换成long类型,13位豪秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertDateTimeLong13(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (long)(time - startTime).TotalMilliseconds;
        }
        public static long ConvertUtcDateTimeLong13(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
            return (long)(time - startTime).TotalMilliseconds;
        }
        /// <summary>
        /// 将普通时间数值转换成DateTime类型
        /// </summary>
        /// <param name="datestr"></param>
        /// <returns></returns>
        public static DateTime ConvertIntToDateTime(string datestr,string timeZoneInfo = "UTC")
        {
            DateTime dtStart;
            switch (timeZoneInfo.ToUpper()) {
                case "UTC":
                    dtStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
                    break;
                case "Local":
                    dtStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
                    break;
                default:
                    dtStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
                    break;
            }
            datestr = datestr.PadRight(17,'0');//补齐17位
			long lTime = long.Parse(datestr);
			TimeSpan toNow = new TimeSpan(lTime);
			DateTime dtResult = dtStart.Add(toNow);
			return dtResult;
		}
		public static DateTime ConvertInt13ToDateTime(string datestr)
        {
            System.DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long lTime = long.Parse(datestr + "0000");//补齐17位
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
        /// <summary>
        /// 将普通long时间数值转换成DateTime类型
        /// </summary>
        /// <param name="datestr"></param>
        /// <returns></returns>
        public static DateTime ConvertLongToDateTime(long d)
        {
            System.DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long lTime = long.Parse(d + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
        public static DateTime ConvertLongToUtcDateTime(long d)
        {
            System.DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
            long lTime = long.Parse(d + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
        #endregion
    }
}
