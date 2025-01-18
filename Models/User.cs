using DotnetAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace DotnetApi.Models
{
    public partial class User
    {
        public int UserId { set; get; }
        public string FirstName { set; get; }="";
        public string LastName { set; get; }="";
        public string Email { set; get; }="";
        public string Gender { set; get; }="";
        public bool Active { set; get; }
        public ICollection<AddressBookEntry> AddressBookEntries { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Department> Departments { get; set; }
    }

}