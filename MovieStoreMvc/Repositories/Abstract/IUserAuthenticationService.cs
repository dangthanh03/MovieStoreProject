using MovieStoreMvc.Models.DTO;
using System.Net.NetworkInformation;

namespace MovieStoreMvc.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(RegistrationModel model);
        //Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username);
        public  Task<List<string>> GetAvailableRoles();
    }
}
