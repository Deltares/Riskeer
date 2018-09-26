// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismSectionCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCreator.Create(null,
                                                                                        new SerializableFailureMechanismSectionCollection(),
                                                                                        ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_CollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCreator.Create(new IdentifierGenerator(),
                                                                                        null,
                                                                                        ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableCollection", exception.ParamName);
        }

        [Test]
        public void Create_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCreator.Create(new IdentifierGenerator(),
                                                                                        new SerializableFailureMechanismSectionCollection(),
                                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Create_WithSection_ReturnsSerializableFailureMechanismSection()
        {
            // Setup
            const string collectionId = "collectionId";
            var collection = new SerializableFailureMechanismSectionCollection(collectionId);

            var idGenerator = new IdentifierGenerator();
            ExportableFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();

            // Call
            SerializableFailureMechanismSection serializableSection =
                SerializableFailureMechanismSectionCreator.Create(idGenerator, collection, section);

            // Assert
            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(section, collection, serializableSection);
        }

        [Test]
        public void CreateWithCombinedAssemblySection_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCreator.Create(null,
                                                                                        new SerializableFailureMechanismSectionCollection(),
                                                                                        ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithCombinedAssemblySection_CollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCreator.Create(new IdentifierGenerator(),
                                                                                        null,
                                                                                        ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableCollection", exception.ParamName);
        }

        [Test]
        public void CreateWithCombinedAssemblySection_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionCreator.Create(new IdentifierGenerator(),
                                                                                        new SerializableFailureMechanismSectionCollection(),
                                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateWithCombinedAssemblySection_WithSection_ReturnsSerializableFailureMechanismSection()
        {
            // Setup
            const string collectionId = "collectionId";
            var collection = new SerializableFailureMechanismSectionCollection(collectionId);

            var idGenerator = new IdentifierGenerator();
            ExportableCombinedFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection();

            // Call
            SerializableFailureMechanismSection serializableSection =
                SerializableFailureMechanismSectionCreator.Create(idGenerator, collection, section);

            // Assert
            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(section, collection, serializableSection);
        }
    }
}