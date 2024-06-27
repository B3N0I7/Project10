using Microsoft.EntityFrameworkCore;
using PatientMS.PatientCore.Models;
using PatientMS.PatientInfrastructure.Data;


namespace PatientMS.PatientCore.Interfaces
{
    public class PatientInterface : IPatientInterface
    {
        private readonly PatientDbContext _context;

        public PatientInterface(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<int> CreateAsync(Patient patient)
        {
            _context.Patients.Add(patient);

            return await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);

            return await _context.SaveChangesAsync();
        }
    }
}
