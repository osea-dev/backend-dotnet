using System.Threading.Tasks;
using WeTalk.Models.Redis;

namespace WeTalk.Web.Services
{
	public interface IRedisService
	{
		void DelToken(string token);
		Task DelTokenAsync(string token);
		void DelUser(string username);
		Task DelUserAsync(string username);
		bool ExistsToken(string token);
		bool ExistsToken(string token, string key);
		bool ExistsUser(string username);
		bool ExistsUser(string username, string key);
		UserToken GetToken(string token);
		string GetToken(string token, string key);
		UserToken GetUser(string username);
		string GetUser(string username, string key);
		void SetToken(string token, UserToken redis_model, int times = 7200);
		Task SetTokenAsync(string token, UserToken redis_model, int times = 7200);
		bool SetTokenField(string token, string field, object val, int times = 7200);
		Task<bool> SetTokenFieldAsync(string token, string field, object val, int times = 7200);
		void UpdateTokenExpire(string token, int times = 7200);
		void SetUser(string username, UserToken redis_model, int times = 7200);
		Task SetUserAsync(string username, UserToken redis_model, int times = 7200);
		bool SetUserField(string username, string field, object val, int times = 7200);
		Task<bool> SetUserFieldAsync(string username, string field, object val, int times = 7200);

	}
}