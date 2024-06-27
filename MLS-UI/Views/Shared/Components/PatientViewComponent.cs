


using Microsoft.AspNetCore.Mvc;
using MLS_UI.Models;

public class PatientViewComponent : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PatientViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Create an HttpClient instance from the factory
        var client = _httpClientFactory.CreateClient();

        // Set the base address to match your controller's API endpoint
        client.BaseAddress = new Uri("https://localhost:6001");  // Replace with your actual base URL

        // Send a GET request to the same endpoint as your controller's Index action
        var request = await client.GetAsync("/gateway/patientsmanager");

        // Handle the response
        if (request.IsSuccessStatusCode)
        {
            var patients = await request.Content.ReadFromJsonAsync<List<PatientDto>>();
            return View(patients);
        }
        else
        {
            // handle error
            return Content("Error retrieving patients");
        }
    }
}
