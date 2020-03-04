using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DynamicDataCMS.Web.Models.Molecules;

namespace DynamicDataCMS.Web.Models.Components
{
	public class CarouselComponent
	{
		[Display(Name = "Aantal seconde tot volgende slide")]
		[Range(5, 60)]
		[DefaultValue(5)]
		public int AmountOfSecondsTillNextSlide { get; set; }

		[Display(Name = "Carrousel", Description = "Vul minimaal twee items in")]
		public List<CarouselSlide> Slides { get; set; } = new List<CarouselSlide>();
	}
}
