namespace doft.Application.Settings
{
    public class S3Settings
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
        public string ProfilePicturesFolder { get; set; } = "profile-pictures";
        public int UrlExpirationMinutes { get; set; } = 60;
    }
} 