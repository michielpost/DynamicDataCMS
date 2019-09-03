using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Web.Models.Components
{
	//[Display(Name = ("Header"))]
	public class HomepageHeaderComponent
	{
		[Display(Name = "Product")]
		[Required]
		public Guid ProductId { get; set; }
		
		[Display(Name = "Titel")]
		public string Title { get; set; }

		[Display(Name = "Subtitel")]
		public string SubTitle { get; set; }
		
	}
}
