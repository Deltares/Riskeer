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
using SystemConsole = System.Console;

namespace Migration.Console.TestUtil
{
    /// <summary>
    /// This class captures the output sent to <see cref="SystemConsole"/>.
    /// </summary>
    public class ConsoleOutput : IDisposable
    {
        private readonly StringWriter stringWriter;
        private readonly TextWriter originalOutput;
        private bool disposed;

        /// <summary>
        /// Creates a new instance of <see cref="ConsoleOutput"/>.
        /// </summary>
        public ConsoleOutput()
        {
            originalOutput = SystemConsole.Out;
            stringWriter = new StringWriter();
            SystemConsole.SetOut(stringWriter);
        }

        /// <summary>
        /// Gets text that was sent to to <see cref="SystemConsole"/> (if any).
        /// </summary>
        /// <returns>The captured text.</returns>
        public string GetConsoleOutput()
        {
            stringWriter.Flush();
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                SystemConsole.SetOut(originalOutput);
                stringWriter?.Dispose();
            }

            disposed = true;
        }
    }
}