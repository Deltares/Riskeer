// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;

namespace Ringtoets.DuneErosion.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneErosionPlugin : PluginBase
    {
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<DuneErosionFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                null,
                null
            );
        }

        #region DuneErosionFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(DuneErosionFailureMechanismContext failureMechanismContext)
        {
            return new object[0];
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(DuneErosionFailureMechanismContext failureMechanismContext)
        {
            return new object[0];
        }

        #endregion
    }
}