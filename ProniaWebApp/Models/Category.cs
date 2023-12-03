using System.ComponentModel.DataAnnotations;

namespace ProniaWebApp.Models
{
    public class Category:BaseEntity
    {
        [StringLength(maximumLength:10,ErrorMessage =" Limit has benn exceeted ")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }

    }
}
