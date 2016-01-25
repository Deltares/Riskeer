using Core.Common.Gui;
using Ringtoets.Integration.Data;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// Command that adds a new <see cref="DuneAssessmentSection"/> with demo data to the project tree.
    /// </summary>
    public class AddNewDemoDuneAssessmentSectionCommand : IGuiCommand
    {
        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public IGui Gui { get; set; }

        public void Execute(params object[] arguments)
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