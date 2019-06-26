namespace Bidster
{
    public static class Constants
    {
        // 0 = event slug --- 1 == filename
        public const string ImagePathFormat = "events/{0}/products/{1}";
    }

    public static class Policies
    {
        public const string Admin = nameof(Admin);
        public const string EventAdmin = nameof(EventAdmin);
    }

    public static class Roles
    {
        public const string Admin = nameof(Admin);
    }
}
