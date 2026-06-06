using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Interfaces
{
    public interface IStorageService
    {
        /// <summary>
        /// Persists the stream and returns a root-relative path,
        /// e.g. "uploads/shops/3f2a1b.jpg"
        /// </summary>
        Task<string> SaveAsync(Stream fileStream,string fileName,string folder,CancellationToken ct = default);
    }
}
