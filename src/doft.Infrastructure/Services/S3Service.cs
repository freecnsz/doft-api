using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace doft.Infrastructure.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _settings;
        private readonly ILogger<S3Service> _logger;

        public S3Service(IOptions<S3Settings> settings, ILogger<S3Service> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
            };

            _s3Client = new AmazonS3Client(
                _settings.AccessKey,
                _settings.SecretKey,
                config
            );
        }

        public async Task<string> UploadProfilePictureAsync(string userId, Stream imageStream)
        {
            try
            {
                var key = $"{_settings.ProfilePicturesFolder}/{userId}";

                // Check if file exists and delete it
                try
                {
                    await _s3Client.DeleteObjectAsync(_settings.BucketName, key);
                }
                catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // File doesn't exist, which is fine
                }

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = imageStream,
                    Key = key,
                    BucketName = _settings.BucketName,
                };

                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);

                _logger.LogInformation("Profile picture uploaded successfully for user {UserId}", userId);
                
                var url = await GetProfilePictureUrlAsync(userId);
                if (url == null)
                {
                    throw new InvalidOperationException("Failed to get profile picture URL after upload");
                }
                
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", userId);
                throw;
            }
        }

        public async Task<string?> GetProfilePictureUrlAsync(string userId)
        {
            try
            {
                var key = $"{_settings.ProfilePicturesFolder}/{userId}";

                // Check if the object exists
                try
                {
                    await _s3Client.GetObjectMetadataAsync(_settings.BucketName, key);
                }
                catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Profile picture not found for user {UserId}", userId);
                    return null; // Return null if the profile picture does not exist
                }

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _settings.BucketName,
                    Key = key,
                    Expires = DateTime.UtcNow.AddMinutes(_settings.UrlExpirationMinutes)
                };

                var url = _s3Client.GetPreSignedURL(request);
                _logger.LogInformation("Generated pre-signed URL for user {UserId}", userId);
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating pre-signed URL for user {UserId}", userId);
                throw;
            }
        }

        public async Task DeleteProfilePictureAsync(string userId)
        {
            try
            {
                var key = $"{_settings.ProfilePicturesFolder}/{userId}";

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
                _logger.LogInformation("Profile picture deleted successfully for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile picture for user {UserId}", userId);
                throw;
            }
        }
    }
} 