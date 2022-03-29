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

using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class AssemblyXmlIdentifiersTest
    {
        [Test]
        public void AssemblyXmlIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("Assemblage", AssemblyXmlIdentifiers.Assembly);
            Assert.AreEqual("assemblagemethode", AssemblyXmlIdentifiers.AssemblyMethod);
            Assert.AreEqual("http://localhost/standaarden/assemblage", AssemblyXmlIdentifiers.AssemblyNamespace);
            Assert.AreEqual("asm", AssemblyXmlIdentifiers.AssemblyNamespaceIdentifier);
            Assert.AreEqual("Beoordelingsproces", AssemblyXmlIdentifiers.AssessmentProcess);
            Assert.AreEqual("BeoordelingsprocesID", AssemblyXmlIdentifiers.AssessmentProcessId);
            Assert.AreEqual("BeoordelingsprocesIDRef", AssemblyXmlIdentifiers.AssessmentProcessIdRef);
            Assert.AreEqual("categorie", AssemblyXmlIdentifiers.AssessmentSectionAssemblyGroup);
            Assert.AreEqual("WaterkeringstelselIDRef", AssemblyXmlIdentifiers.AssessmentSectionIdRef);
            Assert.AreEqual("typeWaterkeringstelsel", AssemblyXmlIdentifiers.AssessmentSectionType);
            Assert.AreEqual("Waterkeringstelsel", AssemblyXmlIdentifiers.AssessmentSection);

            Assert.AreEqual("boundedBy", AssemblyXmlIdentifiers.BoundedBy);

            Assert.AreEqual("FaalanalyseGecombineerd", AssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssembly);
            Assert.AreEqual("FaalanalyseGecombineerdID", AssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssemblyId);
            Assert.AreEqual("analyseGecombineerdDeelvak", AssemblyXmlIdentifiers.CombinedCombinedSectionResult);
            Assert.AreEqual("analyseVak", AssemblyXmlIdentifiers.SectionResult);
            Assert.AreEqual("analyseDeelvak", AssemblyXmlIdentifiers.CombinedSectionFailureMechanismResult);
            Assert.AreEqual("srsName", AssemblyXmlIdentifiers.CoordinateSystem);

            Assert.AreEqual("afstandEinde", AssemblyXmlIdentifiers.EndDistance);
            Assert.AreEqual("eindJaarBeoordelingsronde", AssemblyXmlIdentifiers.EndYear);
            Assert.AreEqual("Envelope", AssemblyXmlIdentifiers.Envelope);

            Assert.AreEqual("Faalmechanisme", AssemblyXmlIdentifiers.FailureMechanism);
            Assert.AreEqual("analyseFaalmechanisme", AssemblyXmlIdentifiers.FailureMechanismAssemblyResult);
            Assert.AreEqual("FaalmechanismeID", AssemblyXmlIdentifiers.FailureMechanismId);
            Assert.AreEqual("FaalmechanismeIDRef", AssemblyXmlIdentifiers.FailureMechanismIdRef);
            Assert.AreEqual("Deelvak", AssemblyXmlIdentifiers.FailureMechanismSection);
            Assert.AreEqual("WaterkeringsectieIDRef", AssemblyXmlIdentifiers.FailureMechanismSectionIdRef);
            Assert.AreEqual("Faalanalyse", AssemblyXmlIdentifiers.FailureMechanismSectionAssembly);
            Assert.AreEqual("FaalanalyseID", AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyId);
            Assert.AreEqual("duidingsklasse", AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyGroup);
            Assert.AreEqual("Vakindeling", AssemblyXmlIdentifiers.FailureMechanismSectionCollection);
            Assert.AreEqual("VakindelingID", AssemblyXmlIdentifiers.FailureMechanismSectionCollectionId);
            Assert.AreEqual("VakindelingIDRef", AssemblyXmlIdentifiers.FailureMechanismSectionCollectionIdRef);
            Assert.AreEqual("typeWaterkeringsectie", AssemblyXmlIdentifiers.FailureMechanismSectionType);
            Assert.AreEqual("typeFaalmechanisme", AssemblyXmlIdentifiers.FailureMechanismType);
            Assert.AreEqual("generiekFaalmechanisme", AssemblyXmlIdentifiers.GenericFailureMechanism);
            Assert.AreEqual("specifiekFaalmechanisme", AssemblyXmlIdentifiers.SpecificFailureMechanism);
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