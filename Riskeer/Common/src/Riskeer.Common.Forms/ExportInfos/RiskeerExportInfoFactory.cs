﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.ExportInfos
{
    /// <summary>
    /// Factory for creating standard <see cref="ExportInfo"/> objects. 
    /// </summary>
    public static class RiskeerExportInfoFactory
    {
        /// <summary>
        /// Creates a <see cref="ExportInfo"/> object for a calculation group configuration
        /// of the type <typeparamref name="TCalculationGroupContext"/>.
        /// </summary>
        /// <typeparam name="TCalculationGroupContext">The type of calculation group context
        /// to create the <see cref="ExportInfo"/> for.</typeparam>
        /// <param name="createFileExporter">The function to create the relevant exporter.</param>
        /// <param name="isEnabled">The function to enable the exporter.</param>
        /// <param name="inquiryHelper">Helper responsible for performing information inquiries.</param>
        /// <returns>An <see cref="ExportInfo"/> object.</returns>
        public static ExportInfo<TCalculationGroupContext> CreateCalculationGroupConfigurationExportInfo<TCalculationGroupContext>(
            Func<TCalculationGroupContext, string, IFileExporter> createFileExporter,
            Func<TCalculationGroupContext, bool> isEnabled,
            IInquiryHelper inquiryHelper)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            return new ExportInfo<TCalculationGroupContext>
            {
                Name = context => Resources.CalculationConfigurationExporter_DisplayName,
                Extension = Resources.DataTypeDisplayName_xml_file_filter_Extension,
                IsEnabled = isEnabled,
                CreateFileExporter = createFileExporter,
                GetExportPath = () => ExportHelper.GetFilePath(
                    inquiryHelper, new FileFilterGenerator(Resources.DataTypeDisplayName_xml_file_filter_Extension,
                                                           Resources.DataTypeDisplayName_xml_file_filter_Description))
            };
        }

        /// <summary>
        /// Creates a <see cref="ExportInfo"/> object for a calculation configuration
        /// of the type <typeparamref name="TCalculationContext"/>.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of calculation context
        /// to create the <see cref="ExportInfo"/> for.</typeparam>
        /// <param name="createFileExporter">The function to create the relevant exporter.</param>
        /// <param name="inquiryHelper">Helper responsible for performing information inquiries.</param>
        /// <returns>An <see cref="ExportInfo"/> object.</returns>
        public static ExportInfo<TCalculationContext> CreateCalculationConfigurationExportInfo<TCalculationContext>(
            Func<TCalculationContext, string, IFileExporter> createFileExporter,
            IInquiryHelper inquiryHelper)
            where TCalculationContext : ICalculationContext<ICalculation, IFailureMechanism>
        {
            return new ExportInfo<TCalculationContext>
            {
                Name = context => Resources.CalculationConfigurationExporter_DisplayName,
                Extension = Resources.DataTypeDisplayName_xml_file_filter_Extension,
                CreateFileExporter = createFileExporter,
                IsEnabled = context => true,
                GetExportPath = () => ExportHelper.GetFilePath(
                    inquiryHelper, new FileFilterGenerator(Resources.DataTypeDisplayName_xml_file_filter_Extension,
                                                           Resources.DataTypeDisplayName_xml_file_filter_Description))
            };
        }
    }
}