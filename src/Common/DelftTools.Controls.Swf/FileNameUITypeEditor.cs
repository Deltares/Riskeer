using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// Override default type editor to provide a savefile dialog as well
    /// </summary>
    /// <remarks>usage example in tag for property editor 
    /// [Editor(typeof(FileNameUITypeEditor), typeof(UITypeEditor)),SaveFile , FileDialogFilter("ASCII files(*.ASC;*.TXT)|*.ASC;*.TXT|All files (*.*)|*.*"),FileDialogTitle("Select output file")]
    /// http://www.codeproject.com/vb/net/UIFilenameEditor.asp?df=100&amp;forumid=39019&amp;exp=0&amp;select=810716
    /// </remarks>
    public class FileNameUITypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return UITypeEditorEditStyle.None;
        }

        /// <summary>
        /// Show a dialog to open or save a file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [RefreshProperties(RefreshProperties.All)]
        public override object EditValue(ITypeDescriptorContext context,
                                         IServiceProvider provider,
                                         object value)
        {
            if (context == null || provider == null || context.Instance == null)
            {
                return EditValue(provider, value);
            }

            if (context.PropertyDescriptor.Attributes[typeof (SelectDirOnlyAttribute)] != null)
            {
                var dialog = new FolderBrowserDialog
                    {
                        ShowNewFolderButton = true,
                        Description = "Select " + context.PropertyDescriptor.DisplayName
                    };

                Environment.SpecialFolder directory;
                if (value != null && Environment.SpecialFolder.TryParse(value.ToString(), true, out directory))
                {
                    dialog.RootFolder = directory;
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }

                dialog.Dispose();
                return value;
            }

            FileDialog fileDlg;
            if (context.PropertyDescriptor.Attributes[typeof(SaveFileAttribute)] == null)
            {
                fileDlg = new OpenFileDialog();
            }
            else
            {
                fileDlg = new SaveFileDialog();
            }

            fileDlg.Title = "Select " + context.PropertyDescriptor.DisplayName;
            if (value == null)
            {
                fileDlg.FileName = "";
            }
            else
            {
                fileDlg.FileName = value.ToString();
            }

            FileDialogFilterAttribute filterAtt =
                context.PropertyDescriptor.Attributes[typeof(FileDialogFilterAttribute)] as
                FileDialogFilterAttribute;
            if (filterAtt != null)
            {
                fileDlg.Filter = filterAtt.Filter;
            }

            FileDialogTitleAttribute titleAtt =
                context.PropertyDescriptor.Attributes[typeof(FileDialogTitleAttribute)] as
                FileDialogTitleAttribute;
            if (titleAtt != null)
            {
                fileDlg.Title = titleAtt.Title;
            }

            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                return fileDlg.FileName;
            }

            fileDlg.Dispose();
            return value;
        }
    }

    /// <summary>
    /// 'SelectDir only' attribute - indicates that the FolderBrowserDialog should be shown
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SelectDirOnlyAttribute : Attribute
    {
    }

    /// <summary>
    /// The filter to use in the file dialog in UIFilenameEditor.
    /// </summary>
    /// <remarks>The following is an example of a filter string: "Text files (*.txt)|*.txt|All files (*.*)|*.*"</remarks>
    /// <history>[logch_o] 2007-09-10 converted to c# from vb</history>
    /// <remark>http://www.gotdotnet.com/team/fxcop/docs/rules/Performance/AvoidUnsealedAttributes.html</remark>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FileDialogFilterAttribute : Attribute
    {
        private readonly string filter;

        /// <summary>
        /// Define a filter for the UIFilenameEditor.
        /// </summary>
        /// <history>[logch_o] 2007-09-10 converted to c# from vb</history>
        public string Filter
        {
            get { return filter; }
        }

        public FileDialogFilterAttribute(string filter)
        {
            this.filter = filter;
        }
    }

    /// <summary>
    /// 'Save file' attribute - indicates that SaveFileDialog must be shown
    /// </summary>
    /// <remark>http://www.gotdotnet.com/team/fxcop/docs/rules/Performance/AvoidUnsealedAttributes.html</remark>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SaveFileAttribute : Attribute
    {
    }

    /// <summary>
    /// Title of the dialog presented to the end-user.
    /// </summary>
    /// <remark>http://www.gotdotnet.com/team/fxcop/docs/rules/Performance/AvoidUnsealedAttributes.html</remark>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FileDialogTitleAttribute : Attribute
    {
        private readonly string title;

        /// <summary>
        /// Define a filter for the UIFilenameEditor.
        /// </summary>
        /// <history>[logch_o] 2007-09-10 converted to c# from vb</history>
        public string Title
        {
            get { return title; }
        }

        public FileDialogTitleAttribute(string title)
        {
            this.title = title;
        }
    }
}