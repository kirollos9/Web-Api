namespace DotnetAPI.Dtos
{
    public partial class PostToEditDto
    {
        public int PostId { set; get; }
        public string PostTitle { set; get; }="";
        public string PostContent { set; get; }="";
      
    }
}