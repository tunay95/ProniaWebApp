﻿namespace ProniaWebApp.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public double Price { get; set; }
        public bool IsDeleted { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category  { get; set; }
        public List<ProductTag>? ProductsTags { get; set; }
        public List<ProductImg>? ProductImages { get; set; }

    }
}
