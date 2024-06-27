using Microsoft.AspNetCore.Mvc;
using MLS_UI.Models;
using System.Net.Http.Headers;

namespace MLS_UI.Controllers
{
    public class PatientDetailsController : Controller
    {
        private readonly HttpClient _httpClient;

        public PatientDetailsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Details(int id)
        {
            // Récupérer le jeton d'authentification
            var token = Request.Cookies["jwtToken"]; // Ou votre méthode de récupération de jeton

            if (string.IsNullOrEmpty(token))
            {
                // Gérer l'erreur si le jeton est manquant
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Appeler l'API Detection pour obtenir les détails du patient
            var response = await _httpClient.GetAsync($"http://detection-api:8080/api/detection/details/1002");

            if (response.IsSuccessStatusCode)
            {
                var patient = await response.Content.ReadFromJsonAsync<PatientDetectionDto>();
                var viewModel = new PatientDetailsViewModel { Patient = patient };
                return View(viewModel);
            }
            else
            {
                // Gérer l'erreur si l'appel à l'API échoue
                return View("Error");
            }
        }
    }
}
