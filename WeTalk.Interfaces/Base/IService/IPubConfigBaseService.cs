using System.Collections.Generic;

namespace WeTalk.Interfaces.Base
{
	public interface IPubConfigBaseService : IBaseService
	{
		string GetConfig(string key, string str = "", string action = "val");
		string GetConfig(string prefix, string key, string str = "", string action = "val");
		decimal GetConfigDecimal(string key, decimal n = 0);
		int GetConfigInt(string key, int n = 0);
		Dictionary<string, string> GetConfigs(string keys, string action = "val");
		Dictionary<string, string> GetConfigs(string prefix, string keys, string action = "val");
		bool UpdateConfig(string key, string val, string action = "val");
		bool UpdateConfigs(Dictionary<string, string> dic, string action = "val");
	}
}