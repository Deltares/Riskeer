// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using log4net.Appender;
using log4net.Core;

namespace Core.Common.Gui.Appenders
{
    public class RunReportLogAppender : AppenderSkeleton
    {
        public RunReportLogAppender()
        {
            Instance = this;
        }

        public static RunReportLogAppender Instance { get; set; }

        public Action<string> AppendMessageLineAction { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (AppendMessageLineAction != null)
            {
                AppendMessageLineAction(loggingEvent.RenderedMessage);
            }
        }
    }
}