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

using System.ComponentModel;
using log4net.Core;

namespace Core.Common.TestUtil
{
    public enum LogLevelConstant
    {
        Off,
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }

    public static class LogLevelConstantExtensions
    {
        public static Level ToLog4NetLevel(this LogLevelConstant level)
        {
            switch (level)
            {
                case LogLevelConstant.Off:
                    return Level.Off;
                case LogLevelConstant.Fatal:
                    return Level.Fatal;
                case LogLevelConstant.Error:
                    return Level.Error;
                case LogLevelConstant.Warn:
                    return Level.Warn;
                case LogLevelConstant.Info:
                    return Level.Info;
                case LogLevelConstant.Debug:
                    return Level.Debug;
                default:
                    throw new InvalidEnumArgumentException("level", (int) level, typeof(LogLevelConstant));
            }
        }
    }
}