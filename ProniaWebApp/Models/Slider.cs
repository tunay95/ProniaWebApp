using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProniaWebApp.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required,StringLength(20,ErrorMessage ="Max Size - 20 Charachters")]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string? ImgUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
