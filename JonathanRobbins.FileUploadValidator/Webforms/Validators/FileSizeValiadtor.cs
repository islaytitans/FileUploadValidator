using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Form.Core.Validators;

namespace JonathanRobbins.FileUploadValidator.Webforms.Validators
{
    public class FileSizeValiadtor : FormCustomValidator
    {
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
                    isValid = ValidateFileSize(fileUpload.PostedFile);
                }
            }

            return isValid;
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

        private int MegaBytesToBytes(int megaBytes)
        {
            return megaBytes * MegaByteByteRaio;
        }
    }
}
