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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System.Collections.Generic;
using Core.Common.Base.IO;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Class that provides application plugin objects (file importers, file exporters and data items).
    /// </summary>
    public abstract class ApplicationPlugin
    {
        /// <summary>
        /// This method activates the <see cref="ApplicationPlugin"/>.
        /// </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        /// This method deactivates the <see cref="ApplicationPlugin"/>.
        /// </summary>
        public virtual void Deactivate()
        {

        }

        /// <summary>
        /// This method returns an enumeration of <see cref="IFileImporter"/>.
        /// </summary>
        /// <returns>The enumeration of <see cref="IFileImporter"/> provided by the <see cref="ApplicationPlugin"/>.</returns>
        public virtual IEnumerable<IFileImporter> GetFileImporters()
        {
            yield break;
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="IFileExporter"/>.
        /// </summary>
        /// <returns>The enumeration of <see cref="IFileExporter"/> provided by the <see cref="ApplicationPlugin"/>.</returns>
        public virtual IEnumerable<IFileExporter> GetFileExporters()
        {
            yield break;
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="DataItemInfo"/>.
        /// </summary>
        /// <returns>The enumeration of <see cref="DataItemInfo"/> provided by the <see cref="ApplicationPlugin"/>.</returns>
        public virtual IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield break;
        }
    }
}