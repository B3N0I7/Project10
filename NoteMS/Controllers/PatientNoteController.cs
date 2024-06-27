using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteMS.Models;
using NoteMS.Services;

namespace NoteMS.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientNoteController : ControllerBase
    {
        private readonly PatientNoteService _patientNoteService;

        public PatientNoteController(PatientNoteService patientNoteService) =>
            _patientNoteService = patientNoteService;

        [HttpGet]
        public async Task<List<Note>> Get() =>
            await _patientNoteService.GetAsync();
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Note>>> Get(int id)
        {
            var notes = await _patientNoteService.GetByPatientAsync(id);

            if (notes is null)
            {
                return NotFound();
            }

            return notes;
        }

        //[HttpGet("{patientName}")]
        //public async Task<ActionResult<List<Note>>> Get(string patientName)
        //{
        //    var notes = await _patientNoteService.GetByPatientAsync(patientName);

        //    if (notes is null)
        //    {
        //        return NotFound();
        //    }

        //    return notes;
        //}

        //[HttpGet("{id:length(24)}")]
        //public async Task<ActionResult<Note>> Get(string id)
        //{
        //    var book = await _patientNoteService.GetAsync(id);

        //    if (book is null)
        //    {
        //        return NotFound();
        //    }

        //    return book;
        //}

        [HttpPost]
        public async Task<IActionResult> Post(Note newNote)
        {
            await _patientNoteService.CreateAsync(newNote);

            return CreatedAtAction(nameof(Get), new { id = newNote.Id }, newNote);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Note updatedNote)
        {
            var book = await _patientNoteService.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            updatedNote.Id = book.Id;

            await _patientNoteService.UpdateAsync(id, updatedNote);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _patientNoteService.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            await _patientNoteService.RemoveAsync(id);

            return NoContent();
        }
    }
}
