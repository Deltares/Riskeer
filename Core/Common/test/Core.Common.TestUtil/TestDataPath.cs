// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Used to make paths strongly typed, see <see cref="TestHelper.GetTestDataPath(TestDataPath)"/>
    /// </summary>
    public class TestDataPath
    {
        public string Path { get; private set; }

        public static implicit operator TestDataPath(string path)
        {
            return new TestDataPath
            {
                Path = path
            };
        }

        public static class Core
        {
            public static class Common
            {
                public static readonly TestDataPath Util = "Core.Common.Util.Test";
                public static readonly TestDataPath IO = "Core.Common.IO.Test";
            }

            public static class Components
            {
                public static class Gis
                {
                    public static readonly TestDataPath IO = "Core.Components.Gis.IO.Test";
                }
            }
        }

        public static class Riskeer
        {
            public static class AssemblyTool
            {
                public static readonly TestDataPath IO = "Riskeer.AssemblyTool.IO.Test";
            }

            public static class ClosingStructures
            {
                public static readonly TestDataPath IO = "Riskeer.ClosingStructures.IO.Test";
            }

            public static class Common
            {
                public static readonly TestDataPath IO = "Riskeer.Common.IO.Test";
                public static readonly TestDataPath Service = "Riskeer.Common.Service.Test";
            }

            public static class GrassCoverErosionInwards
            {
                public static readonly TestDataPath IO = "Riskeer.GrassCoverErosionInwards.IO.Test";
                public static readonly TestDataPath Integration = "Riskeer.GrassCoverErosionInwards.Integration.Test";
            }

            public static class GrassCoverErosionOutwards
            {
                public static readonly TestDataPath IO = "Riskeer.GrassCoverErosionOutwards.IO.Test";
            }

            public static class HeightStructures
            {
                public static readonly TestDataPath IO = "Riskeer.HeightStructures.IO.Test";
                public static readonly TestDataPath Integration = "Riskeer.HeightStructures.Integration.Test";
            }

            public static class HydraRing
            {
                public static readonly TestDataPath IO = "Riskeer.HydraRing.IO.Test";
                public static readonly TestDataPath Calculation = "Riskeer.HydraRing.Calculation.Test";
            }

            public static class Integration
            {
                public static readonly TestDataPath Forms = "Riskeer.Integration.Forms.Test";
                public static readonly TestDataPath Service = "Riskeer.Integration.Service.Test";
                public static readonly TestDataPath IO = "Riskeer.Integration.IO.Test";
                public static readonly TestDataPath Plugin = "Riskeer.Integration.Plugin.Test";
                public static readonly TestDataPath TestUtil = "Riskeer.Integration.TestUtil.Test";
            }

            public static class MacroStabilityInwards
            {
                public static readonly TestDataPath IO = "Riskeer.MacroStabilityInwards.IO.Test";
            }

            public static class Migration
            {
                public static readonly TestDataPath Core = "Riskeer.Migration.Core.Test";
            }

            public static class Piping
            {
                public static readonly TestDataPath IO = "Riskeer.Piping.IO.Test";
            }

            public static class Revetment
            {
                public static readonly TestDataPath IO = "Riskeer.Revetment.IO.Test";
            }

            public static class StabilityPointStructures
            {
                public static readonly TestDataPath IO = "Riskeer.StabilityPointStructures.IO.Test";
            }

            public static class StabilityStoneCover
            {
                public static readonly TestDataPath IO = "Riskeer.StabilityStoneCover.IO.Test";
            }

            public static class Storage
            {
                public static readonly TestDataPath Core = "Riskeer.Storage.Core.Test";
            }
        }
    }
}