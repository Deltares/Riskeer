// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using OldAssemblyXmlIdentifiers = Riskeer.AssemblyTool.IO.Model.AssemblyXmlIdentifiers;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class AssemblyXmlIdentifiersTest
    {
        [Test]
        public void AssemblyXmlIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("Assemblage", OldAssemblyXmlIdentifiers.Assembly);
            Assert.AreEqual("assemblagemethode", OldAssemblyXmlIdentifiers.AssemblyMethod);
            Assert.AreEqual("assemblagemethodeVeiligheidsoordeel", OldAssemblyXmlIdentifiers.TotalAssemblyResultAssemblyMethod);
            Assert.AreEqual("assemblagemethodeFaalkans", OldAssemblyXmlIdentifiers.ProbabilityAssemblyMethod);
            Assert.AreEqual("assemblagemethodeDuidingsklasse", OldAssemblyXmlIdentifiers.AssemblyGroupAssemblyMethod);
            Assert.AreEqual("http://localhost/standaarden/assemblage", OldAssemblyXmlIdentifiers.AssemblyNamespace);
            Assert.AreEqual("asm", OldAssemblyXmlIdentifiers.AssemblyNamespaceIdentifier);
            Assert.AreEqual("Beoordelingsproces", OldAssemblyXmlIdentifiers.AssessmentProcess);
            Assert.AreEqual("BeoordelingsprocesID", OldAssemblyXmlIdentifiers.AssessmentProcessId);
            Assert.AreEqual("BeoordelingsprocesIDRef", OldAssemblyXmlIdentifiers.AssessmentProcessIdRef);
            Assert.AreEqual("categorie", OldAssemblyXmlIdentifiers.AssessmentSectionAssemblyGroup);
            Assert.AreEqual("WaterkeringstelselIDRef", OldAssemblyXmlIdentifiers.AssessmentSectionIdRef);
            Assert.AreEqual("typeWaterkeringstelsel", OldAssemblyXmlIdentifiers.AssessmentSectionType);
            Assert.AreEqual("Waterkeringstelsel", OldAssemblyXmlIdentifiers.AssessmentSection);

            Assert.AreEqual("boundedBy", OldAssemblyXmlIdentifiers.BoundedBy);

            Assert.AreEqual("FaalanalyseGecombineerd", OldAssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssembly);
            Assert.AreEqual("FaalanalyseGecombineerdID", OldAssemblyXmlIdentifiers.CombinedFailureMechanismSectionAssemblyId);
            Assert.AreEqual("analyseGecombineerdDeelvak", OldAssemblyXmlIdentifiers.CombinedCombinedSectionResult);
            Assert.AreEqual("analyseVak", OldAssemblyXmlIdentifiers.SectionResult);
            Assert.AreEqual("analyseDeelvak", OldAssemblyXmlIdentifiers.CombinedSectionFailureMechanismResult);
            Assert.AreEqual("srsName", OldAssemblyXmlIdentifiers.CoordinateSystem);

            Assert.AreEqual("afstandEinde", OldAssemblyXmlIdentifiers.EndDistance);
            Assert.AreEqual("eindJaarBeoordelingsronde", OldAssemblyXmlIdentifiers.EndYear);
            Assert.AreEqual("Envelope", OldAssemblyXmlIdentifiers.Envelope);

            Assert.AreEqual("Faalmechanisme", OldAssemblyXmlIdentifiers.FailureMechanism);
            Assert.AreEqual("analyseFaalmechanisme", OldAssemblyXmlIdentifiers.FailureMechanismAssemblyResult);
            Assert.AreEqual("FaalmechanismeID", OldAssemblyXmlIdentifiers.FailureMechanismId);
            Assert.AreEqual("FaalmechanismeIDRef", OldAssemblyXmlIdentifiers.FailureMechanismIdRef);
            Assert.AreEqual("Deelvak", OldAssemblyXmlIdentifiers.FailureMechanismSection);
            Assert.AreEqual("WaterkeringsectieIDRef", OldAssemblyXmlIdentifiers.FailureMechanismSectionIdRef);
            Assert.AreEqual("Faalanalyse", OldAssemblyXmlIdentifiers.FailureMechanismSectionAssembly);
            Assert.AreEqual("FaalanalyseID", OldAssemblyXmlIdentifiers.FailureMechanismSectionAssemblyId);
            Assert.AreEqual("duidingsklasse", OldAssemblyXmlIdentifiers.FailureMechanismSectionAssemblyGroup);
            Assert.AreEqual("Vakindeling", OldAssemblyXmlIdentifiers.FailureMechanismSectionCollection);
            Assert.AreEqual("VakindelingID", OldAssemblyXmlIdentifiers.FailureMechanismSectionCollectionId);
            Assert.AreEqual("VakindelingIDRef", OldAssemblyXmlIdentifiers.FailureMechanismSectionCollectionIdRef);
            Assert.AreEqual("typeWaterkeringsectie", OldAssemblyXmlIdentifiers.FailureMechanismSectionType);
            Assert.AreEqual("typeFaalmechanisme", OldAssemblyXmlIdentifiers.FailureMechanismType);
            Assert.AreEqual("generiekFaalmechanisme", OldAssemblyXmlIdentifiers.GenericFailureMechanism);
            Assert.AreEqual("specifiekFaalmechanisme", OldAssemblyXmlIdentifiers.SpecificFailureMechanism);
            Assert.AreEqual("featureMember", OldAssemblyXmlIdentifiers.FeatureMember);

            Assert.AreEqual("posList", OldAssemblyXmlIdentifiers.Geometry);
            Assert.AreEqual("geometrie2D", OldAssemblyXmlIdentifiers.Geometry2D);
            Assert.AreEqual("geometrieLijn2D", OldAssemblyXmlIdentifiers.GeometryLine2D);
            Assert.AreEqual("http://www.opengis.net/gml/3.2", OldAssemblyXmlIdentifiers.GmlNamespace);
            Assert.AreEqual("gml", OldAssemblyXmlIdentifiers.GmlNamespaceIdentifier);

            Assert.AreEqual("id", OldAssemblyXmlIdentifiers.Id);

            Assert.AreEqual("lengte", OldAssemblyXmlIdentifiers.Length);
            Assert.AreEqual("LineString", OldAssemblyXmlIdentifiers.LineString);
            Assert.AreEqual("lowerCorner", OldAssemblyXmlIdentifiers.LowerCorner);

            Assert.AreEqual("naam", OldAssemblyXmlIdentifiers.Name);

            Assert.AreEqual("faalkans", OldAssemblyXmlIdentifiers.Probability);

            Assert.AreEqual("afstandBegin", OldAssemblyXmlIdentifiers.StartDistance);
            Assert.AreEqual("beginJaarBeoordelingsronde", OldAssemblyXmlIdentifiers.StartYear);
            Assert.AreEqual("status", OldAssemblyXmlIdentifiers.Status);

            Assert.AreEqual("Veiligheidsoordeel", OldAssemblyXmlIdentifiers.TotalAssemblyResult);
            Assert.AreEqual("VeiligheidsoordeelID", OldAssemblyXmlIdentifiers.TotalAssemblyResultId);
            Assert.AreEqual("VeiligheidsoordeelIDRef", OldAssemblyXmlIdentifiers.TotalAssemblyResultIdRef);

            Assert.AreEqual("uom", OldAssemblyXmlIdentifiers.UnitOfMeasure);
            Assert.AreEqual("upperCorner", OldAssemblyXmlIdentifiers.UpperCorner);
        }
    }
}