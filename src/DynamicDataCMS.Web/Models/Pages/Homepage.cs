using DynamicDataCMS.Web.Models.Components;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DynamicDataCMS.Web.Models.Pages
{
	[DisplayName("Homepage")]
	public class HomePage : BaseCmsPage
    {
        [Required]
        [Display(Name = "Pagina header")]
        public HomepageHeaderComponent HomepageHeaderComponent { get; set; } = default!;

        [Display(Name = "Carousel")]
        public CarouselComponent CarouselComponent { get; set; } = default!;
    }

}
