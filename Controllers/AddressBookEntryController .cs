using AutoMapper;
using DotnetApi.Dtos;
using DotnetApi.Models;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetAPI.Controllers
{
 //   [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AddressBookEntryController : ControllerBase
    {
        private readonly AddressBookContext _context;
        private readonly IMapper _mapper;

        public AddressBookEntryController(AddressBookContext context)
        {
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddressBookEntryDto, AddressBookEntry>();
            }));
            _context = context;
        }

        [HttpGet("Entries/{entryId}/{userId}/{searchParam}")]
        public IActionResult GetEntries(int entryId = 0, int userId = 0, string searchParam = "None")
        {
            var query = _context.AddressBookEntries.AsQueryable();

            if (entryId != 0)
            {
                query = query.Where(e => e.Id == entryId);
            }

            if (userId != 0)
            {
                query = query.Where(e => e.UserId == userId);
            }

            if (!string.IsNullOrEmpty(searchParam) && searchParam.ToLower() != "none")
            {
                query = query.Where(e => e.FullName.Contains(searchParam) || e.Email.Contains(searchParam));
            }

            var entries = query.ToList();
            return Ok(entries);
        }

        [HttpGet("MyEntries")]
        public List<SendingAddressEntryDto> GetMyEntries()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null) return new List<SendingAddressEntryDto>();

            int userId = int.Parse(userIdClaim.Value);

            var entries = _context.AddressBookEntries
                .Where(e => e.UserId == userId)
                .Include(e => e.Job)
                .Include(e => e.Department)
                .ToList();

            // Map AddressBookEntries to SendingAddressEntryDto
            var entriesDto = entries.Select(entry => new SendingAddressEntryDto
            {
                Id = entry.Id,
                FullName = entry.FullName,
                JobId = entry.JobId,
                DepartmentId = entry.DepartmentId,
                MobileNumber = entry.MobileNumber,
                DateOfBirth = entry.DateOfBirth,
                Address = entry.Address,
                Email = entry.Email,
                Password = entry.Password,
 
                PhotoStr = entry.Photo != null ? Convert.ToBase64String(entry.Photo) : null,
                // Include the full Job and Department objects
               Job = entry.Job.Title,
               Department = entry.Department.Name
            }).ToList();

            return entriesDto;
        }



        [HttpPut("UpsertEntry")]
        public IActionResult UpsertEntry(AddressBookEntryDto entryDto)
        {
            AddressBookEntry entry = _mapper.Map<AddressBookEntry>(entryDto);
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null) return Unauthorized("User ID not found in token.");

            entry.UserId = int.Parse(userIdClaim.Value);

            // If the Photo field is a base64 string, decode it into byte array
            if (!string.IsNullOrEmpty(entryDto.PhotoStr))
            {
                entry.Photo = Convert.FromBase64String(entryDto.PhotoStr);
            }

            if (entry.Id > 0)
            {
                // Update existing entry
                var existingEntry = _context.AddressBookEntries.Find(entry.Id);
                if (existingEntry == null) return NotFound("Entry not found.");

                existingEntry.FullName = entry.FullName;
                existingEntry.JobId = entry.JobId;
                existingEntry.DepartmentId = entry.DepartmentId;
                existingEntry.MobileNumber = entry.MobileNumber;
                existingEntry.DateOfBirth = entry.DateOfBirth;
                existingEntry.Address = entry.Address;
                existingEntry.Email = entry.Email;
                existingEntry.Password = entry.Password;
                existingEntry.Photo = entry.Photo; // Update the photo

                _context.AddressBookEntries.Update(existingEntry);
            }
            else
            {
                // Create new entry
                _context.AddressBookEntries.Add(entry);
            }

            _context.SaveChanges();
            return Ok();
        }


        [HttpDelete("Entry/{entryId}")]
        public IActionResult DeleteEntry(int entryId)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null) return Unauthorized("User ID not found in token.");

            int userId = int.Parse(userIdClaim.Value);

            var entry = _context.AddressBookEntries.FirstOrDefault(e => e.Id == entryId && e.UserId == userId);
            if (entry == null) return NotFound("Entry not found");

            _context.AddressBookEntries.Remove(entry);
            _context.SaveChanges();

            return Ok();
        }
    }
}
