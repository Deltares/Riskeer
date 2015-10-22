﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DelftTools.Shell.Core;

namespace Wti.Data
{
    /// <summary>
    /// Model for performing piping calculations.
    /// </summary>
    public class PipingFailureMechanism : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingFailureMechanism"/> class.
        /// </summary>
        public PipingFailureMechanism()
        {
            SurfaceLines = new ObservableList<RingtoetsPipingSurfaceLine>();
            SoilProfiles = Enumerable.Empty<PipingSoilProfile>();
            PipingData = new PipingData();
        }

        /// <summary>
        /// Gets the available <see cref="RingtoetsPipingSurfaceLine"/> within the scope of the piping failure mechanism.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> SurfaceLines { get; private set; }

        /// <summary>
        /// Gets the <see cref="PipingData"/>, which contains input and output of a piping calculation.
        /// </summary>
        public PipingData PipingData { get; private set; }

        /// <summary>
        /// Gets the available profiles within the scope of the piping failure mechanism.
        /// </summary>
        public IEnumerable SoilProfiles { get; private set; }

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
    }
}