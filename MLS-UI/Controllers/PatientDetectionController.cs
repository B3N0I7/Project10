using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MLS_UI.Models;
using System.Net.Http.Headers;

namespace MLS_UI.Controllers
{
    public class PatientDetectionController : Controller
    {
        private readonly ILogger<PatientDetectionController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public PatientDetectionController(ILogger<PatientDetectionController> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            //_httpClient.BaseAddress = new Uri("https://localhost:6001");
            _httpClient.BaseAddress = new Uri("http://gateway-api:8080");
            _contextAccessor = contextAccessor;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index() //Index(int? id)
        {
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await _httpClient.GetAsync("/gateway/patientdetection");  // https://localhost:7003/api/detection ("/gateway/patientdetection")

            if (request.IsSuccessStatusCode)
            {
                var patients = await request.Content.ReadFromJsonAsync<List<PatientDetectionDto>>();

                //_logger.LogInformation(patient);

                // Add modify
                //var patients = await request.Content.ReadFromJsonAsync<List<PatientDetectionDto>>();

                //foreach (var pat in patients)
                //{
                //    patient.Notes = patient.Notes.Select(n => new NoteDto
                //    {
                //        Id = n.Id ?? string.Empty,
                //        Patient = n.Patient,return StatusCode(StatusCodes.Status400BadRequest);
                //        PatientNote = n.PatientNote,
                //        PatientId = n.PatientId
                //    }).ToList();
                //}

                _logger.LogInformation($"Patient retrieved successfully at {DateTime.UtcNow.ToLongTimeString()}");

                return View(patients);
            }
            else
            {
                _logger.LogError($"Error retrieving patient ");

                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }
    }
}
