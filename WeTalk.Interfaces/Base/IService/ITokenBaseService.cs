namespace WeTalk.Interfaces.Base
{
	public interface ITokenBaseService : IBaseService
	{
		/// <summary>
		/// 获取微信服务端AccessToken
		/// </summary>
		/// <returns></returns>
		string UpdateAccessToken();
	}
}