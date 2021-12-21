using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DynamicDataCMS.Web.Models.Molecules
{
	[DisplayName("Content blok")]
	public class ContentBlock
	{
		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; } = default!;

		[Display(Name = "Ondertitel")]
		public string SubTitle { get; set; } = default!;

		[Display(Name = "Tekst")]
		public string Text { get; set; } = default!;

		[Display(Name = "Afbeelding (16x9)")]
		[UIHint("asset-picker")]
		public Guid? ImageId { get; set; }

	}
}
