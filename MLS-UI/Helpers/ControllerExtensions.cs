using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace MLS_UI.Helpers
{
    public static class ControllerExtensions
    {
        public static async Task SetUserRoleAsync(this Controller controller)
        {
            var userManager = controller.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.GetUserAsync(controller.User);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                controller.ViewData["Role"] = roles.FirstOrDefault();
            }
        }

        public static void AddAuthenticationToken(this HttpClient httpClient, HttpContext httpContext)
        {
            var token = httpContext.Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                throw new Exception("Token is missing");
            }
        }
    }
}
