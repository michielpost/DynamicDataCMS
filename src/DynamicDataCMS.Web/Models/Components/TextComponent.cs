using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DynamicDataCMS.Web.Models.Components
{
	[DisplayName("Tekst component")]
	public class TextComponent : BaseComponent
	{
		[Display(Name = "Tekst")]
		[Required]
		[UIHint("markdown")]
		public string Text { get; set; }

		[Display(Name = "Grootte")]
		public Size Size { get; set; }
	
	}

	public enum Size
	{
		[Display(Name = "Normaal")]
		Normal,
		[Display(Name = "Klein")]
		Small,
		[Display(Name = "Groot")]
		Large
	}
}
