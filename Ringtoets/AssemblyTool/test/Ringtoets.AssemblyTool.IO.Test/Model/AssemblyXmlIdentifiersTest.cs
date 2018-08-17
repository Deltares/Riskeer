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

using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class AssemblyXmlIdentifiersTest
    {
        [Test]
        public void AssemblyXmlIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("Assemblage", AssemblyXmlIdentifiers.Assembly);
            Assert.AreEqual("toetsspoorGroep", AssemblyXmlIdentifiers.AssemblyGroup);
            Assert.AreEqual("toetsoordeelZonderKansschatting", AssemblyXmlIdentifiers.AssemblyResultWithoutProbability);
            Assert.AreEqual("toetsoordeelMetKansschatting", AssemblyXmlIdentifiers.AssemblyResultWithProbability);
            Assert.AreEqual("http://localhost/standaarden/assemblage", AssemblyXmlIdentifiers.AssemblyNamespace);
            Assert.AreEqual("asm", AssemblyXmlIdentifiers.AssemblyNamespaceIdentifier);
            Assert.AreEqual("toets", AssemblyXmlIdentifiers.AssessmentLevel);
            Assert.AreEqual("Beoordelingsproces", AssemblyXmlIdentifiers.AssessmentProcess);
            Assert.AreEqual("BeoordelingsprocesID", AssemblyXmlIdentifiers.AssessmentProcessId);
            Assert.AreEqual("BeoordelingsprocesIDRef", AssemblyXmlIdentifiers.AssessmentProcessIdRef);
            Assert.AreEqual("veiligheidsoordeel", AssemblyXmlIdentifiers.AssessmentSectionAssemblyResult);
            Assert.AreEqual("categorie", AssemblyXmlIdentifiers.AssessmentSectionCategoryGroup);
            Assert.AreEqual("WaterkeringstelselIDRef", AssemblyXmlIdentifiers.AssessmentSectionIdRef);
            Assert.AreEqual("typeWaterkeringstelsel", AssemblyXmlIdentifiers.AssessmentSectionType);
            Assert.AreEqual("Waterkeringstelsel", AssemblyXmlIdentifiers.AssessmentSection);

            Assert.AreEqual("boundedBy", AssemblyXmlIdentifiers.BoundedBy);

            Assert.AreEqual("GecombineerdToetsoordeel", AssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssembly);
            Assert.AreEqual("GecombineerdToetsoordeelID", AssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssemblyId);
            Assert.AreEqual("toetsoordeelGecombineerd", AssemblyXmlIdentifiers.CombinedCombinedSectionResult);
            Assert.AreEqual("eindtoetsoordeel", AssemblyXmlIdentifiers.CombinedSectionResult);
            Assert.AreEqual("eindtoetsoordeelToetsspoor", AssemblyXmlIdentifiers.CombinedSectionFailureMechanismResult);
            Assert.AreEqual("srsName", AssemblyXmlIdentifiers.CoordinateSystem);

            Assert.AreEqual("typeFaalmechanisme", AssemblyXmlIdentifiers.DirectFailureMechanism);

            Assert.AreEqual("afstandEinde", AssemblyXmlIdentifiers.EndDistance);
            Assert.AreEqual("eindJaarBeoordelingsronde", AssemblyXmlIdentifiers.EndYear);
            Assert.AreEqual("Envelope", AssemblyXmlIdentifiers.Envelope);

            Assert.AreEqual("Toetsspoor", AssemblyXmlIdentifiers.FailureMechanism);
            Assert.AreEqual("categorieTraject", AssemblyXmlIdentifiers.FailureMechanismCategoryGroup);
            Assert.AreEqual("toetsoordeel", AssemblyXmlIdentifiers.FailureMechanismAssemblyResult);
            Assert.AreEqual("ToetsspoorID", AssemblyXmlIdentifiers.FailureMechanismId);
            Assert.AreEqual("ToetsspoorIDRef", AssemblyXmlIdentifiers.FailureMechanismIdRef);
            Assert.AreEqual("Waterkeringsectie", AssemblyXmlIdentifiers.FailureMechanismSection);
            Assert.AreEqual("WaterkeringsectieIDRef", AssemblyXmlIdentifiers.FailureMechanismSectionIdRef);
            Assert.AreEqual("Toets", AssemblyXmlIdentifiers.FailureMechanismSectionAssembly);
            Assert.AreEqual("ToetsID", AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyId);
            Assert.AreEqual("categorieVak", AssemblyXmlIdentifiers.FailureMechanismSectionCategoryGroup);
            Assert.AreEqual("Vakindeling", AssemblyXmlIdentifiers.FailureMechanismSectionCollection);
            Assert.AreEqual("VakindelingID", AssemblyXmlIdentifiers.FailureMechanismSectionCollectionId);
            Assert.AreEqual("VakindelingIDRef", AssemblyXmlIdentifiers.FailureMechanismSectionCollectionIdRef);
            Assert.AreEqual("typeWaterkeringsectie", AssemblyXmlIdentifiers.FailureMechanismSectionType);
            Assert.AreEqual("typeToetsspoor", AssemblyXmlIdentifiers.FailureMechanismType);
            Assert.AreEqual("featureMember", AssemblyXmlIdentifiers.FeatureMember);

            Assert.AreEqual("posList", AssemblyXmlIdentifiers.Geometry);
            Assert.AreEqual("geometrie2D", AssemblyXmlIdentifiers.Geometry2D);
            Assert.AreEqual("geometrieLijn2D", AssemblyXmlIdentifiers.GeometryLine2D);
            Assert.AreEqual("http://www.opengis.net/gml/3.2", AssemblyXmlIdentifiers.GmlNamespace);
            Assert.AreEqual("gml", AssemblyXmlIdentifiers.GmlNamespaceIdentifier);

            Assert.AreEqual("id", AssemblyXmlIdentifiers.Id);

            Assert.AreEqual("lengte", AssemblyXmlIdentifiers.Length);
            Assert.AreEqual("LineString", AssemblyXmlIdentifiers.LineString);
            Assert.AreEqual("lowerCorner", AssemblyXmlIdentifiers.LowerCorner);

            Assert.AreEqual("naam", AssemblyXmlIdentifiers.Name);

            Assert.AreEqual("faalkans", AssemblyXmlIdentifiers.Probability);

            Assert.AreEqual("toetsoordeelVak", AssemblyXmlIdentifiers.SectionResults);
            Assert.AreEqual("afstandBegin", AssemblyXmlIdentifiers.StartDistance);
            Assert.AreEqual("beginJaarBeoordelingsronde", AssemblyXmlIdentifiers.StartYear);
            Assert.AreEqual("status", AssemblyXmlIdentifiers.Status);

            Assert.AreEqual("Veiligheidsoordeel", AssemblyXmlIdentifiers.TotalAssemblyResult);
            Assert.AreEqual("VeiligheidsoordeelID", AssemblyXmlIdentifiers.TotalAssemblyResultId);
            Assert.AreEqual("VeiligheidsoordeelIDRef", AssemblyXmlIdentifiers.TotalAssemblyResultIdRef);

            Assert.AreEqual("uom", AssemblyXmlIdentifiers.UnitOfMeasure);
            Assert.AreEqual("upperCorner", AssemblyXmlIdentifiers.UpperCorner);

        }
    }
}