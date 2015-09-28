using System.Collections.Generic;

using DelftTools.Shell.Core;
using DelftTools.Utils;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Domain;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.FailureMechanism;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Schematization;

namespace DeltaShell.Plugins.DemoApplicationPlugin.DataObjects
{
    public class WTIProject : INameable, IObservable
    {
        public string Name { get; set; }

        public WTIProject()
        {
            Assessments = new EventedList<IAssessment>();
        }

        public ReferenceLine ReferenceLine { get; set; }

        public HydraulicBoundariesDatabase HydraulicBoundariesDatabase { get; set; }

        public EventedList<IAssessment> Assessments { get; private set; }

        #region IObservable

        private readonly IList<IObserver> observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.UpdateObserver();
            }
        }

        #endregion
    }
}
