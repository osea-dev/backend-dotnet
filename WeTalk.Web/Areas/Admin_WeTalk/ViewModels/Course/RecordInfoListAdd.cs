using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class RecordInfoListAdd : Models.WebRecordCourseInfo
	{
		public string Lang { get; set; }
		public string Title { get; set; }

	}
}
