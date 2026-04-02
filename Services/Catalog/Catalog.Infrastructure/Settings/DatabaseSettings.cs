namespace Catalog.Infrastructure.Settings
{
    // we are getting from Appsetting
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string BrandCollectionName { get; set; }
        public string TypeCollectionName { get; set; }
        public string ProductCollectionName { get; set; }
    }
}
