namespace DotnetApi.Dtos
{
    public partial class UserForRegistrationDto
    {


        public string Email { set; get; } = "";
        public string Password { set; get; } = "";
        public string PasswordConfirm { set; get; } = "";
        public string FirstName { set; get; } = "";
        public string LastName { set; get; } = "";

        public string Gender { set; get; } = "";
       


    }

}