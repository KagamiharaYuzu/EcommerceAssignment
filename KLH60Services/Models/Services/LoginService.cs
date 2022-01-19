using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace KLH60Services.Models.Services
{
    public class LoginService : ILoginService
    {
        private readonly StoreServiceContext _db;

        public LoginService(StoreServiceContext db) => _db = db;

        public async Task<(int, string)> IsManagerLogin(string email)
        {
            if (email is null)
                throw new ArgumentNullException(nameof(email), "Please Provide credentials to login.");
            var aspNetUser = await _db.AspNetUsers.FirstOrDefaultAsync(aspUser => aspUser.Email == email);
            if (aspNetUser != null)
            {
                string managerId = (await _db.AspNetRoles.FirstAsync(role => role.Name == "Manager")).Id;
                return (_db.AspNetUserRoles.FirstOrDefault(userRole => userRole.UserId == aspNetUser.Id).RoleId == managerId) switch
                {
                    true => (0, aspNetUser.UserName),
                    _ => (-1, "not authorized")
                };
            }
            else return (-2, "Invalid credentials. No user found");
        }
    }
}