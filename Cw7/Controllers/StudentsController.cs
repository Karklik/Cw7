using Cw7.DAL;
using Cw7.DTOs.Requests;
using Cw7.DTOs.Responses;
using Cw7.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cw7.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentDbService _dbService;
        private readonly IConfiguration _configuration;

        public StudentsController(IStudentDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginRequest request)
        {
            var student = _dbService.GetStudent(request.Username, request.Password);
            if (student == null)
                return NotFound(new ErrorResponse
                {
                    Message = "Username or password dosen't exists or is incorrect"
                });

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                new Claim(ClaimTypes.Name, student.FirstName + "_" + student.LastName),
                new Claim(ClaimTypes.Role, "student")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "s16556",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            return Ok(new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString()
            });
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
            if (_dbService.DeleteStudent(indexNumber) > 0)
                return Ok("Usuwanie ukończone");
            else
                return NotFound("Nie znaleziono studneta");
        }
    }
}