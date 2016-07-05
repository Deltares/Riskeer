// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Core.Common.TestUtil
{
    public static class LogHelper
    {
        /// <summary>
        /// Sets logging level for all current loggers to the level provided in arguments.
        /// Note: use it only when you need more control on logging, e.g. in unit tests. Otherwise use configuration files.
        /// </summary>
        /// <param name="level"></param>
        public static void SetLoggingLevel(Level level)
        {
            var repositories = LogManager.GetAllRepositories();

            //Configure all loggers to be at the debug level.
            foreach (var repository in repositories)
            {
                repository.Threshold = repository.LevelMap[level.ToString()];
                var hierarchy = (Hierarchy) repository;
                var loggers = hierarchy.GetCurrentLoggers();
                foreach (var logger in loggers)
                {
                    ((Logger) logger).Level = hierarchy.LevelMap[level.ToString()];
                }
            }

            //Configure the root logger.
            var h = (Hierarchy) LogManager.GetRepository();
            var rootLogger = h.Root;
            rootLogger.Level = h.LevelMap[level.ToString()];
        }

        /// <summary>
        /// Configures logging. In case of log4net reads log4net section from app.config file.
        /// </summary>
        public static void ConfigureLogging(Level level = null)
        {
            ResetLogging();
            BasicConfigurator.Configure();
            SetLoggingLevel(level ?? Level.Error);
        }

        /// <summary>
        /// Resets logging configuration, no log messages are sent after that.
        /// </summary>
        public static void ResetLogging()
        {
            LogManager.ResetConfiguration();
            SetLoggingLevel(Level.Error);
        }
    }
}