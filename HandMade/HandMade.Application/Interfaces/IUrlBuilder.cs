using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Interfaces
{
    public interface IUrlBuilder
    {
        /// <summary>
        /// Turns a relative path like "uploads/shops/abc.jpg"
        /// into "https://yourdomain.com/uploads/shops/abc.jpg"
        /// </summary>
        string BuildAbsoluteUrl(string relativePath);
    }
}
