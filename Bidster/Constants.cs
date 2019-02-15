namespace Bidster
{
    public static class Constants
    {
        // 0 = event slug --- 1 == filename
        public const string ImagePathFormat = "events/{0}/products/{1}";
    }

    public static class CacheKeys
    {
        public const string TenantById = "bidster.tenant.id-{0}";
        public const string TenantByHost = "bidster.tenant.host-{0}";
        public const string TenantSettings = "bidster.tenant.settings-{0}";
    }
}
