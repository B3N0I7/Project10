//using Microsoft.AspNetCore.Mvc;
//using PatientDetection.Models;

//namespace PatientDetection.Services
//{
//    public class ApiClient
//    {
//        [ApiController]
//        [Route("[controller]")]
//        public class DetectionController : ControllerBase
//        {
//            private readonly ApiClient _apiClient;

//            public DetectionController(ApiClient apiClient)
//            {
//                _apiClient = apiClient;
//            }

//            [HttpGet("{patientId}/notes")]
//            public async Task<List<Note>> GetNotesForPatient(string patientName)
//            {
//                // Récupérer les informations du patient
//                //var patient = await _apiClient.GetPatient(patientId);

//                // Récupérer les notes du patient
//                var notes = await _apiClient.GetNotesForPatientAsync(patientName);

//                return notes;
//            }
//        }

//    }
//}
