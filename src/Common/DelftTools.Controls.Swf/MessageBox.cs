using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// Wraps normal windows.forms messagebox but adds testability. When CustomMessageBox is set it wil be used instead of the normal windows.forms.messagebox
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// When set the show message of this messagebox will be called and used as a dialogresult 
        /// </summary>
        public static IMessageBox CustomMessageBox { get; set; }

        public static DialogResult Show(string text)
        {
            return Show(text, "");
        }

        public static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            //default uses system windows forms implementation
            return Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            //custom box ignores icon and defaultbutton
            if (CustomMessageBox != null)
            {
                //ignore the icon in the custom messagebox for now...add it when you need it.
                return CustomMessageBox.Show(text, caption, buttons);
            }

            return System.Windows.Forms.MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }
    }

    public interface IMessageBox
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogResult Show(string text, string caption, MessageBoxButtons buttons);
    }
}