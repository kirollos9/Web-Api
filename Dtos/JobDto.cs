using System.ComponentModel.DataAnnotations;

namespace DotnetApi.Dtos
{
    public class JobDto
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
    }
}
