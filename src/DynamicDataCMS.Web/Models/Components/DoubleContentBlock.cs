using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DynamicDataCms.Web.Models.Molecules;

namespace DynamicDataCms.Web.Models.Components
{
	[DisplayName("Dubbel content blok")]
	public class DoubleContentBlock : IBaseComponent
	{
		[Display(Name = "Titel")]
		public string Title { get; set; }

		[Display(Name = "Ondertitel")]
		public string SubTitle { get; set; }
		
		[Display(Name = "Linker blok")]
		[Required]
		public ContentBlock LeftBlock { get; set; } = new ContentBlock();

		[Display(Name = "Rechter blok")]
		[Required]
		public ContentBlock RightBlock { get; set; } = new ContentBlock();
	}
}
