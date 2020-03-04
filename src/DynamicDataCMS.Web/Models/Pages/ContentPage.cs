using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DynamicDataCms.Web.Models.Components;

namespace DynamicDataCms.Web.Models.Pages
{
	[DisplayName("Contentpagina")]
	public class ContentPage : BaseCmsPage
    {
		[Display(Name = "Toon alle blokken als 1 blok")]
		public bool ShowAsOneSection { get; set; }

		[Display(Name = "Pagina header")]
		public PageHeaderComponent PageHeaderComponent { get; set; } = new PageHeaderComponent();

		[Display(Name = "Blokken")]
		public List<IBaseComponent> Components { get; set; } = new List<IBaseComponent>();
	}
}
