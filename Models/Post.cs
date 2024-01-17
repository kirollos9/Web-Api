namespace DotnetAPI.Models
{
    public partial class Post
    {
        public int PostId { set; get; }
        public int UserId { set; get; }
        public string PostTitle { set; get; }="";
        public string PostContent { set; get; }="";
        public DateTime PostCreated { set; get; }
        public DateTime PostUpdated { set; get; }
    }
}