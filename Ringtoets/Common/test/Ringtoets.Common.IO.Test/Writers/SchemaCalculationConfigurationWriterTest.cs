// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Test.Writers
{
    [TestFixture]
    public class SchemaCalculationConfigurationWriterTest
    {
        [Test]
        public void WriteDistribution_WithoutDistributions_ArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteDistribution_WithoutDistributions_ArgumentNullException));

            try
            {
                using (XmlWriter xmlWriter = CreateXmlWriter(filePath))
                {
                    var distribution = new MeanStandardDeviationStochastConfiguration();

                    // Call
                    xmlWriter.WriteDistribution(distribution);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);

                Assert.IsEmpty(actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static XmlWriter CreateXmlWriter(string filePath)
        {
            return XmlWriter.Create(filePath, new XmlWriterSettings
            {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Fragment
            });
        }
    }

    public static class XmlWriterExtensions
    {
        public static void WriteDistribution(this XmlWriter writer, MeanStandardDeviationStochastConfiguration distribution)
        {
            
        }
    }
}