using DotnetApi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetAPI.Models
{
    public class AddressBookEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
       
        public string FullName { get; set; }

        [Required]
        public int JobId { get; set; } 

        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; } 

        [Required]
        public int DepartmentId { get; set; } 

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } 

        [Required]
        [Phone]
        [StringLength(15)]
        public string MobileNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Password { get; set; }

        public byte[] Photo { get; set; } 

        [NotMapped]
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        //public string PhotoBase64 => Photo != null ? Convert.ToBase64String(Photo) : null;
    }
}
