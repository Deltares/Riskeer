using System;
using DelftTools.Utils;

namespace DelftTools.Controls
{
    public interface ITableViewPasteController
    {
        /// <summary>
        /// Occurs when a paste failed due to sorting or filtering.
        /// </summary>
        event EventHandler<EventArgs<string>> PasteFailed;

        ///<summary>
        /// Gets or sets the paste behaviour value
        ///</summary>
        TableViewPasteBehaviourOptions PasteBehaviour { set; }

        /// <summary>
        /// This method does most of the work. It Pastes Clipboard content to the table view. 
        /// </summary>
        void PasteClipboardContents();
    }
}