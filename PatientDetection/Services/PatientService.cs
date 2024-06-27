using PatientDetection.Models;

namespace PatientDetection.Services
{
    public class PatientService
    {
        private readonly HttpClient _httpClient;

        public PatientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:6001");
        }

        public async Task<Patient> GetPatientByIdAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"gateway/patient/{patientId}");

            response.EnsureSuccessStatusCode();

            var patient = await response.Content.ReadFromJsonAsync<Patient>();

            return patient;
        }
    }
}
