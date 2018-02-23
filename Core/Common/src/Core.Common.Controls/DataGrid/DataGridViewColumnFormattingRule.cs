// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Class to define formatting rules on a column.
    /// </summary>
    /// <typeparam name="T">The type of object to use for the formatting rules.</typeparam>
    public class DataGridViewColumnFormattingRule<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DataGridViewColumnFormattingRule{T}"/>
        /// </summary>
        /// <param name="columnIndices">The column indices that the rules apply to.</param>
        /// <param name="rules">The rules to evaluate.</param>
        /// <param name="rulesMeetAction">The action to perform when the rules meet.</param>
        /// <param name="rulesDoNotMeetAction">The action to perform when the rules don't meet.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnIndices"/>,
        /// <paramref name="rules"/> or <paramref name="rulesMeetAction"/> is <c>null</c>.</exception>
        public DataGridViewColumnFormattingRule(int[] columnIndices,
                                                Func<T, bool>[] rules,
                                                Action<int, int> rulesMeetAction,
                                                Action<int, int> rulesDoNotMeetAction)
        {
            if (columnIndices == null)
            {
                throw new ArgumentNullException(nameof(columnIndices));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (rulesMeetAction == null)
            {
                throw new ArgumentNullException(nameof(rulesMeetAction));
            }

            ColumnIndices = columnIndices;
            Rules = rules;
            RulesMeetAction = rulesMeetAction;
            RulesDoNotMeetAction = rulesDoNotMeetAction;
        }

        /// <summary>
        /// Gets the column indices that the rules apply to.
        /// </summary>
        public int[] ColumnIndices { get; }

        /// <summary>
        /// Gets the rules to evaluate.
        /// </summary>
        public Func<T, bool>[] Rules { get; }

        /// <summary>
        /// Gets the action to perform when the rules meet.
        /// </summary>
        public Action<int, int> RulesMeetAction { get; }

        /// <summary>
        /// Gets the action to perform when the rules don't meet.
        /// </summary>
        public Action<int, int> RulesDoNotMeetAction { get; }
    }
}