using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JonathanRobbins.FileUploadValidator.Enums.IO;
using JonathanRobbins.FileUploadValidator.Interfaces;
using JonathanRobbins.FileUploadValidator.Models;
using Sitecore.Diagnostics;

namespace JonathanRobbins.FileUploadValidator.IO
{
    public class MimeTypeUtil : IMimeTypeUtil
    {
        private readonly byte[] BMP = { 66, 77 };
        private readonly byte[] DOC = { 208, 207, 17, 224, 161, 177, 26, 225 };
        private readonly byte[] EXE_DLL = { 77, 90 };
        private readonly byte[] GIF = { 71, 73, 70, 56 };
        private readonly byte[] ICO = { 0, 0, 1, 0 };
        private readonly byte[] JPG = { 255, 216, 255 };
        private readonly byte[] MP3 = { 255, 251, 48 };
        private readonly byte[] OGG = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        private readonly byte[] PDF = { 37, 80, 68, 70, 45, 49, 46 };
        private readonly byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
        private readonly byte[] RAR = { 82, 97, 114, 33, 26, 7, 0 };
        private readonly byte[] SWF = { 70, 87, 83 };
        private readonly byte[] TIFF = { 73, 73, 42, 0 };
        private readonly byte[] TORRENT = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
        private readonly byte[] TTF = { 0, 1, 0, 0, 0 };
        private readonly byte[] WAV_AVI = { 82, 73, 70, 70 };
        private readonly byte[] WMV_WMA = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };
        private readonly byte[] ZIP_DOCX = { 80, 75, 3, 4 };

        public string GetMimeType(byte[] file, string fileName)
        {
            Assert.IsNotNull(file, "the file can not be null");
            Assert.IsNotNullOrEmpty(fileName, "the fileName can not be null");

            string mime = MimeType.DefaultUnknown;
            string extension = Path.GetExtension(fileName) != null
                                   ? Path.GetExtension(fileName).ToUpper()
                                   : string.Empty;
 
            if (file.Take(2).SequenceEqual(BMP))
            {
                mime = MimeType.ImageBmp;
            }
            else if (file.Take(8).SequenceEqual(DOC))
            {
                mime = MimeType.ApplicationMsWord;
            }
            else if (file.Take(2).SequenceEqual(EXE_DLL))
            {
                mime = MimeType.ApplicationXMsDownload;
            }
            else if (file.Take(4).SequenceEqual(GIF))
            {
                mime = MimeType.ImageGif;
            }
            else if (file.Take(4).SequenceEqual(ICO))
            {
                mime = MimeType.ImageXIcon;
            }
            else if (file.Take(3).SequenceEqual(JPG))
            {
                mime = MimeType.ImageJpeg;
            }
            else if (file.Take(3).SequenceEqual(MP3))
            {
                mime = MimeType.AudioMPeg;
            }
            else if (file.Take(14).SequenceEqual(OGG))
            {
                if (extension == ".OGX")
                {
                    mime = MimeType.ApplicationOgg;
                }
                else if (extension == ".OGA")
                {
                    mime = MimeType.AudioOgg;
                }
                else
                {
                    mime = MimeType.VideoOgg;
                }
            }
            else if (file.Take(7).SequenceEqual(PDF))
            {
                mime = MimeType.ApplicationPdf;
            }
            else if (file.Take(16).SequenceEqual(PNG))
            {
                mime = MimeType.ImagePng;
            }
            else if (file.Take(7).SequenceEqual(RAR))
            {
                mime = MimeType.ApplicationXRarCompressed;
            }
            else if (file.Take(3).SequenceEqual(SWF))
            {
                mime = MimeType.ApplicationXShockwaveFlash;
            }
            else if (file.Take(4).SequenceEqual(TIFF))
            {
                mime = MimeType.ImageTiff;
            }
            else if (file.Take(11).SequenceEqual(TORRENT))
            {
                mime = MimeType.ApplicationXBittorrent;
            }
            else if (file.Take(5).SequenceEqual(TTF))
            {
                mime = MimeType.ApplicationXFontTtf;
            }
            else if (file.Take(4).SequenceEqual(WAV_AVI))
            {
                mime = extension == ".AVI" ? MimeType.VideoXMsVideo : MimeType.AudioXWav;
            }
            else if (file.Take(16).SequenceEqual(WMV_WMA))
            {
                mime = extension == ".WMA" ? MimeType.AudioXMsWma : MimeType.VideoxMsWmv;
            }
            else if (file.Take(4).SequenceEqual(ZIP_DOCX))
            {
                mime = extension == ".DOCX" ? MimeType.ApplicationDocx : MimeType.ApplicationXZipCompressed;
            }
 
            return mime;
        }

        public FileType MimeTypeAllowed(byte[] file, string fileName, List<FileType> permittedMimeTypes)
        {
            FileType uploadedFileType = null;

            string extension = Path.GetExtension(fileName) != null
                                   ? Path.GetExtension(fileName).ToUpper()
                                   : string.Empty;

            var matchedByByte = new List<FileType>();
            var matchedByByteAndExtension = new List<FileType>();

            foreach (var permittedMimeType in permittedMimeTypes)
            {
                int byteCount = permittedMimeType.ByteArray.Count();

                if (file.Take(byteCount).SequenceEqual(permittedMimeType.ByteArray))
                {
                    if (!string.IsNullOrEmpty(permittedMimeType.FileExtension))
                    {
                        if (permittedMimeType.FileExtension.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                        {
                            matchedByByteAndExtension.Add(new FileType(permittedMimeType.MimeType, file, extension));
                        }
                    }
                    else
                    {
                        matchedByByte.Add(new FileType(permittedMimeType.MimeType, file, extension));
                    }
                }
            }

            uploadedFileType = matchedByByteAndExtension.Any()
                ? matchedByByteAndExtension.FirstOrDefault()
                : matchedByByte.Any() ? matchedByByte.FirstOrDefault() : null;

            return uploadedFileType;
        }

        public byte[] CsvToByteArray(string byteArrayString)
        {
            int[] intArray = byteArrayString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var byteArray = new byte[intArray.Length];
            int j = 0;
            foreach (var i in intArray)
            {
                byteArray[j] = (byte)i;
                j++;
            }

            return byteArray;
        }
    }
}
