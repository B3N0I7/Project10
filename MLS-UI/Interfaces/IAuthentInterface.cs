using MLS_UI.Models;

namespace MLS_UI.Interfaces
{
    public interface IAuthentInterface
    {
        Task<AuthentStatus> RegisterAsync(Register register);
        Task<AuthentStatus> LoginAsync(Login loging);
        Task LogoutAsync();
    }
}
