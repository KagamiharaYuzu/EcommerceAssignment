using System.Threading.Tasks;

namespace KLH60Services.Models.Services
{
    public interface ILoginService
    {
        /// <summary>
        /// This function checks if the user loging in is a manager
        /// </summary>
        /// <param name="email">The email of the user loging in</param>
        /// <returns>tuple integer code and string result message. 0 -> success, -1 -> Unauthorized, -2 -> not found</returns>
        Task<(int, string)> IsManagerLogin(string email);
    }
}