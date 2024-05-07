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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.ReferenceLines;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms;
using Riskeer.Integration.Forms.Dialogs;
using Riskeer.Integration.Plugin.Properties;
using IntegrationResources = Riskeer.Integration.Data.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// This class is responsible for adding an <see cref="AssessmentSection"/> from a predefined location.
    /// </summary>
    public class AssessmentSectionFromFileHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionFromFileHandler));
        private readonly string shapeFileDirectory = RiskeerSettingsHelper.GetCommonDocumentsRiskeerShapeFileDirectory();

        private readonly IWin32Window dialogParent;

        private readonly HashSet<string> duneAssessmentSections = new HashSet<string>
        {
            "1-1",
            "2-1",
            "3-1",
            "4-1",
            "5-1",
            "13-1",
            "13-3",
            "14-5",
            "14-7",
            "14-9",
            "20-1",
            "25-1",
            "26-1",
            "29-1"
        };

        private IEnumerable<ReferenceLineMeta> referenceLineMetas = new List<ReferenceLineMeta>();

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionFromFileHandler"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionFromFileHandler(IWin32Window dialogParent)
        {
            if (dialogParent == null)
            {
                throw new ArgumentNullException(nameof(dialogParent));
            }

            this.dialogParent = dialogParent;
        }

        /// <summary>
        /// Displays available <see cref="AssessmentSection"/> objects to the user and asks to select one. 
        /// The selected <see cref="AssessmentSection"/>, if any, will be returned.
        /// </summary>
        /// <returns>The selected <see cref="AssessmentSection"/>; or <c>null</c> when cancelled.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="shapeFileDirectory"/> points to an invalid directory.</item>
        /// <item>The path <see cref="shapeFileDirectory"/> does not contain any shape files.</item>
        /// <item cref="CriticalFileReadException">Thrown when the shape file does not contain poly lines.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileValidationException">Thrown when:
        /// <list type="bullet">
        /// <item>The shape file does not contain the required attributes.</item>
        /// <item>The assessment section ids in the shape file are not unique or are missing.</item>
        /// <item>No <see cref="ReferenceLineMeta"/> could be read from the shape file.</item>
        /// <item>The maximum allowable flooding probability is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The signal flooding probability is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The signal flooding probability is larger than the maximum allowable flooding probability.</item>
        /// </list>
        /// </exception>
        public AssessmentSection GetAssessmentSectionFromFile()
        {
            TryReadSourceFiles();
            return GetAssessmentSectionFromDialog();
        }

        #region Dialog

        private AssessmentSection GetAssessmentSectionFromDialog()
        {
            using (ReferenceLineMetaSelectionDialog dialog = CreateReferenceLineMetaSelectionDialogWithItems())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }

                ReferenceLineMeta selectedItem = dialog.SelectedReferenceLineMeta;

                return selectedItem == null
                           ? null
                           : TryCreateAssessmentSection(selectedItem,
                                                        dialog.SelectedMaximumAllowableFloodingProbability,
                                                        dialog.SelectedSignalFloodingProbability,
                                                        dialog.SelectedNormativeProbabilityType);
            }
        }

        private ReferenceLineMetaSelectionDialog CreateReferenceLineMetaSelectionDialogWithItems()
        {
            return new ReferenceLineMetaSelectionDialog(dialogParent, referenceLineMetas);
        }

        #endregion

        #region Create AssessmentSection

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="selectedItem">The selected <see cref="ReferenceLineMeta"/>.</param>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability of the assessment section.</param>
        /// <param name="signalFloodingProbability">The signal flooding probability of the assessment section.</param>
        /// <param name="normativeProbabilityType">The normative probability type of the assessment section.</param>
        /// <returns>The newly created <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="maximumAllowableFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalFloodingProbability"/> is larger than <paramref name="maximumAllowableFloodingProbability"/>.</item>
        /// </list>
        /// </exception>
        private AssessmentSection CreateAssessmentSection(ReferenceLineMeta selectedItem,
                                                          double maximumAllowableFloodingProbability,
                                                          double signalFloodingProbability,
                                                          NormativeProbabilityType normativeProbabilityType)
        {
            AssessmentSection assessmentSection =
                duneAssessmentSections.Contains(selectedItem.AssessmentSectionId)
                    ? new AssessmentSection(AssessmentSectionComposition.Dune, maximumAllowableFloodingProbability, signalFloodingProbability)
                    : new AssessmentSection(AssessmentSectionComposition.Dike, maximumAllowableFloodingProbability, signalFloodingProbability);

            assessmentSection.Name = string.Format(IntegrationResources.AssessmentSection_Id_0, selectedItem.AssessmentSectionId);
            assessmentSection.Id = selectedItem.AssessmentSectionId;

            if (!selectedItem.ReferenceLine.Points.Any())
            {
                log.Warn(Resources.AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_Importing_ReferenceLineFailed);
            }
            else
            {
                assessmentSection.ReferenceLine.SetGeometry(selectedItem.ReferenceLine.Points);
            }

            assessmentSection.FailureMechanismContribution.NormativeProbabilityType = normativeProbabilityType;

            return assessmentSection;
        }

        /// <summary>
        /// Tries to create the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="selectedItem">The selected <see cref="ReferenceLineMeta"/>.</param>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability of the assessment section.</param>
        /// <param name="signalFloodingProbability">The signal flooding probability of the assessment section.</param>
        /// <param name="normativeProbabilityType">The normative probability type of the assessment section.</param>
        /// <returns>The created <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="CriticalFileValidationException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="maximumAllowableFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalFloodingProbability"/> is larger than <paramref name="maximumAllowableFloodingProbability"/>.</item>
        /// </list>
        /// </exception>
        private AssessmentSection TryCreateAssessmentSection(ReferenceLineMeta selectedItem,
                                                             double maximumAllowableFloodingProbability,
                                                             double signalFloodingProbability,
                                                             NormativeProbabilityType normativeProbabilityType)
        {
            try
            {
                return CreateAssessmentSection(selectedItem,
                                               maximumAllowableFloodingProbability,
                                               signalFloodingProbability,
                                               normativeProbabilityType);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                var normValidityRange = new Range<double>(1.0 / 1000000, 1.0 / 10);
                string message = string.Format(Resources.AssessmentSectionFromFileCommandHandler_Unable_to_create_assessmentSection_with_MaximumAllowableFloodingProbability_0_and_SignalFloodingProbability_1_Probabilities_should_be_in_Range_2_,
                                               ProbabilityFormattingHelper.Format(maximumAllowableFloodingProbability),
                                               ProbabilityFormattingHelper.Format(signalFloodingProbability),
                                               normValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));

                throw new CriticalFileValidationException(message, exception);
            }
        }

        #endregion

        #region Validators

        /// <summary>
        /// Tries to read the source files.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="shapeFileDirectory"/> points to an invalid directory.</item>
        /// <item>The path <see cref="shapeFileDirectory"/> does not contain any shape files.</item>
        /// <item cref="CriticalFileReadException">Thrown when the shape file does not contain poly lines.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileValidationException">Thrown when:
        /// <list type="bullet">
        /// <item>The shape file does not contain the required attributes.</item>
        /// <item>The assessment section ids in the shape file are not unique or are missing.</item>
        /// <item>No <see cref="ReferenceLineMeta"/> could be read from the shape file.</item>
        /// </list>
        /// </exception>
        private void TryReadSourceFiles()
        {
            try
            {
                ReadReferenceLineMetas();
            }
            catch (CriticalFileValidationException exception)
            {
                MessageBox.Show(exception.Message, BaseResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Reads all <see cref="ReferenceLineMeta"/> objects from the shape file located at <see cref="shapeFileDirectory"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="shapeFileDirectory"/> points to an invalid directory.</item>
        /// <item>The path <see cref="shapeFileDirectory"/> does not contain any shape files.</item>
        /// <item cref="CriticalFileReadException">Thrown when the shape file does not contain poly lines.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileValidationException">Thrown when:
        /// <list type="bullet">
        /// <item>The shape file does not contain the required attributes.</item>
        /// <item>The assessment section ids in the shape file are not unique or are missing.</item>
        /// <item>No <see cref="ReferenceLineMeta"/> could be read from the shape file.</item>
        /// </list>
        /// </exception>
        private void ReadReferenceLineMetas()
        {
            var importer = new ReferenceLineMetaImporter(shapeFileDirectory);
            referenceLineMetas = importer.GetReferenceLineMetas();

            if (!referenceLineMetas.Any())
            {
                throw new CriticalFileValidationException(Resources.AssessmentSectionFromFileCommandHandler_ValidateReferenceLineMetas_No_referenceLines_in_file);
            }
        }

        #endregion
    }
}