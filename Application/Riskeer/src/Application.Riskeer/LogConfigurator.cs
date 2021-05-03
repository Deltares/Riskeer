// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using Core.Common.Util.Settings;
using Core.Gui.Forms.MessageWindow;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Application.Riskeer
{
    /// <summary>
    /// Class for configuring the logging functionality of Riskeer.
    /// </summary>
    public static class LogConfigurator
    {
        /// <summary>
        /// Initializes the logging functionality.
        /// </summary>
        public static void Initialize()
        {
            var hierarchy = (Hierarchy) LogManager.GetRepository();

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };
            patternLayout.ActivateOptions();

            var fileAppender = new FileAppender
            {
                AppendToFile = false,
                File = Path.Combine(SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(), "BOI", "Riskeer", "Riskeer-") + DateTime.Now.ToString("yyyy.MMM.dd.HH.mm.ss") + ".log",
                Layout = patternLayout
            };

            fileAppender.ActivateOptions();
            hierarchy.Root.AddAppender(fileAppender);

            var messageWindowLogAppender = new MessageWindowLogAppender();
            hierarchy.Root.AddAppender(messageWindowLogAppender);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }
    }
}