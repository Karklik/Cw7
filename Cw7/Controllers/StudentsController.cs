using Cw7.DAL;
using Cw7.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw7.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentDbService _dbService;

        public StudentsController(IStudentDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents(orderBy));
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var student = _dbService.GetStudent(indexNumber);
            if (student != null)
                return Ok(student);
            else
                return NotFound("Nie znaleziono studneta");
        }

        [HttpGet("{indexNumber}/enrollment")]
        public IActionResult GetStudentEnrollment(string indexNumber)
        {
            var student = _dbService.GetStudentEnrollment(indexNumber);
            if (student != null)
                return Ok(student);
            else
                return NotFound("Nie znaleziono studneta");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            if (_dbService.CreateStudent(student) > 0)
                return Ok(student);
            return Conflict(student);
        }

        [HttpPut("{indexNumber}")]
        public IActionResult UpdateStudent(string indexNumber, Student student)
        {
            if (_dbService.UpdateStudent(indexNumber, student) > 0)
                return Ok("Aktualizacja dokończona");
            else
                return NotFound("Nie znaleziono studneta");
        }

        [HttpDelete("{indexNumber}")]
        public IActionResult DeleteStudent(string indexNumber)
        {
            if(_dbService.DeleteStudent(indexNumber) > 0)
                return Ok("Usuwanie ukończone");
            else
                return NotFound("Nie znaleziono studneta");
        }
    }
}