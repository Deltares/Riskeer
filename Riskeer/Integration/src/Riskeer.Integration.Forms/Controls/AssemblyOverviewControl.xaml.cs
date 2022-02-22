// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.Integration.Data;
using Riskeer.Piping.Data;
using Color = System.Windows.Media.Color;

namespace Riskeer.Integration.Forms.Controls
{
    /// <summary>
    /// Interaction logic for <see cref="AssemblyOverviewControl"/>.
    /// </summary>
    public partial class AssemblyOverviewControl
    {
        private readonly AssessmentSection assessmentSection;
        private double widthPerMeter = 5;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyOverviewControl"/>.
        /// </summary>
        public AssemblyOverviewControl(AssessmentSection assessmentSection)
        {
            this.assessmentSection = assessmentSection;
            InitializeComponent();
            
            Render();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            Render();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void Render()
        {
            widthPerMeter = canvas.Width / assessmentSection.ReferenceLine.Length;
            var rowCounter = 0;
            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms().Where(fm => fm.InAssembly))
            {
                if (failureMechanism is PipingFailureMechanism piping)
                {
                    CreateRow(piping, PipingAssemblyFunc, rowCounter++);
                }

                if (failureMechanism is GrassCoverErosionInwardsFailureMechanism gekb)
                {
                    CreateRow(gekb, GrassCoverErosionInwardsAssemblyFunc, rowCounter++);
                }
            }
        }

        private void CreateRow<TFailureMechanism, TSectionResult>(TFailureMechanism failureMechanism,
                                                                  Func<TSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> performAssemblyFunc,
                                                                  int rowNumber)
            where TFailureMechanism : IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            double height = 30;
            double xPosition = 0;
            double yPosition = height * rowNumber;


            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Width = 100,
                Height = height
            };
            Canvas.SetLeft(rectangle, xPosition);
            Canvas.SetTop(rectangle, yPosition);
            canvas.Children.Add(rectangle);

            xPosition = 100;

            foreach (TSectionResult sectionResult in failureMechanism.SectionResults)
            {
                FailureMechanismSectionAssemblyResult sectionAssemblyResult = AssemblyToolHelper.AssembleFailureMechanismSection(
                    sectionResult, sr => performAssemblyFunc(sr, assessmentSection));
                
                double sectionWidth = widthPerMeter * sectionResult.Section.Length;
                
                System.Drawing.Color fillColor = AssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(sectionAssemblyResult.AssemblyGroup);

                rectangle = new System.Windows.Shapes.Rectangle
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(Color.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B)),
                    Width = sectionWidth,
                    Height = height
                };
                Canvas.SetLeft(rectangle, xPosition);
                Canvas.SetTop(rectangle, yPosition);
                canvas.Children.Add(rectangle);

                rectangle.ToolTip = "Tast";

                xPosition += sectionWidth;
            }
        }
        
        #region funcs

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> PipingAssemblyFunc =>
            (sectionResult, assessmentSection) => PipingFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.Piping, assessmentSection);

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverErosionInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverErosionInwards, assessmentSection);

        #endregion
    }
}