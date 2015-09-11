using Sitecore.Form.Core.Attributes;
using Sitecore.Form.Core.Configuration;
using Sitecore.Form.Core.Controls.Data;
using Sitecore.Form.Core.Visual;
using Sitecore.Form.UI.Adapters;
using Sitecore.Form.UI.Converters;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Form.Core.Media;
using Sitecore.Form.Web.UI.Controls;
using ListItemCollection = System.Web.UI.WebControls.ListItemCollection;

namespace JonathanRobbins.FileUploadValidator.Webforms.Controls
{
    [Adapter(typeof(FileUploadAdapter))]
    public class SecureFileUpload : List
    {
        private static readonly string baseCssClassName = "scfFileUploadBorder";
        protected Panel generalPanel = new Panel();
        protected System.Web.UI.WebControls.Label title = new System.Web.UI.WebControls.Label();
        protected FileUpload upload = new FileUpload();
        private string uploadDir = "/sitecore/media library";

        public override string ID
        {
            get { return this.upload.ID; }
            set
            {
                this.title.ID = value + "text";
                this.upload.ID = value;
                base.ID = value + "scope";
            }
        }

        [VisualFieldType(typeof(SelectDirectoryField))]
        [VisualProperty("Upload To:", 0)]
        [DefaultValue("/sitecore/media library")]
        [VisualCategory("Upload")]
        public string UploadTo
        {
            get { return this.uploadDir; }
            set { this.uploadDir = value; }
        }

        public override ControlResult Result
        {
            get
            {
                if (this.upload.HasFile)
                    return new ControlResult(this.ControlName,
                        (object)new PostedFile(this.upload.FileBytes, this.upload.FileName, this.UploadTo), "medialink");
                return new ControlResult(this.ControlName, (object)null, string.Empty);
            }
            set { }
        }

        public string Title
        {
            get { return this.title.Text; }
            set { this.title.Text = value; }
        }

        [VisualProperty("CSS Class:", 600)]
        [VisualFieldType(typeof(CssClassField))]
        [DefaultValue("scfFileUploadBorder")]
        public new string CssClass
        {
            get { return base.CssClass; }
            set { base.CssClass = value; }
        }

        protected override Control ValidatorContainer
        {
            get { return (Control)this; }
        }

        protected override Control InnerValidatorContainer
        {
            get { return (Control)this.generalPanel; }
        }

        public SecureFileUpload2(HtmlTextWriterTag tag)
            : base()
        {
            this.CssClass = SecureFileUpload2.baseCssClassName;
        }

        public SecureFileUpload2()
            : this(HtmlTextWriterTag.Div)
        {
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            this.DoRender(writer);
        }

        protected virtual void DoRender(HtmlTextWriter writer)
        {
            base.RenderControl(writer);
        }

        protected override void OnInit(EventArgs e)
        {
            this.upload.CssClass = "scfFileUpload";
            this.help.CssClass = "scfFileUploadUsefulInfo";
            this.title.CssClass = "scfFileUploadLabel";
            this.title.AssociatedControlID = this.upload.ID;
            this.generalPanel.CssClass = "scfFileUploadGeneralPanel";
            this.Controls.AddAt(0, (Control)this.generalPanel);
            this.Controls.AddAt(0, (Control)this.title);
            this.generalPanel.Controls.AddAt(0, (Control)this.help);
            this.generalPanel.Controls.AddAt(0, (Control)this.upload);
        }
    }
}