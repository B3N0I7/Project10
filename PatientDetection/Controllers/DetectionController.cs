using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientDetection.Models;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace PatientDetection.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DetectionController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public DetectionController(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<List<PatientWithNotes>>> GetPatientsWithNotes()
        {
            // Retrieve patients
            //var token = _contextAccessor.HttpContext.Session.GetString("token");
            //var token = Request.Cookies["jwtToken"];
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var patientsResponse = await _httpClient.GetAsync("http://patient-api:8080/api/patientsmanager");     // https://localhost:7001/api/patientsmanager");
            if (!patientsResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)patientsResponse.StatusCode, "Failed to retrieve patients");
            }

            var patients = await patientsResponse.Content.ReadFromJsonAsync<List<Patient>>();

            // Retrieve notes
            var notesResponse = await _httpClient.GetAsync("http://note-api:8080/api/patientnote/");    // https://localhost:7002/api/patientnote");
            if (!notesResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)notesResponse.StatusCode, "Failed to retrieve notes");
            }

            var notes = await notesResponse.Content.ReadFromJsonAsync<List<Note>>();

            //List<string> triggers = new List<string> { "Anormal", "Anticorps", "Cholestérol", "Fume", "Fumer", "Fumeur", "Fumeuse", "Hémoglobine A1C", "Microalbumine", "Poids", "Réaction", "Rechute", "Taille", "Vertige", "Vertiges" };
            // RegEx for triggers
            var triggersPattern = new Dictionary<string, string>
            {
                { "Anormal", "anorm[a-z]*" },
                { "Anticorps", "anticorp[a-z]*" },
                { "Cholestérol", "cholest[a-z]*" },
                { "Fume", "fum[a-z]*" },
                { "Hémoglobine A1C", "hémoglobine a1c|hb a1c|a1c" },
                { "Microalbumine", "microalbumin[a-z]*" },
                { "Poids", "poid[a-z]*" },
                { "Réaction", "réaction[a-z]*" },
                { "Rechute", "rechut[a-z]*" },
                { "Taille", "taille[a-z]*" },
                { "Vertige", "vertig[a-z]*" }
            };


            // Mapping in PatientWithNotes
            var patientsWithNotes = patients.Select(p =>
            {
                var patientNotes = notes.Where(n => n.PatientId == p.Id).ToList();

                var triggerCount = new Dictionary<string, int>();

                foreach (var trigger in triggersPattern)
                {
                    //triggerCount[trigger] = patientNotes.Count(n => n.PatientNote.Contains(trigger, StringComparison.OrdinalIgnoreCase));
                    var regex = new Regex(trigger.Value, RegexOptions.IgnoreCase);
                    int count = patientNotes.Count(note => regex.IsMatch(note.PatientNote));
                    triggerCount[trigger.Key] = count;
                }

                int totalUniqueTrigger = triggerCount.Values.Sum();
                //var countedTriggers = new HashSet<string>(); // <--- new HashSet<string>

                //foreach (var trigger in triggersPattern)
                //{
                //    var regex = new Regex(trigger.Value, RegexOptions.IgnoreCase);
                //    int count = 0;
                //    foreach (var note in patientNotes)
                //    {
                //        if (!countedTriggers.Contains(note.PatientNote) && regex.IsMatch(note.PatientNote))
                //        {
                //            count++;
                //            countedTriggers.Add(note.PatientNote); // mark as counted
                //        }
                //    }
                //    triggerCount[trigger.Key] = count;
                //}

                //int totalUniqueTrigger = triggerCount.Values.Sum();

                // Age
                int age = DateTime.Today.Year - p.BirthDate.Year;
                if (p.BirthDate > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                // Risk Level
                string riskLevel;
                if (totalUniqueTrigger >= 2 && totalUniqueTrigger <= 5 && age > 30)
                {
                    riskLevel = "Borderline";
                }
                else if ((p.Gender == "M" && age < 30 && totalUniqueTrigger >= 5)
                         || (p.Gender == "F" && age < 30 && totalUniqueTrigger >= 7)
                         || (age > 30 && totalUniqueTrigger >= 8))
                {
                    riskLevel = "Early onset";
                }
                else if ((p.Gender == "M" && age < 30 && totalUniqueTrigger >= 3)
                  || (p.Gender == "F" && age < 30 && totalUniqueTrigger >= 4)
                  || (age > 30 && totalUniqueTrigger >= 6))
                {
                    riskLevel = "In Danger";
                }
                else
                {
                    riskLevel = "None";
                }

                return new PatientWithNotes
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    BirthDate = p.BirthDate,
                    Gender = p.Gender,
                    PostalAddress = p.PostalAddress,
                    PhoneNumber = p.PhoneNumber,
                    Notes = patientNotes,
                    TriggersCount = triggerCount,
                    TotalUniqueTrigger = totalUniqueTrigger,
                    Age = age,
                    RiskLevel = riskLevel
                };
            }).ToList();

            return patientsWithNotes;
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<PatientWithNotes>> GetPatientWithDetailsById(int id)
        {
            // Récupérer le jeton d'authentification
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Récupérer le patient
            var patientResponse = await _httpClient.GetAsync($"http://patient-api:8080/api/patientsmanager/{id}");
            if (!patientResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)patientResponse.StatusCode, "Failed to retrieve patient");
            }
            var patient = await patientResponse.Content.ReadFromJsonAsync<Patient>();

            // Récupérer les notes du patient
            var notesResponse = await _httpClient.GetAsync($"http://note-api:8080/api/patientnote/{id}");
            if (!notesResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)notesResponse.StatusCode, "Failed to retrieve notes");
            }
            var notes = await notesResponse.Content.ReadFromJsonAsync<List<Note>>();

            // ... (Logique de calcul des triggers, de l'âge et du niveau de risque, identique à GetPatientsWithNotes)
            var triggersPattern = new Dictionary<string, string>
            {
                { "Anormal", "anorm[a-z]*" },
                { "Anticorps", "anticorp[a-z]*" },
                { "Cholestérol", "cholest[a-z]*" },
                { "Fume", "fum[a-z]*" },
                { "Hémoglobine A1C", "hémoglobine a1c|hb a1c|a1c" },
                { "Microalbumine", "microalbumin[a-z]*" },
                { "Poids", "poid[a-z]*" },
                { "Réaction", "réaction[a-z]*" },
                { "Rechute", "rechut[a-z]*" },
                { "Taille", "taille[a-z]*" },
                { "Vertige", "vertig[a-z]*" }
            };


            // Mapping in PatientWithNotes
            //var patientsWithNotes = patient.Select(p =>
            //{
            var patientNotes = notes.Where(n => n.PatientId == patient.Id).ToList();

            var triggerCount = new Dictionary<string, int>();

            foreach (var trigger in triggersPattern)
            {
                //triggerCount[trigger] = patientNotes.Count(n => n.PatientNote.Contains(trigger, StringComparison.OrdinalIgnoreCase));
                var regex = new Regex(trigger.Value, RegexOptions.IgnoreCase);
                int count = patientNotes.Count(note => regex.IsMatch(note.PatientNote));
                triggerCount[trigger.Key] = count;
            }

            int totalUniqueTrigger = triggerCount.Values.Sum();

            // Age
            int age = DateTime.Today.Year - patient.BirthDate.Year;
            if (patient.BirthDate > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            // Risk Level
            string riskLevel;
            if (totalUniqueTrigger >= 2 && totalUniqueTrigger <= 5 && age > 30)
            {
                riskLevel = "Borderline";
            }
            else if ((patient.Gender == "M" && age < 30 && totalUniqueTrigger >= 5)
                     || (patient.Gender == "F" && age < 30 && totalUniqueTrigger >= 7)
                     || (age > 30 && totalUniqueTrigger >= 8))
            {
                riskLevel = "Early onset";
            }
            else if ((patient.Gender == "M" && age < 30 && totalUniqueTrigger >= 3)
              || (patient.Gender == "F" && age < 30 && totalUniqueTrigger >= 4)
              || (age > 30 && totalUniqueTrigger >= 6))
            {
                riskLevel = "In Danger";
            }
            else
            {
                riskLevel = "None";
            }
            var patientWithNotes = new PatientWithNotes
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                PostalAddress = patient.PostalAddress,
                PhoneNumber = patient.PhoneNumber,
                Notes = patientNotes,
                TriggersCount = triggerCount,
                TotalUniqueTrigger = totalUniqueTrigger,
                Age = age,
                RiskLevel = riskLevel
            };

            return patientWithNotes;
        }
    }
}
