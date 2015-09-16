using Sitecore.Form.Core.Attributes;
using Sitecore.Form.Core.Configuration;
using Sitecore.Form.Core.Controls.Data;
using Sitecore.Form.Core.Visual;
using Sitecore.Form.UI.Adapters;
using Sitecore.Form.UI.Converters;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Form.Core.Media;
using Sitecore.Form.Web.UI.Controls;
using ListItemCollection = System.Web.UI.WebControls.ListItemCollection;

namespace JonathanRobbins.FileUploadValidator.Webforms.Controls
{
    //[Adapter(typeof(ListControlAdapter))]
    [ListAdapter("Items", typeof(ListItemsAdapter))]
    [Adapter(typeof(FileUploadAdapter))]
    public class SecureFileUpload : ValidateControl, IHasTitle
    {
        private static readonly string baseCssClassName = "scfFileUploadBorder";
        protected Panel generalPanel = new Panel();
        protected System.Web.UI.WebControls.Label title = new System.Web.UI.WebControls.Label();
        protected FileUpload upload = new FileUpload();
        private int _fileSizeLimit;
        private ListItemCollection _items;
        private ListItemCollection _selectedItems;
        private string _uploadDir = "/sitecore/media library";

        //protected System.Web.UI.WebControls.ListControl InnerListControl
        //{
        //    get { throw new NotImplementedException(); }
        //}

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
            get { return this._uploadDir; }
            set { this._uploadDir = value; }
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

        public SecureFileUpload(HtmlTextWriterTag tag)
            : base()
        {
            this.CssClass = SecureFileUpload.baseCssClassName;
        }

        public SecureFileUpload()
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
            this.InitItems(this._items);
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        [VisualCategory("List")]
        [VisualFieldType(typeof(ListField))]
        [TypeConverter(typeof(ListItemCollectionConverter))]
        [DefaultValue("%3cquery+t%3d%22fast%22%3e%3cvalue%3e%2fsitecore%2fsystem%2fModules%2fWeb+Forms+for+Marketers%2fSettings%2fMeta+data%2fFile+Types%2f*%3c%2fvalue%3e%3c%2fquery%3e")]
        [Description("Collection of items.")]
        [VisualProperty("Items:", 100)]
        public ListItemCollection Items
        {
            get
            {
                return this._items;
            }
            set
            {
                this._items = value;
            }
        }

        [TypeConverter(typeof(ListItemCollectionConverter))]
        [VisualFieldType(typeof(MultipleSelectedValueField))]
        [VisualCategory("List")]
        [Browsable(false)]
        [VisualProperty("Selected Values:", 450)]
        public ListItemCollection SelectedValue
        {
            get
            {
                return this._selectedItems;
            }
            set
            {
                this._selectedItems = value;
            }
        }

        public override string DefaultValue
        {
            set
            {
                //this.SelectedValue = (ListItemCollection)value;
            }
        }

        protected virtual void InitItems(ListItemCollection collection)
        {
            if (collection == null)
                collection = new ListItemCollection();
            //this.InnerListControl.Items.Clear();
            //this.InnerListControl.Items.AddRange(collection.Cast<ListItem>().ToArray());
            if (this._selectedItems == null)
                return;
            foreach (ListItem listItem in this._selectedItems)
            {
                //ListItem byValue = this.InnerListControl.Items.FindByValue(listItem.Value);
                //if (byValue != null)
                //    byValue.Selected = true;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            //if (this.KeepHiddenValue && Settings.IsAnalyticsEnabled && this.FindControl(this.ID + "_complexvalue") == null)
            //{
            //    HiddenField hiddenField = new HiddenField();
            //    hiddenField.ID = this.ID + "_complexvalue";
            //    hiddenField.Value = (string)this.Result.Value;
            //    this.Controls.AddAt(0, (Control)hiddenField);
            //}
            //this.title.AssociatedControlID = this.InnerListControl.ID;
            base.OnPreRender(e);
        }

        [DefaultValue(5)]
        [VisualFieldType(typeof(EditField))]
        [VisualProperty("File size limit (MB) :", 5)]
        [VisualCategory("Upload")]
        public string FileSizeLimit
        {
            get
            {
                return this._fileSizeLimit.ToString();
            }
            set
            {
                int result;
                if (!int.TryParse(value, out result))
                    result = 5;
                this._fileSizeLimit = result;
            }
        }
    }
}