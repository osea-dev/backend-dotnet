using SqlSugar;
using System.Linq;
using WeTalk.Models;

namespace WeTalk.Common.Helper
{
	public static class PageInfo 
	{

		#region "设置分页"
		public static PagedInfo<T> ToPageInfo<T>(this ISugarQueryable<T> thisValue, int page, int pagesize)
		{
			int total = 0;
			var pageObj = new PagedInfo<T>();
			pageObj.Page = page;
			pageObj.PageSize = pagesize;
			pageObj.DataSource = thisValue.ToPageList(page, pagesize, ref total);
			pageObj.TotalCount = total;
			return pageObj;
		}
		#endregion
	}
}
