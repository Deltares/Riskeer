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

using System.Linq;
using Core.Common.Base.Plugin;
using NUnit.Framework;

namespace Core.Common.Base.Test.Plugin
{
    [TestFixture]
    public class ApplicationPluginTest
    {
        [Test]
        public void GetFileImporters_ReturnEmptyEnumerable()
        {
            // Setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // Call
            var importers = applicationPlugin.GetFileImporters().ToArray();

            // Assert
            CollectionAssert.IsEmpty(importers);
        }

        [Test]
        public void GetFileExporters_ReturnEmptyEnumerable()
        {
            // Setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // Call
            var importers = applicationPlugin.GetFileExporters().ToArray();

            // Assert
            CollectionAssert.IsEmpty(importers);
        }

        [Test]
        public void GetDataItemInfos_ReturnEmptyEnumerable()
        {
            // Setup
            var applicationPlugin = new SimpleApplicationPlugin();

            // Call
            var importers = applicationPlugin.GetDataItemInfos().ToArray();

            // Assert
            CollectionAssert.IsEmpty(importers);
        }

        private class SimpleApplicationPlugin : ApplicationPlugin
        {

        }
    }
}