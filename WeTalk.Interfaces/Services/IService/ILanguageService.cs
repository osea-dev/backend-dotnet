using System.Threading.Tasks;
using WeTalk.Models;
namespace WeTalk.Interfaces.Services
{
	public interface ILanguageService
	{
		Task<ApiResult> SetLang(string lang);
	}
}
