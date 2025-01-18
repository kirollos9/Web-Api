namespace DotnetApi.Dtos
{
    public class AddressBookEntryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int JobId { get; set; }
        public int DepartmentId { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[]? Photo { get; set; }
        public string PhotoStr { get; set; }
    }
}
