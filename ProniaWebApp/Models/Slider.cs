using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required,StringLength(13,ErrorMessage ="Max Size - 10 Charachters")]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
    }
}
