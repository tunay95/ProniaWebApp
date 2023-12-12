﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;

namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class CategoryController : Controller
    {
        AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles ="Admin,Moderator")]
        public IActionResult Index()
        {
            List<Category>categories= _context.Categories.Include(p=>p.Products).ToList();
            return View(categories);
        }
        
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Admin,Moderator")]
        public IActionResult Create(Category category) 
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index","Category");
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Delete(int id) 
        {
            Category category=_context.Categories.Find(id);
            _context.Categories.Remove(category);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Category category = _context.Categories.Find(id);
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category oldCategory = _context.Categories.Find(newCategory.Id);
            oldCategory.Name = newCategory.Name;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public string GetString()
        {
            return "";
        }
    }
}
