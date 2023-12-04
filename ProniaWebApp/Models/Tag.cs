namespace ProniaWebApp.Models
{
    public class Tag:BaseEntity
    {
        public string Name { get; set; }
        public List<ProductTag> ProductsTags { get; set; }
    }
}
