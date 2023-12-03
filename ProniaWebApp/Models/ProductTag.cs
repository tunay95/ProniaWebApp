namespace ProniaWebApp.Models
{
    public class ProductTag:BaseEntity
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int TagId { get; set; }
        public Product Product { get; set; }
        public Tag Tag { get; set; }
    }
}
