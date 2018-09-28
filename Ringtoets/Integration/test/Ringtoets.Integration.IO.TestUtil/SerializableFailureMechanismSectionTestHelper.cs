// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.Model.Helpers;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert a <see cref="SerializableFailureMechanismSection"/>.
    /// </summary>
    public static class SerializableFailureMechanismSectionTestHelper
    {
        /// <summary>
        /// Asserts a <see cref="SerializableFailureMechanismSection"/> against
        /// an <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <param name="expectedSection">The <see cref="ExportableFailureMechanismSection"/> to assert against.</param>
        /// <param name="expectedCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the section belongs to.</param>
        /// <param name="actualSerializableSection">The <see cref="SerializableFailureMechanismSection"/> to assert.</param>
        /// <param name="expectedId">The expected id for the <see cref="SerializableFailureMechanismSection"/>.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The id does not match with the expected id.</item>
        /// <item>The id of the failure mechanism section collection does not match.</item>
        /// <item>The geometry, start distance or the end distance of the failure mechanism section does not match.</item>
        /// <item>The failure mechanism section type does not match.</item>
        /// <item>The used assembly method to obtain the section does not match.</item>
        /// </list>
        /// </exception>
        public static void AssertFailureMechanismSection(ExportableFailureMechanismSection expectedSection,
                                                         SerializableFailureMechanismSectionCollection expectedCollection,
                                                         SerializableFailureMechanismSection actualSerializableSection,
                                                         int expectedId = 0)
        {
            Assert.AreEqual($"Tv.{expectedId}", actualSerializableSection.Id);
            Assert.AreEqual(expectedCollection.Id, actualSerializableSection.FailureMechanismSectionCollectionId);

            Assert.AreEqual(GeometrySerializationFormatter.Format(expectedSection.Geometry),
                            actualSerializableSection.Geometry.LineString.Geometry);
            Assert.AreEqual(expectedSection.StartDistance, actualSerializableSection.StartDistance.Value);
            Assert.AreEqual(expectedSection.EndDistance, actualSerializableSection.EndDistance.Value);
            Assert.AreEqual(SerializableFailureMechanismSectionType.FailureMechanism,
                            actualSerializableSection.FailureMechanismSectionType);
            Assert.IsNull(actualSerializableSection.AssemblyMethod);
        }

        /// <summary>
        /// Asserts a <see cref="SerializableFailureMechanismSection"/> against
        /// an <see cref="ExportableCombinedFailureMechanismSection"/>.
        /// </summary>
        /// <param name="expectedSection">The <see cref="ExportableCombinedFailureMechanismSection"/> to assert against.</param>
        /// <param name="expectedCollection">The <see cref="SerializableFailureMechanismSectionCollection"/> the section belongs to.</param>
        /// <param name="actualSerializableSection">The <see cref="SerializableFailureMechanismSection"/> to assert.</param>
        /// <param name="expectedId">The expected id for the <see cref="SerializableFailureMechanismSection"/>.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The id does not match with the expected id.</item>
        /// <item>The id of the failure mechanism section collection does not match.</item>
        /// <item>The geometry, start distance or the end distance of the failure mechanism section does not match.</item>
        /// <item>The failure mechanism section type does not match.</item>
        /// <item>The used assembly method to obtain the section does not match.</item>
        /// </list>
        /// </exception>
        public static void AssertFailureMechanismSection(ExportableCombinedFailureMechanismSection expectedSection,
                                                         SerializableFailureMechanismSectionCollection expectedCollection,
                                                         SerializableFailureMechanismSection actualSerializableSection,
                                                         int expectedId = 0)
        {
            Assert.AreEqual($"Tv.{expectedId}", actualSerializableSection.Id);
            Assert.AreEqual(expectedCollection.Id, actualSerializableSection.FailureMechanismSectionCollectionId);

            Assert.AreEqual(GeometrySerializationFormatter.Format(expectedSection.Geometry),
                            actualSerializableSection.Geometry.LineString.Geometry);
            Assert.AreEqual(expectedSection.StartDistance, actualSerializableSection.StartDistance.Value);
            Assert.AreEqual(expectedSection.EndDistance, actualSerializableSection.EndDistance.Value);
            Assert.AreEqual(SerializableFailureMechanismSectionType.Combined,
                            actualSerializableSection.FailureMechanismSectionType);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedSection.AssemblyMethod),
                            actualSerializableSection.AssemblyMethod);
        }
    }
}