﻿using Microsoft.EntityFrameworkCore;
using ProniaWebApp.Models;

namespace ProniaWebApp.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {


        }
       public DbSet<Slider>Sliders { get; set; }
       public DbSet<Product>Products { get; set; }
       public DbSet<Category>Categories { get; set; }
       public DbSet<Tag>Tags { get; set; }
       public DbSet<ProductTag> ProductTags { get; set; }
       public DbSet<ProductImg> ProductImgs { get; set; }
    }
}