using System;
using DelftTools.Controls;
using log4net.Core;

namespace DelftTools.Shell.Gui.Forms
{
    public interface IMessageWindow : IView 
    {
        /// <summary>
        /// Adds logging event as a log4net event to the window.
        /// Only some columns are added.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="time"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="timestamp"></param>
        void AddMessage(Level level, DateTime time, string source, string message, string exception);

        /// <summary>
        /// Clears all messages in the window.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns true if message level is enabled in the window (can be shown).
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool IsMessageLevelEnabled(Level level);
    }
}