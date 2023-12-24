using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Renci.SshNet;
using Microsoft.Extensions.Logging;

namespace DSP.Service
{
    public class FileTransferService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FileTransferService> _logger;
        public FileTransferService(HttpClient httpClient, ILogger<FileTransferService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> DownloadAndUploadImageAsync(string imageUrl, string sftpServer, string username, string password)
        {
            _logger.LogInformation($"Attempting to download image from: {imageUrl}");
            var fileData = await DownloadImageAsync(imageUrl);

            if (fileData != null)
            {
                string targetDirectory = Path.GetExtension(imageUrl).ToLower() == ".mp4" ? "public/267/" : "public/266/";
                string fileName = Path.GetFileName(imageUrl);

                await UploadToSftpAsync(fileData, fileName, sftpServer, username, password, targetDirectory);

                
                return imageUrl.EndsWith(".mp4") ? $"https://img.sp.com/267/{fileName}" : $"/266/{fileName}";
            }
            _logger.LogError("Failed to download image");
            return null;
        }

        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                return await _httpClient.GetByteArrayAsync(imageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading the image or uploading to SFTP.");
                return null;
            }
        }

        private async Task UploadToSftpAsync(byte[] data, string fileName, string sftpServer, string username, string password, string targetDirectory)
        {
            using (var sftp = new SftpClient(sftpServer, username, password))
            {
                try
                {
                    sftp.Connect();
                    using (var ms = new MemoryStream(data))
                    {
                        sftp.UploadFile(ms, Path.Combine(targetDirectory, fileName));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while uploading to SFTP.");
                }
                finally
                {
                    if (sftp.IsConnected)
                    {
                        sftp.Disconnect();
                    }
                }
            }
        }
    }
}
