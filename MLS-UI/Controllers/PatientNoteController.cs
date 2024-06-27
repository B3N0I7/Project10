using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MLS_UI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MLS_UI.Controllers
{
    public class PatientNoteController : Controller
    {
        private readonly ILogger<PatientNoteController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public PatientNoteController(ILogger<PatientNoteController> logger, IHttpClientFactory httpClient, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient.CreateClient();
            //_httpClient.BaseAddress = new Uri("https://localhost:6001");
            _httpClient.BaseAddress = new Uri("http://gateway-api:8080");
            _contextAccessor = contextAccessor;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await _httpClient.GetAsync("/gateway/patientnote");

            if (request.IsSuccessStatusCode)
            {
                var notes = await request.Content.ReadFromJsonAsync<List<NoteDto>>();

                _logger.LogInformation($"List of notes retrieved successyfully");

                return View(notes);
            }
            else
            {
                _logger.LogError("Error retrieving notes");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(NoteDto note)
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(note), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/gateway/patientnote", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{response.StatusCode} : Note added successfully");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Something went wrong");

                return View("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(string id)          // int a tester en modifiant le modele
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await _httpClient.GetAsync($"/gateway/patientnote/{id}");

            if (request.IsSuccessStatusCode)
            {
                var note = await request.Content.ReadFromJsonAsync<NoteDto>();

                _logger.LogInformation($"Patient note with ID {id} retrieved successfully at {DateTime.UtcNow.ToLongTimeString()}");

                return View(note);
            }
            else
            {
                _logger.LogError($"Error retrieving patient note with ID {id}");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(string id, NoteDto updatedNote)         // int a tester en modifiant le modele
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(updatedNote), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/gateway/patientnote/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{response.StatusCode} : Note with ID {id} updated successfully");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Something went wrong while updating note with ID {id}");

                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(string id)          // int a tester en modifiant le modele
        {
            // recuperation du token
            //var token = _contextAccessor.HttpContext.Session.GetString("token");

            // if (string.IsNullOrEmpty(token))
            // {
            //     return BadRequest("Token is missing");
            // }

            // ajout du token dans le header du httpclient
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/gateway/patientnote/{id}");

            if (response.IsSuccessStatusCode)
            {
                var patient = await response.Content.ReadFromJsonAsync<NoteDto>();
                return View(patient);
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Unable to retrieve patient note");
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReallyDelete(string id)            // int a tester en modifiant le modele
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"/gateway/patientnote/{id}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{response.StatusCode} : Note with ID {id} deleted successfully");
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Something went wrong while deleting patient note with ID {id}");
                return View("Error");
            }
        }
    }
}
