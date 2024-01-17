namespace DotnetApi.Dtos
{
    public partial class UserToAddDto
    {
       
        public string FirstName { set; get; }="";
        public string LastName { set; get; }="";
        public string Email { set; get; }="";
        public string Gender { set; get; }="";
        public bool Active { set; get; }

    }

}