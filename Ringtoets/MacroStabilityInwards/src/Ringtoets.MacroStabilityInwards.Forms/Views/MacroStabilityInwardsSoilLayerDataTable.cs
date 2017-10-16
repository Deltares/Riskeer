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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="MacroStabilityInwardsSoilLayerData"/> instances
    /// are shown as rows.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerDataTable : DataGridViewControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerDataTable"/>.
        /// </summary>
        public MacroStabilityInwardsSoilLayerDataTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="layers"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="layers">The collection of layers to show.</param>
        public void SetData(IEnumerable<MacroStabilityInwardsSoilLayerData> layers)
        {
            SetDataSource(layers?.Select(l => new FormattedMacroStabilityInwardsSoilLayerDataRow(l)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.MaterialName),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_MaterialName,
                             true);
            AddColorColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.Color),
                           Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_Color);
            AddCheckBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.IsAquifer),
                              Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_IsAquifer,
                              true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.AbovePhreaticLevel),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_AbovePhreaticLevel,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.BelowPhreaticLevel),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_BelowPhreaticLevel,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.ShearStrengthModel),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_ShearStrengthModel,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.Cohesion),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_Cohesion,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.FrictionAngle),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_FrictionAngle,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.ShearStrengthRatio),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_ShearStrengthRatio,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.StrengthIncreaseExponent),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_StrengthIncreaseExponent,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.UsePop),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_UsePop,
                             true);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerDataRow.Pop),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_Pop,
                             true);
        }

        private static string FormatDesignVariable(VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> distribution)
        {
            return $"{distribution.GetDesignValue()} ({RingtoetsCommonFormsResources.NormalDistribution_Mean_DisplayName} = {distribution.Distribution.Mean}, " +
                   $"{RingtoetsCommonFormsResources.NormalDistribution_StandardDeviation_DisplayName} = {distribution.Distribution.CoefficientOfVariation})";
        }

        private class FormattedMacroStabilityInwardsSoilLayerDataRow
        {
            public FormattedMacroStabilityInwardsSoilLayerDataRow(IMacroStabilityInwardsSoilLayerData layerData)
            {
                MaterialName = layerData.MaterialName;
                Color = layerData.Color;
                IsAquifer = layerData.IsAquifer;
                AbovePhreaticLevel = FormatDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(layerData));
                BelowPhreaticLevel = FormatDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(layerData));
                ShearStrengthModel = layerData.ShearStrengthModel;
                Cohesion = FormatDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(layerData));
                FrictionAngle = FormatDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(layerData));
                ShearStrengthRatio = FormatDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(layerData));
                StrengthIncreaseExponent = FormatDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(layerData));
                UsePop = layerData.UsePop;
                Pop = FormatDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(layerData));
            }

            /// <summary>
            /// Gets a value indicating whether or not the <see cref="IMacroStabilityInwardsSoilLayerData"/> is an aquifer.
            /// </summary>
            public bool IsAquifer { get; }

            /// <summary>
            /// Gets the name of the material that was assigned to the <see cref="IMacroStabilityInwardsSoilLayerData"/>.
            /// </summary>
            public string MaterialName { get; }

            /// <summary>
            /// Gets the <see cref="Color"/> that was used to represent the <see cref="IMacroStabilityInwardsSoilLayerData"/>.
            /// </summary>
            public Color Color { get; }

            /// <summary>
            /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.AbovePhreaticLevel"/>.
            /// </summary>
            public string AbovePhreaticLevel { get; }

            /// <summary>
            /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.BelowPhreaticLevel"/>.
            /// </summary>
            public string BelowPhreaticLevel { get; }

            /// <summary>
            /// Gets the <see cref="IMacroStabilityInwardsSoilLayerData.ShearStrengthModel"/> type.
            /// </summary>
            public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { get; }

            /// <summary>
            /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.Cohesion"/>.
            /// </summary>
            public string Cohesion { get; }

            /// <summary>
            /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.FrictionAngle"/>.
            /// </summary>
            public string FrictionAngle { get; }

            /// <summary>
            /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.ShearStrengthRatio"/>.
            /// </summary>
            public string ShearStrengthRatio { get; }

            /// <summary>
            /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.StrengthIncreaseExponent"/>.
            /// </summary>
            public string StrengthIncreaseExponent { get; }

            /// <summary>
            /// Gets a value indicating whether or not the <see cref="IMacroStabilityInwardsSoilLayerData"/> is using POP.
            /// </summary>
            public bool UsePop { get; }

            /// <summary>
            /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.Pop"/>.
            /// </summary>
            public string Pop { get; }
        }
    }
}