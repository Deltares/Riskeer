// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

namespace Riskeer.AssemblyTool.IO.Test
{
    [TestFixture]
    public class AssemblyXmlIdentifiersTest
    {
        [Test]
        public void AssemblyXmlIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("schemaLocation", AssemblyXmlIdentifiers.SchemaLocationIdentifier);
            Assert.AreEqual("https://data.aquo.nl/xsd/BOI/uitwisselmodel/v20220630 https://data.aquo.nl/xsd/BOI/uitwisselmodel/v20220630/BOI_Uitwisselmodel_v1_0.xsd", AssemblyXmlIdentifiers.SchemaLocation);
            Assert.AreEqual("Assembleren-collectie", AssemblyXmlIdentifiers.AssemblyCollection);
            Assert.AreEqual("Waterkeringstelsel", AssemblyXmlIdentifiers.AssessmentSection);
            Assert.AreEqual("typeWaterkeringstelsel", AssemblyXmlIdentifiers.AssessmentSectionType);
            Assert.AreEqual("Beoordelingsproces", AssemblyXmlIdentifiers.AssessmentProcess);
            Assert.AreEqual("beginJaarBeoordelingsronde", AssemblyXmlIdentifiers.StartYear);
            Assert.AreEqual("eindJaarBeoordelingsronde", AssemblyXmlIdentifiers.EndYear);
            Assert.AreEqual("GeneriekFaalmechanisme", AssemblyXmlIdentifiers.GenericFailureMechanism);
            Assert.AreEqual("generiekFaalmechanisme", AssemblyXmlIdentifiers.GenericFailureMechanismName);
            Assert.AreEqual("SpecifiekFaalmechanisme", AssemblyXmlIdentifiers.SpecificFailureMechanism);
            Assert.AreEqual("specifiekFaalmechanisme", AssemblyXmlIdentifiers.SpecificFailureMechanismName);

            Assert.AreEqual("href", AssemblyXmlIdentifiers.Link);
            Assert.AreEqual("beoordeelt", AssemblyXmlIdentifiers.Assesses);
            Assert.AreEqual("uitkomstVan", AssemblyXmlIdentifiers.ResultOf);
            Assert.AreEqual("bepaalt", AssemblyXmlIdentifiers.Determines);
            Assert.AreEqual("onderdeelVan", AssemblyXmlIdentifiers.PartOf);
            Assert.AreEqual("analyseert", AssemblyXmlIdentifiers.Analyzes);
            Assert.AreEqual("geldtVoor", AssemblyXmlIdentifiers.AppliesTo);
            Assert.AreEqual("specificeert", AssemblyXmlIdentifiers.Specifies);
            Assert.AreEqual("afgeleidVan", AssemblyXmlIdentifiers.DerivedFrom);
            Assert.AreEqual("duidt", AssemblyXmlIdentifiers.Indicates);

            Assert.AreEqual("xmlns", AssemblyXmlIdentifiers.XmlnsIdentifier);
            Assert.AreEqual("xlink", AssemblyXmlIdentifiers.XLinkNamespaceIdentifier);
            Assert.AreEqual("http://www.w3.org/1999/xlink", AssemblyXmlIdentifiers.XLinkNamespace);
            Assert.AreEqual("gml", AssemblyXmlIdentifiers.GmlNamespaceIdentifier);
            Assert.AreEqual("http://www.opengis.net/gml/3.2", AssemblyXmlIdentifiers.GmlNamespace);
            Assert.AreEqual("imwap", AssemblyXmlIdentifiers.ImwapNamespaceIdentifier);
            Assert.AreEqual("https://data.aquo.nl/xsd/BOI/imwaproxies/v20220630", AssemblyXmlIdentifiers.ImwapNamespace);
            Assert.AreEqual("uboi", AssemblyXmlIdentifiers.UboiNamespaceIdentifier);
            Assert.AreEqual("https://data.aquo.nl/xsd/BOI/uitwisselmodel/v20220630", AssemblyXmlIdentifiers.UboiNamespace);
            Assert.AreEqual("xsi", AssemblyXmlIdentifiers.XsiNamespaceIdentifier);
            Assert.AreEqual("http://www.w3.org/2001/XMLSchema-instance", AssemblyXmlIdentifiers.XsiNamespace);

            Assert.AreEqual("id", AssemblyXmlIdentifiers.Id);
            Assert.AreEqual("naam", AssemblyXmlIdentifiers.Name);
            Assert.AreEqual("featureMember", AssemblyXmlIdentifiers.FeatureMember);
            Assert.AreEqual("status", AssemblyXmlIdentifiers.Status);

            Assert.AreEqual("LineString", AssemblyXmlIdentifiers.LineString);
            Assert.AreEqual("srsName", AssemblyXmlIdentifiers.CoordinateSystem);
            Assert.AreEqual("posList", AssemblyXmlIdentifiers.Geometry);
            Assert.AreEqual("geometrie2D", AssemblyXmlIdentifiers.Geometry2D);
            Assert.AreEqual("geometrieLijn2D", AssemblyXmlIdentifiers.GeometryLine2D);
            Assert.AreEqual("lengte", AssemblyXmlIdentifiers.Length);

            Assert.AreEqual("Vakindeling", AssemblyXmlIdentifiers.FailureMechanismSectionCollection);
            Assert.AreEqual("Deelvak", AssemblyXmlIdentifiers.FailureMechanismSection);
            Assert.AreEqual("typeWaterkeringsectie", AssemblyXmlIdentifiers.FailureMechanismSectionType);
            Assert.AreEqual("afstandBegin", AssemblyXmlIdentifiers.StartDistance);
            Assert.AreEqual("afstandEinde", AssemblyXmlIdentifiers.EndDistance);
            Assert.AreEqual("Faalanalyse", AssemblyXmlIdentifiers.FailureMechanismSectionAssembly);

            Assert.AreEqual("Veiligheidsoordeel", AssemblyXmlIdentifiers.TotalAssemblyResult);
            Assert.AreEqual("categorie", AssemblyXmlIdentifiers.TotalAssemblyResultAssemblyGroup);
            Assert.AreEqual("assemblagemethodeVeiligheidsoordeel", AssemblyXmlIdentifiers.TotalAssemblyResultAssemblyMethod);
            Assert.AreEqual("faalkans", AssemblyXmlIdentifiers.Probability);
            Assert.AreEqual("assemblagemethodeFaalkans", AssemblyXmlIdentifiers.ProbabilityAssemblyMethod);
            Assert.AreEqual("duidingsklasse", AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyGroup);
            Assert.AreEqual("assemblagemethodeDuidingsklasse", AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyGroupAssemblyMethod);
            Assert.AreEqual("assemblagemethode", AssemblyXmlIdentifiers.AssemblyMethod);
        }
    }
}