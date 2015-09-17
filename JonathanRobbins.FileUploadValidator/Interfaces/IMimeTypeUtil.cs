using System.Collections.Generic;
using JonathanRobbins.SecureFileUpload.Models;

namespace JonathanRobbins.SecureFileUpload.Interfaces
{
    public interface IMimeTypeUtil
    {
        /// <summary>
        /// Gets the MimeType of a file from a list of constant MimeType
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetMimeType(byte[] file, string fileName);
        /// <summary>
        /// Determines if the file's MimeType is contained within the permitted MimeTypes
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="permittedMimeTypes"></param>
        /// <returns>FileType of the file if matched</returns>
        FileType MimeTypeAllowed(byte[] file, string fileName, List<FileType> permittedMimeTypes);
        /// <summary>
        /// Creates a ByteArray from values in a Csv
        /// </summary>
        /// <param name="byteArrayString"></param>
        /// <returns>ByteArray of the values</returns>
        byte[] CsvToByteArray(string byteArrayString);
    }
}
