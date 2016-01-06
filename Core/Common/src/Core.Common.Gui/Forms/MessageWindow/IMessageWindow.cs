using System;
using Core.Common.Controls;
using Core.Common.Forms.Views;
using log4net.Core;

namespace Core.Common.Gui.Forms.MessageWindow
{
    public interface IMessageWindow : IView
    {
        /// <summary>
        /// Adds logging event as a log4net event to the window.
        /// Only some columns are added.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="time"></param>
        /// <param name="message"></param>
        void AddMessage(Level level, DateTime time, string message);

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