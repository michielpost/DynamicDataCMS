using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DynamicDataCMS.Web.Models.Components
{
	[DisplayName("Pagina header")]
	public class PageHeaderComponent
	{
		[Display(Name = "Titel in plaats van afbeelding (optioneel)")]
		public string TitleInsteadOfImage { get; set; } = default!;

		[Display(Name = "Titel")]
		public string Title { get; set; } = default!;

		[Display(Name = "Intro tekst")]
		public string MainMarkdown { get; set; } = default!;
	}
}
