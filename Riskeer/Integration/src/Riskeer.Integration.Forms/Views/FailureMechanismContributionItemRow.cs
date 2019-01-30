// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="IFailureMechanism"/>.
    /// </summary>
    internal class FailureMechanismContributionItemRow : IHasColumnStateDefinitions
    {
        private readonly IViewCommands viewCommands;
        private readonly IFailureMechanism failureMechanism;
        private readonly FailureMechanismContribution failureMechanismContribution;

        private readonly int isRelevantColumnIndex;
        private readonly int nameColumnIndex;
        private readonly int codeColumnIndex;
        private readonly int contributionColumnIndex;
        private readonly int probabilitySpaceColumnIndex;

        /// <summary>
        /// Fired when the row has started updating.
        /// </summary>
        public EventHandler RowUpdated;

        /// <summary>
        /// Fired when the row has finished updating.
        /// </summary>
        public EventHandler RowUpdateDone;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionItemRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism this row contains.</param>
        /// <param name="failureMechanismContribution">The failure mechanism contribution to get the norm from.</param>
        /// <param name="viewCommands">Class responsible for exposing high level <see cref="IView"/>
        /// related commands.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="FailureMechanismContributionItemRow"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal FailureMechanismContributionItemRow(IFailureMechanism failureMechanism, FailureMechanismContribution failureMechanismContribution,
                                                     IViewCommands viewCommands, ConstructionProperties constructionProperties)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (failureMechanismContribution == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismContribution));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            this.failureMechanism = failureMechanism;
            this.failureMechanismContribution = failureMechanismContribution;
            this.viewCommands = viewCommands;

            isRelevantColumnIndex = constructionProperties.IsRelevantColumnIndex;
            nameColumnIndex = constructionProperties.NameColumnIndex;
            codeColumnIndex = constructionProperties.CodeColumnIndex;
            contributionColumnIndex = constructionProperties.ContributionColumnIndex;
            probabilitySpaceColumnIndex = constructionProperties.ProbabilitySpaceColumnIndex;

            CreateColumnStateDefinitions();

            Update();
        }

        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        public string Name
        {
            get
            {
                return failureMechanism.Name;
            }
        }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public string Code
        {
            get
            {
                return failureMechanism.Code;
            }
        }

        /// <summary>
        /// Gets the contribution of the failure mechanism.
        /// </summary>
        public double Contribution
        {
            get
            {
                return failureMechanism.Contribution;
            }
        }

        /// <summary>
        /// Gets the probability space of the failure mechanism.
        /// </summary>
        public double ProbabilitySpace
        {
            get
            {
                return 100 / (failureMechanismContribution.Norm * failureMechanism.Contribution);
            }
        }

        /// <summary>
        /// Gets or sets whether the failure mechanism is relevant.
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

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; private set; }

        public void Update()
        {
            if (!IsRelevant)
            {
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[nameColumnIndex]);
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[codeColumnIndex]);
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[contributionColumnIndex]);
                FailureMechanismSectionResultRowHelper.DisableColumn(ColumnStateDefinitions[probabilitySpaceColumnIndex]);
            }
            else
            {
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[nameColumnIndex], true);
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[codeColumnIndex], true);
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[contributionColumnIndex], true);
                FailureMechanismSectionResultRowHelper.EnableColumn(ColumnStateDefinitions[probabilitySpaceColumnIndex], true);
            }
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>
            {
                {
                    isRelevantColumnIndex, failureMechanism is OtherFailureMechanism
                                               ? DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                                               : new DataGridViewColumnStateDefinition()
                },
                {
                    nameColumnIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                },
                {
                    codeColumnIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                },
                {
                    contributionColumnIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                },
                {
                    probabilitySpaceColumnIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition()
                }
            };
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="FailureMechanismContributionItemRow"/>.
        /// </summary>
        internal class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the relevant column index.
            /// </summary>
            public int IsRelevantColumnIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the name column index.
            /// </summary>
            public int NameColumnIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the code column index.
            /// </summary>
            public int CodeColumnIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the contribution column index.
            /// </summary>
            public int ContributionColumnIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the probability space column index.
            /// </summary>
            public int ProbabilitySpaceColumnIndex { internal get; set; }
        }
    }
}