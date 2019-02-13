using System.ComponentModel.DataAnnotations;

namespace Bidster.Models.Tenants
{
    public class TenantModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "Host name(s)")]
        [Required, MaxLength(255)]
        public string HostNames { get; set; }

        public bool IsDisabled { get; set; }
    }
}
