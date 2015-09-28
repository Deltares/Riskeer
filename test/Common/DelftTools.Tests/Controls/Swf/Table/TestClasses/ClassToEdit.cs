using System.ComponentModel;
using System.Drawing.Design;
using DelftTools.Controls.Swf;

namespace DelftTools.Shell.Gui.Swf.Controls.Tests.Table.TestClasses
{
    public class ClassToEdit
    {
        private string _filenameToOpen;

        private string _filenameToSave;

        [Editor(typeof (FileNameUITypeEditor), typeof (UITypeEditor))]
        [FileDialogFilter("Assembly files (*.dll, *.exe)|*.dll;*.exe|All files (*.*)|*.*")]
        [Description("The filename to open.")]
        [Category("Filenames")]
        public string FilenameToOpen
        {
            get { return _filenameToOpen; }
            set { _filenameToOpen = value; }
        }

        [Editor(typeof (FileNameUITypeEditor), typeof (UITypeEditor))]
        [SaveFile]
        [FileDialogFilter("Text files (*.txt)|*.txt|All files (*.*)|*.*")]
        [Description("The filename to save.")]
        [Category("Filenames")]
        public string FilenameToSave
        {
            get { return _filenameToSave; }
            set { _filenameToSave = value; }
        }
    }
}