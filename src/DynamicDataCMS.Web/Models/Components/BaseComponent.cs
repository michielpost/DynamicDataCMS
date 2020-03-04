using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DynamicDataCms.Web.Models.Components
{
	public abstract class BaseComponent
	{
		[Display(Name = "Titel")]
		public string Title { get; set; }
	}
}
