using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using JonathanRobbins.FileUploadValidator.Enums.IO;
using JonathanRobbins.FileUploadValidator.Interfaces;
using JonathanRobbins.FileUploadValidator.IO;
using JonathanRobbins.FileUploadValidator.Models;
using JonathanRobbins.FileUploadValidator.Webforms.Controls;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Form.Core.Validators;

namespace JonathanRobbins.FileUploadValidator.Webforms.Validators
{
    public class FileUploadValidator : FormCustomValidator
    {
        private IMimeTypeUtil _mimeTypeUtil = new MimeTypeUtil();
        private int MegaByteByteRaio = 1048576;

        private Item _fieldItem;
        private Item FieldItem
        {
            get
            {
                if (_fieldItem == null)
                {
                    string fieldId = this.classAttributes["fieldid"];

                    if (!string.IsNullOrEmpty(fieldId))
                    {
                        _fieldItem = Sitecore.Context.Database.GetItem(fieldId);
                    }
                }

                return _fieldItem;
            }
        }

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

            bool validSize = ValidateFileSize(fileUploaded);
            if (!validSize) return false;

            return true;
        }

        private bool ValidateMimeType(HttpPostedFile postedFile)
        {
            bool valid = false;

            try
            {
                var permittedMimeTypes = DeterminePermitedMimeTypes();

                FileType uploadedFileType = _mimeTypeUtil.MimeTypeAllowed(ReadFully(postedFile.InputStream), postedFile.FileName, permittedMimeTypes);

                valid = uploadedFileType != null;
            }
            catch (Exception ex)
            {
                valid = false;
                Log.Error("Error occurred when determining mime type of file", ex, this);
                throw;
            }

            return valid;
        }

        private List<FileType> DeterminePermitedMimeTypes()
        {
            var fileTypes = new List<FileType>();

            if (FieldItem != null)
            {
                var itemIds = new List<string>();

                var regexSelectedValue = new Regex(@"<selectedvalue>(.*?)</selectedvalue>".ToLower());
                var selectedValueNodes = regexSelectedValue.Matches(FieldItem["Parameters"].ToLower());

                if (selectedValueNodes.Count > 0)
                {
                    foreach (Match selectedValueNode in selectedValueNodes)
                    {
                        var regexItems = new Regex(@"<item>(.*?)</item>".ToLower());
                        var itemNodes = regexItems.Matches(selectedValueNode.ToString());

                        itemIds.AddRange(from Match itemNode in itemNodes
                                         where itemNode.Groups[1] != null 
                                         && !string.IsNullOrEmpty(itemNode.Groups[1].Value) 
                                         && TryParseGuid(itemNode.Groups[1].Value)
                                         select itemNode.Groups[1].Value);
                    }

                    if (itemIds.Any())
                    {
                        var fileTypeItems = itemIds.Select(i => Sitecore.Context.Database.GetItem(i));
                        fileTypes = fileTypeItems.Select(i => new FileType(i)).ToList();
                    }
                }
            }

            return fileTypes;
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
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

        private bool ValidateFileSize(HttpPostedFile postedFile)
        {
            int fileSizeLimitinBytes = DetermineFileSizeLimit();

            int sizeInBytes = postedFile.ContentLength;

            return (sizeInBytes <= fileSizeLimitinBytes);
        }

        private int DetermineFileSizeLimit()
        {
            int fileSizeLimitinBytes = 0;

            var regexfileSizeLimit = new Regex(@"<filesizelimit>(.*?)</filesizelimit>".ToLower());
            var fileSizeLimitNodes = regexfileSizeLimit.Matches(FieldItem["Parameters"].ToLower());

            if (fileSizeLimitNodes.Count > 0)
            {
                double intOut = 0;
                double fileSizeLimitinMegaBytes = 0;
                fileSizeLimitinMegaBytes = (from Match limit in fileSizeLimitNodes
                    where limit.Groups[1] != null 
                    && !string.IsNullOrEmpty(limit.Groups[1].Value)
                    && double.TryParse(limit.Groups[1].Value, out intOut)
                    select int.Parse((limit.Groups[1].Value))).FirstOrDefault();

                fileSizeLimitinBytes = MegaBytesToBytes(Convert.ToInt32(fileSizeLimitinMegaBytes));
            }

            return fileSizeLimitinBytes;
        }

        private bool TryParseGuid(string guidString)
        {
            if (guidString == null)
                throw new ArgumentNullException("guidString");

            try
            {
                var guid = new Guid(guidString);
                return true;
            }
            catch (FormatException)
            {
                throw new Exception("Failed to parse the Guid of File Type Item - ensure the Value field of the Items property of the SecureFileUpload WFFM Field is set to __Id");
            }
        }

        private int MegaBytesToBytes(int megaBytes)
        {
            return megaBytes*MegaByteByteRaio;
        }
    }
}
