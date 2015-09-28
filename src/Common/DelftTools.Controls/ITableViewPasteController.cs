using System;
using DelftTools.Utils;

namespace DelftTools.Controls
{
    public interface ITableViewPasteController
    {
        /// <summary>
        /// Paste value string into tableview at current selection
        /// </summary>
        /// <param name="lines"></param>
        void PasteLines(string[] lines);

        /// <summary>
        /// Whether or not the controller is pasting.
        /// </summary>
        bool IsPasting { get; }

        /// <summary>
        /// This method does most of the work. It Pastes Clipboard content to the table view. 
        /// </summary>
        void PasteClipboardContents();

        /// <summary>
        /// Occurs when a paste failed due to sorting or filtering.
        /// </summary>
        event EventHandler<EventArgs<string>> PasteFailed;

        /// <summary>
        /// Occurs when a paste finished.
        /// </summary>
        event EventHandler<EventArgs> PasteFinished;

        ///<summary>
        /// Gets or sets the paste behaviour value
        ///</summary>
        TableViewPasteBehaviourOptions PasteBehaviour { get; set; }
    }
}