namespace Bidster.Configuration
{
    public class FileStorageConfig
    {
        public FileStorageConfig StorageType { get; set; }

        public AmazonS3StorageConfig S3Config { get; set; }
    }

    public enum FileStorageType
    {
        //Local = 1,
        AmazonS3 = 2
    }

    public class AmazonS3StorageConfig
    {
        public string BucketName { get; set; }

        public string ProductImageFilePrefix { get; set; }
    }
}
