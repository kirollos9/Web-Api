namespace DotnetApi.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
       
   
        public byte[] PasswordHash { set; get; }=new byte[0];
        public byte[] PasswordSalt { set; get; }=[];
      
    }

}