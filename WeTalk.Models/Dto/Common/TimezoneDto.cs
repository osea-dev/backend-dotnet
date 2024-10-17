
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Common
{
	/// <summary>
	/// 时区信息
	/// </summary>
	public class TimezoneDto
	{
        /// <summary>
        /// 初始化
        /// </summary>
        public TimezoneDto()
        {
            TimeZones = new List<TimeZone>();
        }
        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// 时区列表
        /// </summary>
        public List<TimeZone> TimeZones { get; set; }

        /// <summary>
        /// 时区对象
        /// </summary>
        public class TimeZone
        {
            /// <summary>
            /// 时区表ID
            /// </summary>
            public long Timezoneid { get; set; }
            /// <summary>
            /// 时区名
            /// </summary>
            public string TimezoneName { get; set; }
            /// <summary>
            /// UTC分差（如北京时间：(0-8)*60=-480)
            /// </summary>
            public int UtcSec { get; set; }
        }
    }
}
