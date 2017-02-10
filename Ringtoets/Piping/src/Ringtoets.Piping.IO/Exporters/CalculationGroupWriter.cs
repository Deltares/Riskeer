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
using System.Xml;
using Core.Common.IO.Exceptions;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Piping.Data;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Exporters
{
    /// <summary>
    /// Xml file writer for writing <see cref="CalculationGroup"/> objects to *.xml file.
    /// </summary>
    internal static class CalculationGroupWriter
    {
        /// <summary>
        /// Writes the calculation group to a xml file.
        /// </summary>
        /// <param name="rootCalculationGroup">The root calculation group to be written to the file.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        internal static void WriteCalculationGroups(CalculationGroup rootCalculationGroup, string filePath)
        {
            if (rootCalculationGroup == null)
            {
                throw new ArgumentNullException(nameof(rootCalculationGroup));
            }
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t"
                };

                using (XmlWriter writer = XmlWriter.Create(filePath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("root");

                    WriteChildElements(rootCalculationGroup, writer);

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilsResources.Error_General_output_error_0, filePath), e);
            }
        }

        private static void WriteChildElements(CalculationGroup calculationGroup, XmlWriter writer)
        {
            foreach (ICalculationBase child in calculationGroup.Children)
            {
                var innerGroup = child as CalculationGroup;
                if (innerGroup != null)
                {
                    WriteCalculationGroupToFile(innerGroup, writer);
                }

                var calculation = child as PipingCalculation;
                if (calculation != null)
                {
                    WriteCalculationToFile(calculation, writer);
                }
            }
        }

        private static void WriteCalculationGroupToFile(CalculationGroup calculationGroup, XmlWriter writer)
        {
            writer.WriteStartElement("folder");
            writer.WriteAttributeString("naam", calculationGroup.Name);

            WriteChildElements(calculationGroup, writer);

            writer.WriteEndElement();
        }

        private static void WriteCalculationToFile(PipingCalculation calculation, XmlWriter writer)
        {
            writer.WriteStartElement("berekening");
            writer.WriteAttributeString("naam", calculation.Name);

            PipingInput calculationInputParameters = calculation.InputParameters;

            if (calculationInputParameters.UseAssessmentLevelManualInput)
            {
                writer.WriteElementString("toetspeil", calculationInputParameters.AssessmentLevel.ToString());
            }
            else
            {
                if (calculationInputParameters.HydraulicBoundaryLocation != null)
                {
                    writer.WriteElementString("hrlocatie", calculationInputParameters.HydraulicBoundaryLocation.Name);
                }
            }

            if (calculationInputParameters.SurfaceLine != null)
            {
                writer.WriteElementString("profielschematisatie", calculationInputParameters.SurfaceLine.Name);
                writer.WriteElementString("intredepunt", calculationInputParameters.EntryPointL.ToString());
                writer.WriteElementString("uittredepunt", calculationInputParameters.ExitPointL.ToString());
            }

            if (calculationInputParameters.StochasticSoilModel != null)
            {
                writer.WriteElementString("ondergrondmodel", calculationInputParameters.StochasticSoilModel.Name);

                if (calculationInputParameters.StochasticSoilProfile?.SoilProfile != null)
                {
                    writer.WriteElementString("ondergrondschematisatie", calculationInputParameters.StochasticSoilProfile.SoilProfile.Name);
                }
            }

            WriteDistribution(calculationInputParameters.PhreaticLevelExit, "polderpeil", writer);
            WriteDistribution(calculationInputParameters.DampingFactorExit, "dempingsfactor", writer);


            writer.WriteEndElement();
        }

        private static void WriteDistribution(IDistribution distribution, string elementName, XmlWriter writer)
        {
            writer.WriteStartElement("stochast");
            writer.WriteAttributeString("naam", elementName);

            writer.WriteElementString("verwachtingswaarde", distribution.Mean.ToString());
            writer.WriteElementString("standaardafwijking", distribution.StandardDeviation.ToString());

            writer.WriteEndElement();
        }
    }
}