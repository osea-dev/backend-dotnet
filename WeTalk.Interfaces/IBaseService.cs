using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto;

namespace WeTalk.Interfaces
{
	public interface IBaseService
	{

        T ConvertFromRedis<T>(Dictionary<string, string> dicEntries);
		bool DelRedisObj(string key);
        string GetCardUrl(long userid, long cardid, string name);
        string GetNewsReviewUrl(long cardid, long newsid, long reviewid);
        string GetNewsUrl(long cardid, long newsid);
        T GetRedisObj<T>(string key);
        string GetRedisString(string key);
        string JsonCharFilter(string sourceStr);
        bool SetRedisObj(string key, object obj, int times = 86400);
        bool SetRedisObj(string key, string val, int times = 86400);
		object[] ToHashEntries(object obj);
	}
}