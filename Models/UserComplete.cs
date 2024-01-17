namespace DotnetApi.Models
{
    public partial class UserComplete
    {
        public int UserId { set; get; }
        public string FirstName { set; get; } = "";
        public string LastName { set; get; } = "";
        public string Email { set; get; } = "";
        public string Gender { set; get; } = "";
        public bool Active { set; get; }
        public string JobTitle { set; get; } = "";
        public string Department { set; get; } = "";
        public decimal Salary { set; get; }
        public decimal AvgSalary { set; get; }
    }

}