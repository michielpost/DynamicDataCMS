using System.ComponentModel.DataAnnotations;

namespace DynamicDataCMS.Web.Models.Molecules
{
	public class CarouselSlide
	{
		[Display(Name = "Slide label")]
		public string Label { get; set; }

		[Display(Name = "Slide info tekst", Description = "Deze tekst wordt alleen op desktop getoond")]
		public string InfoText { get; set; }

		[Display(Name = "Titel")]
		public string Title { get; set; }

		[Display(Name = "Subtitel")]
		public string SubTitle { get; set; }

	}
}
