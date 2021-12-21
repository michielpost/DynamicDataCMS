using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Web.Models.Pages
{
    public abstract class BaseCmsPage
    {
        [Display(Name = "Titel")]
        [Required]
        [MinLength(2, ErrorMessage = "Vul minimaal 2 karakters in")]
        public string MetaTitle { get; set; } = default!;

        [Display(Name = "Postfix tonen", Description = "De postfix wordt achter de titel toegevoegd, bijvoorbeeld '- Foo Bar'")]
        public bool ShowPostFix { get; set; } = true;

        [Display(Name = "Omschrijving")]
        [UIHint("textarea")]
        [MaxLength(300, ErrorMessage = "Een omschrijving langer dan 300 tekens is niet mogelijk in Google")]
        public string MetaDescription { get; set; } = default!;

        [Display(Name = "Indexeren", Description = "Indexeren betekent dat de pagina in zoekmachines (zoals Google) wordt opgenomen.")]
        public bool MetaIndex { get; set; } = true;

        [Display(Name = "Follow", Description = "Follow betekent dat links op de pagina gevolgd worden tijdens het indexeren door een zoekmachine.")]
        public bool MetaFollow { get; set; } = true;

        [Url]
        [Display(Name = "Canonical", Description = "Geeft aan zoekmachines aan welke url voor deze content de voorkeur heeft.")]
        public string Canonical { get; set; } = default!;

        public DateTimeOffset? PublishDate { get; set; }

        public DateTimeOffset? DepublishDate { get; set; }

    }
}
