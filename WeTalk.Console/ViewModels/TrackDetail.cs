using System;
using System.Collections.Generic;

#nullable disable

namespace WeTalk.Console.ViewModels
{
	public partial class viewTrackDetail
	{

		public long TrackDetailid { get; set; }
		public DateTime Date { get; set; }
		public long Userid { get; set; }
		public long Cardid { get; set; }
		public long Companyid { get; set; }
		public long OperatorCompanyid { get; set; }
		public long OperatorCardid { get; set; }
		public long OperatorUserid { get; set; }
		public string FromType { get; set; }
		public int Type { get; set; }
		public int Count { get; set; }
		public DateTime FirstTime { get; set; }
		public DateTime Dtime { get; set; }

	}

	public partial class viewTrackDetailGroup
	{
		public long FromCardid { get; set; }
		public int Count { get; set; }
	}
}
