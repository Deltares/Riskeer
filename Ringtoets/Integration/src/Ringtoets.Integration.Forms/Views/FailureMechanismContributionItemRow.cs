// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="IFailureMechanism"/>.
    /// </summary>
    internal class FailureMechanismContributionItemRow : IHasColumnStateDefinitions
    {
        /// <summary>
        /// Fired when the row has started updating.
        /// </summary>
        public EventHandler RowUpdated;

        /// <summary>
        /// Fired when the row has finished updating.
        /// </summary>
        public EventHandler RowUpdateDone;

        private readonly IViewCommands viewCommands;
        private readonly IFailureMechanism failureMechanism;
        private readonly double norm;

        private const int isRelevantIndex = 0;
        private const int nameIndex = 1;
        private const int codeIndex = 2;
        private const int contributionIndex = 3;
        private const int probabilitySpaceIndex = 4;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionItemRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism this row contains.</param>
        /// <param name="norm">The norm of the assessment section.</param>
        /// <param name="viewCommands">>Class responsible for exposing high level <see cref="IView"/>
        /// related commands.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal FailureMechanismContributionItemRow(IFailureMechanism failureMechanism, double norm, IViewCommands viewCommands)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.failureMechanism = failureMechanism;
            this.norm = norm;
            this.viewCommands = viewCommands;

            CreateColumnStateDefinitions();

            Update();
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>
            {
                {
                    isRelevantIndex, failureMechanism is OtherFailureMechanism 
                                         ? DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                                         : new DataGridViewColumnStateDefinition()
                },
                {
                    nameIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                },
                {
                    codeIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                },
                {
                    contributionIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                },
                {
                    probabilitySpaceIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                }
            };
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.Assessment"/>.
        /// </summary>
        public string Assessment
        {
            get
            {
                return failureMechanism.Name;
            }
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.AssessmentCode"/>.
        /// </summary>
        public string Code
        {
            get
            {
                return failureMechanism.Code;
            }
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.Contribution"/>.
        /// </summary>
        public double Contribution
        {
            get
            {
                return failureMechanism.Contribution;
            }
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.ProbabilitySpace"/>.
        /// </summary>
        public double ProbabilitySpace
        {
            get
            {
                return 100 / (norm * failureMechanism.Contribution);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="FailureMechanismContributionItem.IsRelevant"/>.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return failureMechanism.IsRelevant;
            }
            set
            {
                if (!value)
                {
                    viewCommands.RemoveAllViewsForItem(failureMechanism);
                }

                failureMechanism.IsRelevant = value;

                Update();

                RowUpdated?.Invoke(this, EventArgs.Empty);
                failureMechanism.NotifyObservers();
                RowUpdateDone?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Update()
        {
            if (!IsRelevant)
            {
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[nameIndex]);
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[codeIndex]);
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[contributionIndex]);
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[probabilitySpaceIndex]);
            }
            else
            {
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[nameIndex]);
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[codeIndex]);
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[contributionIndex]);
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[probabilitySpaceIndex]);
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; private set; }
    }
}