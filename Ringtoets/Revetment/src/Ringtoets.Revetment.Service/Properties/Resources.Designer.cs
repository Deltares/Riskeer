﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.Revetment.Service.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Revetment.Service.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geen geldige damhoogte ingevoerd..
        /// </summary>
        internal static string WaveConditionsCalculationService_ValidateInput_Invalid_BreakWaterHeight_value {
            get {
                return ResourceManager.GetString("WaveConditionsCalculationService_ValidateInput_Invalid_BreakWaterHeight_value", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen..
        /// </summary>
        internal static string WaveConditionsCalculationService_ValidateInput_No_derived_WaterLevels {
            get {
                return ResourceManager.GetString("WaveConditionsCalculationService_ValidateInput_No_derived_WaterLevels", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan het toetspeil niet afleiden op basis van de invoer..
        /// </summary>
        internal static string WaveConditionsCalculationService_ValidateInput_No_DesignWaterLevel_calculated {
            get {
                return ResourceManager.GetString("WaveConditionsCalculationService_ValidateInput_No_DesignWaterLevel_calculated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen hydraulische randvoorwaardenlocatie geselecteerd..
        /// </summary>
        internal static string WaveConditionsCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selected {
            get {
                return ResourceManager.GetString("WaveConditionsCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selec" +
                        "ted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Berekening &apos;{0}&apos; voor waterstand &apos;{1}&apos; is niet gelukt..
        /// </summary>
        internal static string WaveConditionsCalculationService_VerifyWaveConditionsCalculationOutput_Error_in_wave_conditions_calculation_0_for_waterlevel_1 {
            get {
                return ResourceManager.GetString("WaveConditionsCalculationService_VerifyWaveConditionsCalculationOutput_Error_in_w" +
                        "ave_conditions_calculation_0_for_waterlevel_1", resourceCulture);
            }
        }
    }
}
