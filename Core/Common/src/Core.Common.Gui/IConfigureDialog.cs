using System;
using System.Windows.Forms;

namespace Core.Common.Gui
{
    /// <summary>
    /// Dialog with configuring abilities
    /// </summary>
    public interface IConfigureDialog
    {
        /// <summary>
        /// Configures model or source of dialog
        /// </summary>
        /// <param name="model">model or source of dialog</param>
        [Obsolete("responsibility of this method is veeeeert implicit, it uses importer / exporter as model! Object as argument.")]
        void Configure(object model);

        DialogResult ShowModal();
    }
}