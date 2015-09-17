using JonathanRobbins.SecureFileUpload.Enums;
using JonathanRobbins.SecureFileUpload.Interfaces;
using JonathanRobbins.SecureFileUpload.IO;
using Sitecore.Data.Items;

namespace JonathanRobbins.SecureFileUpload.Models
{
    public class FileType
    {
        public string MimeType { get; set; }
        public byte[] ByteArray { get; set; }
        public string FileExtension { get; set; }

        public FileType()
        {
            
        }

        public FileType(string mimeType, byte[] byteArray, string fileExtension = "")
        {
            IMimeTypeUtil mimeTypeUtil = new MimeTypeUtil();

            MimeType = mimeType;
            FileExtension = fileExtension;
            ByteArray = byteArray;
        }

        public FileType(Item item)
        {
            if (item == null 
                || string.IsNullOrEmpty(item[FieldNames.MimeType]) 
                || string.IsNullOrEmpty(item[FieldNames.ByteArray]))
                return;

            IMimeTypeUtil mimeTypeUtil = new MimeTypeUtil();

            MimeType = item[FieldNames.MimeType];
            FileExtension = item[FieldNames.FileExtension];
            ByteArray = mimeTypeUtil.CsvToByteArray(item[FieldNames.ByteArray]);
        }


    }
}
