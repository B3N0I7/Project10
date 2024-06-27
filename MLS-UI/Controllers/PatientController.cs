using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MLS_UI.Helpers;
using MLS_UI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MLS_UI.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public PatientController(ILogger<PatientController> logger, IHttpClientFactory httpClient, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient.CreateClient();
            //_httpClient.BaseAddress = new Uri("https://localhost:6001");    // Uri de la passerelle API
            //_httpClient.BaseAddress = new Uri("http://gateway-api:8080");
            _httpClient.BaseAddress = new Uri(CommonConstants.GATEWAY_URI);
            _contextAccessor = contextAccessor;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()     //string token)        // Index()
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            //var token = Request.Cookies["jwtToken"];
            //if (string.IsNullOrEmpty(token))
            //{
            //    return BadRequest("Token is missing");
            //}
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await this.SetUserRoleAsync();

            _httpClient.AddAuthenticationToken(HttpContext);

            var request = await _httpClient.GetAsync(CommonConstants.PATIENT_API);     // "/gateway/patientsmanager"

            if (request.IsSuccessStatusCode)
            {
                var patients = await request.Content.ReadFromJsonAsync<List<PatientDto>>();

                _logger.LogInformation($"List of patients retrieved successfully at {DateTime.UtcNow.ToLongTimeString()}");

                return View(patients);
            }
            else
            {
                _logger.LogError("Error retrieving patients");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PatientDto patient)
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(patient), Encoding.UTF8, "application/json");

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(CommonConstants.PATIENT_API, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{response.StatusCode} : Patient added successfully");

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
        public async Task<IActionResult> Update(int id)
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await _httpClient.GetAsync($"{CommonConstants.PATIENT_API}/{id}");

            if (request.IsSuccessStatusCode)
            {
                var patient = await request.Content.ReadFromJsonAsync<PatientDto>();

                _logger.LogInformation($"Patient with ID {id} retrieved successfully at {DateTime.UtcNow.ToLongTimeString()}");

                return View(patient);
            }
            else
            {
                _logger.LogError($"Error retrieving patient with ID {id}");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(int id, PatientDto updatedPatient)
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(updatedPatient), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{CommonConstants.PATIENT_API}/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{response.StatusCode} : Patient with ID {id} updated successfully");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Something went wrong while updating patient with ID {id}");

                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{CommonConstants.PATIENT_API}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var patient = await response.Content.ReadFromJsonAsync<PatientDto>();
                return View(patient);
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Unable to retrieve patient details");
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReallyDelete(int id)
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"{CommonConstants.PATIENT_API}/{id}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{response.StatusCode} : Patient with ID {id} deleted successfully");
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Something went wrong while deleting patient with ID {id}");
                return View("Error");
            }
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{CommonConstants.DETECTION_API}/details/{id}");     // http://detection-api:8080/api/detection/details/{id}

            if (response.IsSuccessStatusCode)
            {
                var patient = await response.Content.ReadFromJsonAsync<PatientDetectionDto>();
                var viewModel = new PatientDetailsViewModel { Patient = patient };
                return View(viewModel);
            }
            else
            {
                return View("Error");
            }
        }

        public IActionResult AddNote(int patientId, string patientName)
        {
            var model = new NoteDto
            {
                PatientId = patientId,
                Patient = patientName
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNote(NoteDto note)
        {
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(note), Encoding.UTF8, "application/json");


            var response = await _httpClient.PostAsync(CommonConstants.NOTE_API, content);       // http://note-api:8080/api/patientnote

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{response.StatusCode} : Patient added successfully");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"{response.StatusCode} : Something went wrong");

                return View("Error");
            }

        }
    }
}
