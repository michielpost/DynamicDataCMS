using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QMS.Web.Models.Components
{
	public abstract class BaseComponent
	{
		[Display(Name = "Titel")]
		public string Title { get; set; }
	}
}
