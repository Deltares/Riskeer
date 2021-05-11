// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Gui.Plugin;

namespace Core.Gui.TestUtil
{
    /// <summary>
    /// Simple plugin that can be used in tests.
    /// </summary>
    public class TestPlugin : PluginBase
    {
        private readonly IEnumerable<StateInfo> stateInfos;

        public TestPlugin(IEnumerable<StateInfo> stateInfos = null)
        {
            this.stateInfos = stateInfos;
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<IProject>();
        }

        public override IEnumerable<StateInfo> GetStateInfos()
        {
            return stateInfos ?? base.GetStateInfos();
        }
    }
}