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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Dialogs;
using Ringtoets.Integration.Forms.Properties;
using IntegrationResources = Ringtoets.Integration.Data.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Forms.Commands
{
    /// <summary>
    /// This class is responsible for adding an <see cref="AssessmentSection"/> from a predefined location.
    /// </summary>
    public class AssessmentSectionFromFileCommandHandler : IAssessmentSectionFromFileCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionFromFileCommandHandler));
        private readonly string shapeFileDirectory = RingtoetsSettingsHelper.GetCommonDocumentsRingtoetsShapeFileDirectory();

        private readonly IWin32Window dialogParent;
        private readonly IProjectOwner projectOwner;
        private readonly IDocumentViewController viewController;
        private IEnumerable<AssessmentSectionSettings> settings;
        private IEnumerable<ReferenceLineMeta> referenceLineMetas = new List<ReferenceLineMeta>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionFromFileCommandHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="projectOwner">The class owning the application project.</param>
        /// <param name="viewController">The document view controller.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionFromFileCommandHandler(IWin32Window dialogParent, IProjectOwner projectOwner, IDocumentViewController viewController)
        {
            if (dialogParent == null)
            {
                throw new ArgumentNullException(nameof(dialogParent));
            }

            if (projectOwner == null)
            {
                throw new ArgumentNullException(nameof(projectOwner));
            }

            if (viewController == null)
            {
                throw new ArgumentNullException(nameof(viewController));
            }

            this.dialogParent = dialogParent;
            this.projectOwner = projectOwner;
            this.viewController = viewController;
        }

        public void AddAssessmentSectionFromFile()
        {
            if (!TryReadSourceFiles())
            {
                return;
            }

            AssessmentSection assessmentSection = GetAssessmentSectionFromDialog();
            var ringtoetsProject = projectOwner.Project as RingtoetsProject;
            if (assessmentSection == null || ringtoetsProject == null)
            {
                return;
            }

            SetAssessmentSectionToProject(ringtoetsProject, assessmentSection);
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
                                                        dialog.SelectedLowerLimitNorm,
                                                        dialog.SelectedSignalingNorm,
                                                        dialog.SelectedNormativeNorm);
            }
        }

        private ReferenceLineMetaSelectionDialog CreateReferenceLineMetaSelectionDialogWithItems()
        {
            return new ReferenceLineMetaSelectionDialog(dialogParent, referenceLineMetas);
        }

        #endregion

        #region Set AssessmentSection to Project

        private static void SetFailureMechanismsValueN(AssessmentSection assessmentSection, int n)
        {
            var roundedN = (RoundedDouble) n;
            assessmentSection.GrassCoverErosionInwards.GeneralInput.N = roundedN;
            assessmentSection.GrassCoverErosionOutwards.GeneralInput.N = roundedN;
            assessmentSection.HeightStructures.GeneralInput.N = roundedN;
        }

        private void SetAssessmentSectionToProject(RingtoetsProject ringtoetsProject, AssessmentSection assessmentSection)
        {
            assessmentSection.Name = GetUniqueForAssessmentSectionName(ringtoetsProject.AssessmentSections, assessmentSection.Name);
            ringtoetsProject.AssessmentSections.Add(assessmentSection);
            ringtoetsProject.NotifyObservers();

            viewController.OpenViewForData(assessmentSection);
        }

        private static string GetUniqueForAssessmentSectionName(IEnumerable<IAssessmentSection> assessmentSections, string baseName)
        {
            return NamingHelper.GetUniqueName(assessmentSections, baseName, a => a.Name);
        }

        #endregion

        #region Create AssessmentSection

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSection"/> with <see cref="AssessmentSectionComposition"/> 
        /// set to <see cref="AssessmentSectionComposition.Dike"/>.
        /// </summary>
        /// <param name="lowerLimitNorm">The lower limit norm of the assessment section.</param>
        /// <param name="signalingNorm">The signaling norm which of the assessment section.</param>
        /// <returns>The newly created <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNorm"/> is larger than <paramref name="lowerLimitNorm"/>.</item>
        /// </list>
        /// </exception>
        private static AssessmentSection CreateDikeAssessmentSection(double lowerLimitNorm, double signalingNorm)
        {
            return new AssessmentSection(AssessmentSectionComposition.Dike, lowerLimitNorm, signalingNorm);
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSection"/> with <see cref="AssessmentSectionComposition"/> 
        /// set to <see cref="AssessmentSectionComposition.Dike"/>.
        /// </summary>
        /// <param name="lowerLimitNorm">The lower limit norm of the assessment section.</param>
        /// <param name="signalingNorm">The signaling norm which of the assessment section.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>The newly created <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNorm"/> is larger than <paramref name="lowerLimitNorm"/>.</item>
        /// </list>
        /// </exception>
        private static AssessmentSection CreateDikeAssessmentSection(double lowerLimitNorm, double signalingNorm, int n)
        {
            AssessmentSection assessmentSection = CreateDikeAssessmentSection(lowerLimitNorm, signalingNorm);
            SetFailureMechanismsValueN(assessmentSection, n);
            return assessmentSection;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSection"/> with <see cref="AssessmentSectionComposition"/> 
        /// set to <see cref="AssessmentSectionComposition.Dune"/>.
        /// </summary>
        /// <param name="lowerLimitNorm">The lower limit norm of the assessment section.</param>
        /// <param name="signalingNorm">The signaling norm which of the assessment section.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>The newly created <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNorm"/> is larger than <paramref name="lowerLimitNorm"/>.</item>
        /// </list>
        /// </exception>
        private static AssessmentSection CreateDuneAssessmentSection(double lowerLimitNorm, double signalingNorm, int n)
        {
            var duneAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dune,
                                                              lowerLimitNorm,
                                                              signalingNorm);
            SetFailureMechanismsValueN(duneAssessmentSection, n);
            return duneAssessmentSection;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="selectedItem">The selected <see cref="ReferenceLineMeta"/>.</param>
        /// <param name="lowerLimitNorm">The lower limit norm of the assessment section.</param>
        /// <param name="signalingNorm">The signaling norm which of the assessment section.</param>
        /// <param name="normativeNorm">The norm type of the assessment section.</param>
        /// <returns>The newly created <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNorm"/> is larger than <paramref name="lowerLimitNorm"/>.</item>
        /// </list>
        /// </exception>
        private AssessmentSection CreateAssessmentSection(ReferenceLineMeta selectedItem,
                                                          double lowerLimitNorm,
                                                          double signalingNorm,
                                                          NormType normativeNorm)
        {
            AssessmentSection assessmentSection;
            AssessmentSectionSettings settingOfSelectedAssessmentSection = settings.FirstOrDefault(s => s.AssessmentSectionId == selectedItem.AssessmentSectionId);
            if (settingOfSelectedAssessmentSection == null)
            {
                log.Warn(Resources.AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_No_settings_found_for_AssessmentSection);
                assessmentSection = CreateDikeAssessmentSection(lowerLimitNorm, signalingNorm);
            }
            else
            {
                assessmentSection = settingOfSelectedAssessmentSection.IsDune
                                        ? CreateDuneAssessmentSection(lowerLimitNorm,
                                                                      signalingNorm,
                                                                      settingOfSelectedAssessmentSection.N)
                                        : CreateDikeAssessmentSection(lowerLimitNorm,
                                                                      signalingNorm,
                                                                      settingOfSelectedAssessmentSection.N);
            }

            assessmentSection.Name = string.Format(IntegrationResources.AssessmentSection_Id_0, selectedItem.AssessmentSectionId);
            assessmentSection.Id = selectedItem.AssessmentSectionId;

            if (!selectedItem.ReferenceLine.Points.Any())
            {
                log.Warn(Resources.AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_Importing_ReferenceLineFailed);
            }
            else
            {
                assessmentSection.ReferenceLine = selectedItem.ReferenceLine;
            }

            assessmentSection.FailureMechanismContribution.NormativeNorm = normativeNorm;

            return assessmentSection;
        }

        private AssessmentSection TryCreateAssessmentSection(ReferenceLineMeta selectedItem,
                                                             double lowerLimitNorm,
                                                             double signalingNorm,
                                                             NormType normativeNorm)
        {
            try
            {
                return CreateAssessmentSection(selectedItem,
                                               lowerLimitNorm,
                                               signalingNorm,
                                               normativeNorm);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                var normValidityRange = new Range<double>(1.0 / 1000000, 1.0 / 10);
                string message = string.Format(Resources.AssessmentSectionFromFileCommandHandler_Unable_to_create_assessmentSection_with_LowerLimitNorm_0_and_SignalingNorm_1_Norms_should_be_in_Range_2_,
                                               ProbabilityFormattingHelper.Format(lowerLimitNorm),
                                               ProbabilityFormattingHelper.Format(signalingNorm),
                                               normValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));

                log.Error(message, exception);
            }

            return null;
        }

        #endregion

        #region Validators

        private bool TryReadSourceFiles()
        {
            ReadAssessmentSectionSettings();

            try
            {
                ReadReferenceLineMetas();
            }
            catch (CriticalFileValidationException exception)
            {
                MessageBox.Show(exception.Message, BaseResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Error(exception.Message, exception.InnerException);
                return false;
            }
            catch (CriticalFileReadException exception)
            {
                log.Error(exception.Message, exception.InnerException);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads all <see cref="ReferenceLineMeta"/> objects from the shape file located at <see cref="shapeFileDirectory"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="shapeFileDirectory"/> points to an invalid directory.</item>
        /// <item>The path <see cref="shapeFileDirectory"/> does not contain any shape files.</item>
        /// <item cref="CriticalFileReadException">Thrown when the shape file does not contain poly lines.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileValidationException">Thrown when:
        /// <list type="bullet">
        /// <item>The shape file does not contain the required attributes.</item>
        /// <item>The assessment section ids in the shape file are not unique or are missing.</item>
        /// <item>No <see cref="ReferenceLineMeta"/> could be read from the shape file.</item>
        /// </list></exception>
        private void ReadReferenceLineMetas()
        {
            var importer = new ReferenceLineMetaImporter(shapeFileDirectory);
            referenceLineMetas = importer.GetReferenceLineMetas();

            if (!referenceLineMetas.Any())
            {
                throw new CriticalFileValidationException(Resources.AssessmentSectionFromFileCommandHandler_ValidateReferenceLineMetas_No_referenceLines_in_file);
            }
        }

        private void ReadAssessmentSectionSettings()
        {
            var reader = new AssessmentSectionSettingsReader();
            settings = reader.ReadSettings();
        }

        #endregion
    }
}