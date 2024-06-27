using PatientMS.PatientCore.Models;

namespace PatientMS.PatientCore.Interfaces
{
    public interface IPatientInterface
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> GetByIdAsync(int id);
        Task<int> CreateAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task<int> DeleteAsync(int id);
    }
}
