﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Riskeer.MacroStabilityInwards.Primitives.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.MacroStabilityInwards.Primitives.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Klei dijk op klei (geval 1A).
        /// </summary>
        public static string ClayDikeOnClay_DisplayName {
            get {
                return ResourceManager.GetString("ClayDikeOnClay_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Klei dijk op zand (geval 1B).
        /// </summary>
        public static string ClayDikeOnSand_DisplayName {
            get {
                return ResourceManager.GetString("ClayDikeOnSand_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geen lagen gevonden voor de ondergrondschematisatie..
        /// </summary>
        public static string Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers {
            get {
                return ResourceManager.GetString("Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CPhi.
        /// </summary>
        public static string MacroStabilityInwardsShearStrengthModel_CPhi_DisplayName {
            get {
                return ResourceManager.GetString("MacroStabilityInwardsShearStrengthModel_CPhi_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CPhi of Su berekend.
        /// </summary>
        public static string MacroStabilityInwardsShearStrengthModel_CPhiOrSuCalculated_DisplayName {
            get {
                return ResourceManager.GetString("MacroStabilityInwardsShearStrengthModel_CPhiOrSuCalculated_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Su berekend.
        /// </summary>
        public static string MacroStabilityInwardsShearStrengthModel_SuCalculated_DisplayName {
            get {
                return ResourceManager.GetString("MacroStabilityInwardsShearStrengthModel_SuCalculated_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een of meerdere lagen hebben een top onder de bodem van de ondergrondschematisatie..
        /// </summary>
        public static string MacroStabilityInwardsSoilProfile_Layers_Layer_top_below_profile_bottom {
            get {
                return ResourceManager.GetString("MacroStabilityInwardsSoilProfile_Layers_Layer_top_below_profile_bottom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Zand dijk op klei (geval 2A).
        /// </summary>
        public static string SandDikeOnClay_DisplayName {
            get {
                return ResourceManager.GetString("SandDikeOnClay_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Zand dijk op zand (geval 2B).
        /// </summary>
        public static string SandDikeOnSand_DisplayName {
            get {
                return ResourceManager.GetString("SandDikeOnSand_DisplayName", resourceCulture);
            }
        }
    }
}
