using AutoMapper;
using DotnetApi.Dtos;
using DotnetApi.Models;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AddressBookContext _context;
        IMapper _mapper;
        public DepartmentsController(AddressBookContext context)
        {
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DepartmentDto, Department>();

            }));
            _context = context;
        }

        //[HttpGet("Departments/{DepartmentId}/{userId}/{searchParam}")]
        //public IActionResult GetDepartments(int DepartmentId = 0, int userId = 0, string searchParam = "None")
        //{
        //    var query = _context.Departments.AsQueryable();

        //    if (DepartmentId != 0)
        //    {
        //        query = query.Where(j => j.DepartmentId == DepartmentId);
        //    }

        //    if (userId != 0)
        //    {
        //        query = query.Where(j => j.UserId == userId);
        //    }

        //    if (!string.IsNullOrEmpty(searchParam) && searchParam.ToLower() != "none")
        //    {
        //        query = query.Where(j => j.Name.Contains(searchParam));
        //    }

        //    var Departments = query.ToList();
        //    return Ok(Departments);
        //}

        [HttpGet("MyDepartments")]
        public List<Department> GetMyDepartments()
        {
            var userIdClaim = User.FindFirst("userId");


            int userId = int.Parse(userIdClaim.Value);
            var Departments = _context.Departments.Where(j => j.UserId == userId).ToList();

            return Departments;
        }

        [HttpPut("UpsertDepartment")]
        public IActionResult UpsertDepartment(DepartmentDto departmentD)
        {
            Department department = _mapper.Map<Department>(departmentD);
            var userIdClaim = User.FindFirst("userId");


            department.UserId = int.Parse(userIdClaim.Value);

            if (department.DepartmentId > 0)
            {
                // Update existing Department
                var existingDepartment = _context.Departments.Find(department.DepartmentId);
                if (existingDepartment == null)
                {
                    return NotFound("Department not found.");
                }

                existingDepartment.Name = department.Name;
                _context.Departments.Update(existingDepartment);
            }
            else
            {
                // Create new Department
                _context.Departments.Add(department);
            }

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("Department/{departmentId}")]
        public IActionResult DeleteDepartment(int departmentId)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var Department = _context.Departments.FirstOrDefault(j => j.DepartmentId == departmentId && j.UserId == userId);
            if (Department == null)
            {
                return NotFound("Department not found");
            }

            _context.Departments.Remove(Department);
            _context.SaveChanges();

            return Ok();
        }
    }
}
