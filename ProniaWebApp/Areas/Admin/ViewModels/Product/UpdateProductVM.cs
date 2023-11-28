﻿namespace ProniaWebApp.Areas.Admin.ViewModels.Product
{
	public class UpdateProductVM
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string SKU { get; set; }
		public double Price { get; set; }
		public int? CategoryId { get; set; }
		public List<int>? TagIds { get; set; }


	}
}
