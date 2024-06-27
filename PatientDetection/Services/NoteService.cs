using PatientDetection.Models;

namespace PatientDetection.Services
{
    public class NoteService
    {
        private readonly HttpClient _httpClient;

        public NoteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:6001");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<Note> GetNotesByPatientIdAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"gateway/patientnote/{patientId}");

            response.EnsureSuccessStatusCode();

            var notes = await response.Content.ReadFromJsonAsync<Note>();

            return notes;
        }
    }
}
