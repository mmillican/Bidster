using System.IO;
using System.Threading.Tasks;

namespace Bidster.Services.FileStorage
{
    /// <summary>
    /// Interface for interacting with file stores
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Resolves the full path to a file
        /// </summary>
        /// <param name="path">Relative path, including file name</param>
        /// <returns>Resolved path to the file</returns>
        string ResolveFilePath(string path);

        /// <summary>
        /// Resolves the full URL to a file
        /// </summary>
        /// <param name="path">Relative path, including file name</param>
        /// <returns>URL to the file</returns>
        string ResolveFileUrl(string path);

        /// <summary>
        /// Saves a file
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <param name="contentType">The mime type of the file</param>
        /// <param name="stream">File stream</param>
        Task SaveFileAsync(string path, string contentType, Stream stream);

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="path">Path of file</param>
        Task DeleteFileAsync(string path);
    }
}
