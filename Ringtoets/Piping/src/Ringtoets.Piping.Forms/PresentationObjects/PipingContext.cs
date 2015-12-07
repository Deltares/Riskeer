using System;
using System.Collections.Generic;

using Core.Common.Base;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object representing all required piping knowledge to configure and create
    /// piping related objects. It'll delegate observable behavior to the wrapped data object.
    /// </summary>
    public abstract class PipingContext<T> : IObservable where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingContext{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The concrete data instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="soilProfiles">The soil profiles available within the piping context.</param>
        /// <exception cref="System.ArgumentNullException">When any input parameter is null.</exception>
        protected PipingContext(
            T wrappedData,
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines,
            IEnumerable<PipingSoilProfile> soilProfiles)
        {
            AssertInputsAreNotNull(wrappedData, surfaceLines, soilProfiles);

            WrappedData = wrappedData;
            AvailablePipingSurfaceLines = surfaceLines;
            AvailablePipingSoilProfiles = soilProfiles;
        }

        public override bool Equals(object obj)
        {
            var context = obj as PipingContext<T>;
            if (context != null)
            {
                return WrappedData.Equals(context.WrappedData);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Gets the available piping surface lines in order for the user to select one to 
        /// set <see cref="PipingInput.SurfaceLine"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> AvailablePipingSurfaceLines { get; private set; }

        /// <summary>
        /// Gets the available piping soil profiles in order for the user to select one to 
        /// set <see cref="PipingInput.SoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> AvailablePipingSoilProfiles { get; private set; }

        /// <summary>
        /// Gets the concrete data instance wrapped by this context object.
        /// </summary>
        public T WrappedData { get; private set; }

        /// <summary>
        /// Asserts the inputs are not null.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <param name="surfaceLines">The surface lines.</param>
        /// <param name="soilProfiles">The soil profiles.</param>
        /// <exception cref="System.ArgumentNullException">When any input parameter is null.</exception>
        private static void AssertInputsAreNotNull(object wrappedData, object surfaceLines, object soilProfiles)
        {
            if (wrappedData == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_WrappedData);
                throw new ArgumentNullException("wrappedData", message);
            }
            if (surfaceLines == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_Surfacelines);
                throw new ArgumentNullException("surfaceLines", message);
            }
            if (soilProfiles == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_Soilprofiles);
                throw new ArgumentNullException("soilProfiles", message);
            }
        }

        #region IObservable

        public void Attach(IObserver observer)
        {
            WrappedData.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            WrappedData.Detach(observer);
        }

        public void NotifyObservers()
        {
            WrappedData.NotifyObservers();
        }

        #endregion
    }
}