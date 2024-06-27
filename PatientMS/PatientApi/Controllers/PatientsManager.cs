using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientMS.PatientCore.Interfaces;
using PatientMS.PatientCore.Models;

namespace Booking.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsManager : ControllerBase
    {
        private readonly IPatientInterface _context;
        private readonly ILogger<PatientsManager> _logger;

        public PatientsManager(IPatientInterface context, ILogger<PatientsManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all Patients.
        /// </summary>
        // GET: api/Patients
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            try
            {
                var patients = await _context.GetAllAsync();

                if (patients == null)
                {
                    _logger.LogInformation("No Patient found");

                    return NotFound();
                }

                _logger.LogInformation($"List of Patients retrieved successfully at {DateTime.UtcNow.ToLongTimeString() + 1}");

                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Patients");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Retrieve a specific Patient by ID.
        /// </summary>
        /// <param name="id">Patient ID</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> GetPatientById(int id)
        {
            try
            {
                var patient = await _context.GetByIdAsync(id);

                if (patient == null)
                {
                    _logger.LogInformation($"Patient {id} not found");

                    return NotFound();
                }

                _logger.LogInformation($"Patient {id} retrieved successfully at {DateTime.UtcNow.ToLongTimeString() + 1}");

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving Patient {id}");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new Patient.
        /// </summary>
        /// <param name="patient">Patient object</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
        {
            try
            {
                if (patient == null)
                {
                    _logger.LogError("Invalid Patient data");

                    return BadRequest();
                }

                await _context.CreateAsync(patient);
                _logger.LogInformation($"New Patient created successfully at {DateTime.UtcNow.ToLongTimeString() + 1}");

                return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new Patient");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a Patient.
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <param name="patient">Patient object</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePatient(int id, Patient patient)
        {
            try
            {
                if (id != patient?.Id)
                {
                    _logger.LogError($"Patient {id} mismatch");

                    return BadRequest();
                }

                await _context.UpdateAsync(patient);
                _logger.LogInformation($"Patient {id} updated successfully at {DateTime.UtcNow.ToLongTimeString() + 1}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating Patient {id}");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete a Patient by ID.
        /// </summary>
        /// <param name="id">Patient ID</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePatient(int id)
        {
            try
            {
                await _context.DeleteAsync(id);
                _logger.LogInformation($"Patient {id} deleted successfully at {DateTime.UtcNow.ToLongTimeString() + 1}");

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Patient {id}");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        ///// <summary>
        ///// Retrieve patients based on filter criteria.
        ///// </summary>
        ///// <param name="id">Patient ID for filtering</param>
        //[HttpGet("Filter")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<IEnumerable<Patient>>> FilterPatients(int? id)
        //{
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return BadRequest("Patient ID cannot be null");
        //        }

        //        var patient = await _context.GetByIdAsync(id.Value);

        //        if (patient == null)
        //        {
        //            _logger.LogInformation($"Patient {id} not found");

        //            return NotFound();
        //        }

        //        var filteredPatients = new List<Patient> { patient };

        //        _logger.LogInformation($"Filtered Patients retrieved successfully at {DateTime.UtcNow.ToLongTimeString()}");

        //        return Ok(filteredPatients);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error filtering Patients by ID: {id}");

        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
    }
}
