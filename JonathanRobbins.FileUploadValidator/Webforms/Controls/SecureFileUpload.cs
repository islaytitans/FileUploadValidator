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
using Sitecore.Form.Web.UI.Controls;
using ListItemCollection = System.Web.UI.WebControls.ListItemCollection;

namespace JonathanRobbins.FileUploadValidator.Webforms.Controls
{
    public class SecureFileUpload : UploadFile
    {
        protected ListItemCollection mimeItems;
        protected ListItemCollection selectedItems;
        protected ListBox list = new ListBox();

        [VisualFieldType(typeof(SelectionModeField))]
        [VisualProperty("Selection Mode:", 400)]
        [VisualCategory("List")]
        [DefaultValue("Single")]
        public string SelectionMode
        {
            get
            {
                return list.SelectionMode.ToString();
            }
            set
            {
                list.SelectionMode = (ListSelectionMode)Enum.Parse(typeof(ListSelectionMode), value, true);
            }
        }

        [VisualProperty("Rows:", 450)]
        [DefaultValue(4)]
        public int Rows
        {
            get
            {
                return this.list.Rows;
            }
            set
            {
                this.list.Rows = value;
            }
        }

        public override string ID
        {
            get
            {
                return this.list.ID;
            }
            set
            {
                this.title.ID = value + "text";
                this.list.ID = value;
                this.upload.ID = value;
                base.ID = value;
            }
        }

        protected System.Web.UI.WebControls.ListControl InnerListControl
        {
            get
            {
                return (System.Web.UI.WebControls.ListControl)this.list;
            }
        }

        public SecureFileUpload()
            : this(HtmlTextWriterTag.Div)
        {
        }

        private SecureFileUpload(HtmlTextWriterTag tag)
            : base(tag)
        {
            //this.CssClass = base.baseCssClassName;
            this.list.Rows = 4;
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        [VisualCategory("List")]
        [VisualFieldType(typeof(ListField))]
        [TypeConverter(typeof(ListItemCollectionConverter))]
        [Description("Collection of mimeItems.")]
        [VisualProperty("mimeItems:", 100)]
        public ListItemCollection Items
        {
            get
            {
                return this.mimeItems;
            }
            set
            {
                this.mimeItems = value;
            }
        }

        [Browsable(false)]
        public char Separator
        {
            get
            {
                return '|';
            }
        }

        [TypeConverter(typeof(ListItemCollectionConverter))]
        [VisualFieldType(typeof(SelectedValueField))]
        [VisualCategory("List")]
        [Browsable(false)]
        [VisualProperty("Selected Value:", 200)]
        public ListItemCollection SelectedValue
        {
            get
            {
                return this.selectedItems;
            }
            set
            {
                this.selectedItems = value;
            }
        }

        #region ListControl


        #endregion 
    }
}