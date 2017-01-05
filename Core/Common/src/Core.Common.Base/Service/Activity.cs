// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Base.Properties;
using log4net;

namespace Core.Common.Base.Service
{
    /// <summary>
    /// Abstract class that can be derived for performing activities (like calculations, data imports, data exports, etc.).
    /// The regular workflow for completely performing an <see cref="Activity"/> is: <see cref="Run"/> -> <see cref="Finish"/>.
    /// <see cref="Cancel"/> can be called for cancelling a running <see cref="Activity"/>.
    /// </summary>
    /// <remarks>
    /// By convention, only <see cref="Finish"/> should contain UI thread related logic.
    /// </remarks>
    public abstract class Activity
    {
        private readonly ILog log = LogManager.GetLogger(typeof(Activity));

        private string progressText;

        /// <summary>
        /// Event for notifying progress changes.
        /// </summary>
        public event EventHandler ProgressChanged;

        /// <summary>
        /// Constructs a new <see cref="Activity"/>.
        /// </summary>
        protected Activity()
        {
            Name = "";
            State = ActivityState.None;
            LogMessages = new Collection<string>();
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="Activity"/>.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="ActivityState"/> of the <see cref="Activity"/>.
        /// </summary>
        public ActivityState State { get; protected set; }

        /// <summary>
        /// Gets or sets the progress text of the <see cref="Activity"/>.
        /// <see cref="ProgressChanged"/> listeners are notified when the progress text is set.
        /// </summary>
        public string ProgressText
        {
            get
            {
                return progressText;
            }
            protected set
            {
                progressText = value;

                OnProgressChanged();
            }
        }

        /// <summary>
        /// Gets or sets the collection of log messages of the <see cref="Activity"/> (which are appended while performing the <see cref="Activity"/>).
        /// </summary>
        /// <remarks>
        /// Derived classes themselves are responsible for clearing the collection of log messages.
        /// </remarks>
        public ICollection<string> LogMessages { get; private set; }

        /// <summary>
        /// This method resets <see cref="State"/> to <see cref="ActivityState.None"/> and then runs the <see cref="Activity"/>.
        /// The <see cref="State"/> of a successfully run <see cref="Activity"/> will become <see cref="ActivityState.Executed"/>.
        /// When the <see cref="Activity"/> run fails, the <see cref="State"/> will become <see cref="ActivityState.Failed"/>.
        /// </summary>
        public void Run()
        {
            State = ActivityState.None;

            ChangeState(OnRun, ActivityState.Executed);
        }

        /// <summary>
        /// This method cancels a running <see cref="Activity"/>.
        /// The <see cref="State"/> of a successfully cancelled <see cref="Activity"/> will become <see cref="ActivityState.Canceled"/>.
        /// When the <see cref="Activity"/> cancel action fails, the <see cref="State"/> will become <see cref="ActivityState.Failed"/>.
        /// </summary>
        public void Cancel()
        {
            ChangeState(OnCancel, ActivityState.Canceled);
        }

        /// <summary>
        /// This method finishes an <see cref="Activity"/> that successfully ran.
        /// Successfully ran activities can be identified by a <see cref="State"/> equal to <see cref="ActivityState.Executed"/>.
        /// The <see cref="State"/> of a successfully finished <see cref="Activity"/> will become <see cref="ActivityState.Finished"/>.
        /// When the <see cref="Activity"/> finish action fails, the <see cref="State"/> will become <see cref="ActivityState.Failed"/>.
        /// </summary>
        public void Finish()
        {
            ChangeState(OnFinish, State == ActivityState.Executed ? ActivityState.Finished : State); // If relevant, preserve the previous state

            if (State == ActivityState.Finished)
            {
                log.InfoFormat(Resources.Activity_Finish_Execution_of_ActivityName_0_has_succeeded, Name);
            }

            if (State == ActivityState.Canceled)
            {
                log.WarnFormat(Resources.Activity_Finish_Execution_of_ActivityName_0_has_been_canceled, Name);
            }

            if (State == ActivityState.Failed)
            {
                log.ErrorFormat(Resources.Activity_Finish_Execution_of_ActivityName_0_has_failed, Name);
            }

            if (State == ActivityState.Skipped)
            {
                log.InfoFormat(Resources.Activity_Finish_Execution_of_ActivityName_0_has_been_skipped, Name);
            }
        }

        /// <summary>
        /// This template method provides the actual run logic (it is called within <see cref="Run"/>).
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="State"/> should be set to <see cref="ActivityState.Failed"/> when one or more errors occur.</para>
        /// <para>By convention, the implementation of this method should not contain UI thread related logic.</para>
        /// <para>Implementations of this method are allowed to throw exceptions of any kind.</para>
        /// </remarks>
        protected abstract void OnRun();

        /// <summary>
        /// This template method provides the actual cancel logic (it is called within <see cref="Cancel"/>).
        /// </summary>
        /// <remarks>
        /// <para>By convention, the implementation of this method should not contain UI thread related logic.</para>
        /// <para>Implementations of this method are allowed to throw exceptions of any kind.</para>
        /// </remarks>
        protected abstract void OnCancel();

        /// <summary>
        /// This template method provides the actual finish logic (it is called within <see cref="Finish"/>).
        /// </summary>
        /// <remarks>
        /// <para>By convention, only the implementation of this method might contain UI thread related logic.</para>
        /// <para>Implementations of this method are allowed to throw exceptions of any kind.</para>
        /// </remarks>
        protected abstract void OnFinish();

        private void OnProgressChanged()
        {
            ProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ChangeState(Action transitionAction, ActivityState stateAfter)
        {
            try
            {
                transitionAction();

                if (State == ActivityState.Failed || State == ActivityState.Canceled || State == ActivityState.Skipped)
                {
                    return;
                }
            }
            catch (Exception)
            {
                State = ActivityState.Failed;
                return;
            }

            State = stateAfter;
        }
    }
}