using System.ComponentModel;
using System.Configuration;

namespace Core.Common.Base
{
    // Decoration class for settings with IsDirty flag (speed-up exit)
    public class ApplicationCoreSettings : ApplicationSettingsBase
    {
        private readonly ApplicationSettingsBase child;

        public ApplicationCoreSettings(ApplicationSettingsBase child)
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