using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using JonathanRobbins.FileUploadValidator.Enums.IO;
using JonathanRobbins.FileUploadValidator.IO;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Form.Core.Validators;

namespace JonathanRobbins.FileUploadValidator.Webforms.Validators
{
    public class FileUploadValidator : FormCustomValidator
    {
        protected override bool EvaluateIsValid()
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(base.ControlToValidate))
            {
                Control control = this.FindControl(base.ControlToValidate);
                var fileUpload = control as FileUpload;
                if (fileUpload != null && fileUpload.HasFile)
                {
                    isValid = ValidateFile(fileUpload.PostedFile);
                }
            }

            return isValid;
        }

        // Custom function which calls the various stages of validation
        private bool ValidateFile(HttpPostedFile fileUploaded)
        {
            bool validMime = ValidateMimeType(fileUploaded);
            if (!validMime) return false;

            //bool validSize = ValidateFileSize(fileUploaded);
            //if (!validSize) return false;

            //Branch commit test

            return true;
        }

        private bool ValidateMimeType(HttpPostedFile postedFile)
        {
            bool valid = false;

            try
            {
                Stream stream = postedFile.InputStream;

                var mimeTypeUtil = new MimeTypeUtil();
                string mime = mimeTypeUtil.GetMimeType(ReadFully(stream), postedFile.FileName);

                valid = PermittedMimeTypes.Contains(mime);
            }
            catch (Exception ex)
            {
                valid = false;
                Log.Error("Error occurred when determining mime type of file", ex, this);
                throw;
            }

            return valid;
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private IEnumerable<string> _permittedMimeTypes;

        private IEnumerable<string> PermittedMimeTypes
        {
            get
            {
                if (_permittedMimeTypes == null)
                {
                    _permittedMimeTypes = new List<string>()
                    {
                        MimeType.ImageBmp,
                        MimeType.ImageGif,
                        MimeType.ImageJpeg,
                        MimeType.ImagePng,
                    };
                }

                return _permittedMimeTypes;
            }
        }

        private const int DefaultFileSizeLimit = 3000000;

        //private int? _fileSizeLimitInBytes;
        //private int FileSizeLimitInBytes
        //{
        //    get
        //    {
        //        if (_fileSizeLimitInBytes == null || _fileSizeLimitInBytes == DefaultFileSizeLimit)
        //        {
        //            _fileSizeLimitInBytes = DefaultFileSizeLimit;

        //            Item fileUploadConfig = ItemNodes.SiteConfig.Children.FirstOrDefault(x => x.TemplateID == Enumerators.SitecoreConfig.Guids.Templates.FileUploadConfigId);
        //            if (fileUploadConfig != null && fileUploadConfig.Fields[Enumerators.SitecoreConfig.Fields.Global.ImageFileSizeLimit] != null
        //                && !string.IsNullOrEmpty(fileUploadConfig[Enumerators.SitecoreConfig.Fields.Global.ImageFileSizeLimit]))
        //            {
        //                int sizeInMegaBytes;
        //                bool success =
        //                    int.TryParse(fileUploadConfig[Enumerators.SitecoreConfig.Fields.Global.ImageFileSizeLimit],
        //                        out sizeInMegaBytes);

        //                if (success)
        //                    _fileSizeLimitInBytes = sizeInMegaBytes * 1000000;

        //            }
        //        }

        //        return _fileSizeLimitInBytes.Value;
        //    }
        //}

        //private bool ValidateFileSize(HttpPostedFile postedFile)
        //{
        //    var sizeInBytes = postedFile.ContentLength;

        //    return (sizeInBytes <= FileSizeLimitInBytes);
        //}
    }
}
