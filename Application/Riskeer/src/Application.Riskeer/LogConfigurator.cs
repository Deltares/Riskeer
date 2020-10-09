// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.IO;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.Util.Settings;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Application.Riskeer
{
    /// <summary>
    /// Class for managing the logging capabilities of Riskeer.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Sets the configuration of the logger.
        /// </summary>
        public static void Setup()
        {
            var hierarchy = (Hierarchy) LogManager.GetRepository();

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };
            patternLayout.ActivateOptions();

            var roller = new FileAppender
            {
                AppendToFile = false,
                File = Path.Combine(SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(), "BOI", "Riskeer", "Riskeer-") + DateTime.Now.ToString("yyyy.MMM.dd.HH.mm.ss") + ".log",
                Layout = patternLayout
            };

            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            var messageWindowLogAppender = new MessageWindowLogAppender();
            hierarchy.Root.AddAppender(messageWindowLogAppender);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }
    }
}