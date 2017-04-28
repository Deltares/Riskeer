// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using log4net.Appender;
using log4net.Core;

namespace Core.Common.Gui.Appenders
{
    /// <summary>
    /// Singleton log-appender for Log4Net that is used to fill a 'Run-report' when executing
    /// a particular piece of code.
    /// </summary>
    public class RenderedMessageLogAppender : AppenderSkeleton
    {
        private Action<string> appendMessageLineAction;

        /// <summary>
        /// Initializes the singleton.
        /// </summary>
        public RenderedMessageLogAppender() // Constructor might be marked as unused, but is called in app.config.
        {
            Instance = this;
        }

        /// <summary>
        /// The singleton value.
        /// </summary>
        public static RenderedMessageLogAppender Instance { get; private set; }

        /// <summary>
        /// The action to be called when a new message is logged.
        /// </summary>
        public Action<string> AppendMessageLineAction
        {
            get
            {
                return appendMessageLineAction;
            }
            set
            {
                if (value != null && appendMessageLineAction != null)
                {
                    throw new InvalidOperationException("An action is already set");
                }

                appendMessageLineAction = value;
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            AppendMessageLineAction?.Invoke(loggingEvent.RenderedMessage);
        }
    }
}