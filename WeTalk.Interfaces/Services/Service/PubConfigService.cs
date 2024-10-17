using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Interfaces.Base;

namespace WeTalk.Interfaces.Services
{
	public partial class PubConfigService : BaseService, IPubConfigService
	{
		private readonly IPubConfigBaseService _pubConfigBaseService;
		public PubConfigService(IPubConfigBaseService pubConfigBaseService)
		{
			_pubConfigBaseService = pubConfigBaseService;
		}

		#region "获取指定的数据字典JSON"
		public string GetConfig(string name, string valType = "val")
		{
			return _pubConfigBaseService.GetConfig(name, "", valType);
		}

		public async Task<string> GetConfig(string prefix, string key, string str = "", string action = "val")
		{
			return _pubConfigBaseService.GetConfig(prefix, key, str, action);
		}

		public int GetConfigInt(string key, int n = 0)
		{
			return _pubConfigBaseService.GetConfigInt(key, n);
		}
		public Dictionary<string, string> GetConfigs(string keys, string action = "val")
		{
			return _pubConfigBaseService.GetConfigs(keys, action);
		}

		public bool UpdateConfig(string key, string val, string action = "val")
		{
			return _pubConfigBaseService.UpdateConfig( key, val, action);
		}

		public bool UpdateConfigs(Dictionary<string, string> dic, string action = "val")
		{
			return _pubConfigBaseService.UpdateConfigs(dic, action);
		}
		#endregion
	}
}