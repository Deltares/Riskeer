// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base;
using Ringtoets.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.Observers
{
    /// <summary>
    /// Class that observes all objects in an <typeparamref name="TFailureMechanism"/> related to
    /// its section results.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism to observe.</typeparam>
    /// <typeparam name="TSectionResult">The type of the section results in the failure mechanism.</typeparam>
    public class FailureMechanismResultObserver<TFailureMechanism, TSectionResult> : Observable, IDisposable
        where TFailureMechanism : IFailureMechanism, IHasSectionResults<TSectionResult>
        where TSectionResult : FailureMechanismSectionResult
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer failureMechanismSectionResultObserver;
        private readonly RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult> failureMechanismSectionResultsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultObserver{TFailureMechanism,TSectionResult}"/>.
        /// </summary>
        /// <param name="failureMechanism">The <typeparamref name="TFailureMechanism"/> to observe.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public FailureMechanismResultObserver(TFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            failureMechanismObserver = new Observer(NotifyObservers)
            {
                Observable = failureMechanism
            };

            failureMechanismSectionResultObserver = new Observer(NotifyObservers)
            {
                Observable = failureMechanism.SectionResults
            };

            failureMechanismSectionResultsObserver = new RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult>(
                NotifyObservers,
                sr => sr)
            {
                Observable = failureMechanism.SectionResults
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismObserver.Dispose();
                failureMechanismSectionResultObserver.Dispose();
                failureMechanismSectionResultsObserver.Dispose();
            }
        }
    }
}