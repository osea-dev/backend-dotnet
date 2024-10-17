using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeTalk.Interfaces.Services
{
	public partial interface IPubConfigService : IBaseService
	{
		string GetConfig(string name, string val_type = "val");
		Task<string> GetConfig(string prefix, string key, string str = "", string action = "val");
		int GetConfigInt(string key, int n = 0);
		Dictionary<string, string> GetConfigs(string keys, string action = "val");
		bool UpdateConfig(string key, string val, string action = "val");
		bool UpdateConfigs(Dictionary<string, string> dic, string action = "val");
	}
}