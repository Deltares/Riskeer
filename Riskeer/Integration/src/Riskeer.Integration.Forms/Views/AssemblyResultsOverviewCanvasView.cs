using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.Integration.Data;
using Riskeer.Piping.Data;

namespace Riskeer.Integration.Forms.Views
{
    public partial class AssemblyResultsOverviewCanvasView : UserControl, IView
    {
        private double widthPerMeter;

        public AssemblyResultsOverviewCanvasView(AssessmentSection assessmentSection)
        {
            AssessmentSection = assessmentSection;
            InitializeComponent();
        }

        protected override void OnResize(EventArgs e)
        {
            Render();
            base.OnResize(e);
        }

        private void Render()
        {    
            using (var image = new Bitmap(pictureBox.Width, pictureBox.Height))
            using (var graphics = Graphics.FromImage(image))
            using (var pen = new Pen(Color.Black, 2))
            {
                widthPerMeter = image.Width / AssessmentSection.ReferenceLine.Length;
                var rowCounter = 0;
                foreach (IFailureMechanism failureMechanism in AssessmentSection.GetFailureMechanisms().Where(fm => fm.InAssembly))
                {
                    if (failureMechanism is PipingFailureMechanism piping)
                    {
                        CreateRow(piping, PipingAssemblyFunc, rowCounter++, graphics, pen);
                    }
                    if (failureMechanism is GrassCoverErosionInwardsFailureMechanism gekb)
                    {
                        CreateRow(gekb, GrassCoverErosionInwardsAssemblyFunc, rowCounter++, graphics, pen);
                    }
                }
                pictureBox.Image?.Dispose();
                pictureBox.Image = (Bitmap) image.Clone();
            }
        }

        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        private void CreateRow<TFailureMechanism, TSectionResult>(TFailureMechanism failureMechanism,
                                                                  Func<TSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc,
                                                                  int rowNumber, Graphics graphics, Pen pen)
            where TFailureMechanism : IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            var height = 30;
            var xPosition = 0;
            int yPosition = height * rowNumber;
            
            graphics.DrawRectangle(pen, new Rectangle(xPosition, yPosition, 100, height));

            xPosition = 100;

            foreach (TSectionResult sectionResult in failureMechanism.SectionResults)
            {
                FailureMechanismSectionAssemblyResult sectionAssemblyResult = AssemblyToolHelper.AssembleFailureMechanismSection(
                    sectionResult, sr => performAssemblyFunc(sr, AssessmentSection));

                var sectionWidth = (int) (widthPerMeter * sectionResult.Section.Length);
                graphics.DrawRectangle(pen, new Rectangle(xPosition, yPosition, sectionWidth, height));
                graphics.FillRectangle(new SolidBrush(FailureMechanismSectionAssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyGroupColor(
                                                          sectionAssemblyResult.FailureMechanismSectionAssemblyGroup)),
                                       xPosition, yPosition, sectionWidth, height);

                xPosition += sectionWidth;
            }
        }

        #region funcs

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResultWrapper> PipingAssemblyFunc =>
            (sectionResult, assessmentSection) => PipingFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.Piping, assessmentSection);

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResultWrapper> GrassCoverErosionInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverErosionInwards, assessmentSection);

        #endregion
    }
}