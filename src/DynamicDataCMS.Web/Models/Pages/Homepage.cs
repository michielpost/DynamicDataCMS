using DynamicDataCms.Web.Models.Components;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DynamicDataCms.Web.Models.Pages
{
	[DisplayName("Homepage")]
	public class HomePage : BaseCmsPage
    {
        [Required]
        [Display(Name = "Pagina header")]
        public HomepageHeaderComponent HomepageHeaderComponent { get; set; }

        [Display(Name = "Carousel")]
        public CarouselComponent CarouselComponent { get; set; }
    }

}
