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
        private readonly Pen pen;
        private double widthPerMeter;

        public AssemblyResultsOverviewCanvasView(AssessmentSection assessmentSection)
        {
            AssessmentSection = assessmentSection;
            InitializeComponent();
            
            pen = new Pen(Color.Black, 2);
        }

        protected override void OnLoad(EventArgs e)
        {
            Graphics g = pictureBox.CreateGraphics();
            
            widthPerMeter = g.ClipBounds.Width / AssessmentSection.ReferenceLine.Length;
            
            var rowCounter = 0;
            foreach (IFailureMechanism failureMechanism in AssessmentSection.GetFailureMechanisms().Where(fm => fm.InAssembly))
            {
                if (failureMechanism is PipingFailureMechanism piping)
                {
                    CreateRow(piping, PipingAssemblyFunc, rowCounter++, g);
                }

                if (failureMechanism is GrassCoverErosionInwardsFailureMechanism gekb)
                {
                    CreateRow(gekb, GrassCoverErosionInwardsAssemblyFunc, rowCounter++, g);
                }
            }
            
            OnPaint(new PaintEventArgs(g, Rectangle.Ceiling(g.ClipBounds)));
        }

        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        private void CreateRow<TFailureMechanism, TSectionResult>(TFailureMechanism failureMechanism,
                                                                  Func<TSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> performAssemblyFunc,
                                                                  int rowNumber, Graphics graphics)
            where TFailureMechanism : IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            var height = 10;
            var xPosition = 0;
            int yPosition = height * rowNumber;
            
            graphics.DrawRectangle(pen, new Rectangle(xPosition, yPosition, 10, height));

            xPosition = 10;

            foreach (TSectionResult sectionResult in failureMechanism.SectionResults)
            {
                FailureMechanismSectionAssemblyResult sectionAssemblyResult = AssemblyToolHelper.AssembleFailureMechanismSection(
                    sectionResult, sr => performAssemblyFunc(sr, AssessmentSection));

                var sectionWidth = (int) (widthPerMeter * sectionResult.Section.Length);
                graphics.FillRectangle(new SolidBrush(AssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(
                                                          sectionAssemblyResult.AssemblyGroup)),
                                       xPosition, yPosition, sectionWidth, height);

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