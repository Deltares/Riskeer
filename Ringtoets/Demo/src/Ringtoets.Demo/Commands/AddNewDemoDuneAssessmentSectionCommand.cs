using Core.Common.Gui;

using Ringtoets.Integration.Data;

namespace Ringtoets.Demo.Commands
{
    /// <summary>
    /// Command that adds a new <see cref="DuneAssessmentSection"/> with demo data to the project tree.
    /// </summary>
    public class AddNewDemoDuneAssessmentSectionCommand : GuiCommand
    {
        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        protected override void OnExecute(params object[] arguments)
        {
            var project = Gui.Project;
            project.Items.Add(CreateNewDemoAssessmentSection());
            project.NotifyObservers();
        }

        private DuneAssessmentSection CreateNewDemoAssessmentSection()
        {
            var demoAssessmentSection = new DuneAssessmentSection
            {
                Name = "Demo duintraject"
            };
            return demoAssessmentSection;
        }
    }
}