using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QMS.Web.Models.Molecules
{
	[DisplayName("Content blok")]
	public class ContentBlock
	{
		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; }

		[Display(Name = "Ondertitel")]
		public string SubTitle { get; set; }

		[Display(Name = "Tekst")]
		public string Text { get; set; }

		[Display(Name = "Afbeelding (16x9)")]
		[UIHint("asset-picker")]
		public Guid? ImageId { get; set; }

	}
}
