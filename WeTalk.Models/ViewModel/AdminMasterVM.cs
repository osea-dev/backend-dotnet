using System;
using System.Collections.Generic;

namespace WeTalk.Models.ViewModel
{
	public class AdminMasterVM : AdminMaster
	{
		public List<long> list_Functionid { get; set; }
		public List<long> list_Menuid { get; set; }
		public int KeepHours { get; set; }
		public string Lang { get; set; }
		public bool Isadmin { get; set; }
	}
}