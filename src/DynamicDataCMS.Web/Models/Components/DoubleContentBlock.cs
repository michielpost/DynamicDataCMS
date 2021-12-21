using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DynamicDataCMS.Web.Models.Molecules;

namespace DynamicDataCMS.Web.Models.Components
{
	[DisplayName("Dubbel content blok")]
	public class DoubleContentBlock : IBaseComponent
	{
		[Display(Name = "Titel")]
		public string Title { get; set; } = default!;

		[Display(Name = "Ondertitel")]
		public string SubTitle { get; set; } = default!;

		[Display(Name = "Linker blok")]
		[Required]
		public ContentBlock LeftBlock { get; set; } = new ContentBlock();

		[Display(Name = "Rechter blok")]
		[Required]
		public ContentBlock RightBlock { get; set; } = new ContentBlock();
	}
}
