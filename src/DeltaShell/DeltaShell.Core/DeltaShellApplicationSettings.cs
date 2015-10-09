using System.ComponentModel;
using System.Configuration;

namespace DeltaShell.Core
{
    // Decoration class for settings with IsDirty flag (speed-up exit)
    public class DeltaShellApplicationSettings : ApplicationSettingsBase
    {
        private readonly ApplicationSettingsBase child;

        public DeltaShellApplicationSettings(ApplicationSettingsBase child)
        {
            this.child = child;
        }

        public override object this[string propertyName]
        {
            get
            {
                return child[propertyName];
            }
            set
            {
                child[propertyName] = value;
                IsDirty = true;
                //don't forget to publish changes
                OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsDirty { get; private set; }
    }
}