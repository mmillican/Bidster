namespace Bidster.Entities.Tenants
{
    public class TenantSettings
    {
        public BrandingSettings Branding { get; set; }
    }

    public class BrandingSettings
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public string Description { get; set; }
    }
}
