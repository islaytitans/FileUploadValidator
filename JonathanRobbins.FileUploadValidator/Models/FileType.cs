using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JonathanRobbins.FileUploadValidator.Enums;
using JonathanRobbins.FileUploadValidator.Interfaces;
using JonathanRobbins.FileUploadValidator.IO;
using Sitecore.Data.Items;

namespace JonathanRobbins.FileUploadValidator.Models
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
            if (item == null)
                return;

            IMimeTypeUtil mimeTypeUtil = new MimeTypeUtil();

            MimeType = item[FieldNames.MimeType];
            FileExtension = item[FieldNames.FileExtension];
            ByteArray = mimeTypeUtil.CsvToByteArray(item[FieldNames.ByteArray]);
        }


    }
}
