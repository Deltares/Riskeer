using System.Collections.Generic;
using System.ComponentModel;

namespace DelftTools.Controls
{
    public interface IWizardDialog : IDialog
    {
        void AddPage(IComponent page, string title, string description);

        void RemovePage(IComponent page);

        void UpdateNavigationButtons();

        IList<IComponent> Pages { get; }
        
        IList<string> PageTitles { get; }
        
        IList<string> PageDescriptions { get; }
        
        IComponent CurrentPage { get; set; }

        string WelcomeMessage { get; set; }
        
        string FinishedPageMessage { get; set; }
    }
}