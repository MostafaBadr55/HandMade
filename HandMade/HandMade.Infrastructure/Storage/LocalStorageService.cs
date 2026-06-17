using HandMade.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Infrastructure.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _webRootPath;

        public LocalStorageService(IWebHostEnvironment env)
        {
            _webRootPath = env.WebRootPath;
        }

        public async Task<string> SaveFileAsync(Stream fileStream,string fileName,string folder,
            CancellationToken ct = default)
        {
            string absoluteFolder = Path.Combine(_webRootPath, folder);
            Directory.CreateDirectory(absoluteFolder);

            string absolutePath = Path.Combine(absoluteFolder, fileName);

            await using var fileOnDisk = new FileStream(absolutePath, FileMode.Create);
            await fileStream.CopyToAsync(fileOnDisk, ct);

            // Always return forward-slash relative path
            return $"{folder}/{fileName}";
        }

        public Task<bool> DeleteAsync(string relativePath, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_webRootPath, relativePath);

            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            File.Delete(fullPath);
            return Task.FromResult(true);
        }
    }
}
