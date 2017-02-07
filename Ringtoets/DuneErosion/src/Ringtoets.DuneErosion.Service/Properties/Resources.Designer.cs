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
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.DuneErosion.Service.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.DuneErosion.Service.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Duinafslag berekening is uitgevoerd op de tijdelijke locatie &apos;{0}&apos;. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden..
        /// </summary>
        internal static string DuneErosionBoundaryCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0 {
            get {
                return ResourceManager.GetString("DuneErosionBoundaryCalculationService_Calculate_Calculation_temporary_directory_c" +
                        "an_be_found_on_location_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De berekening voor duinafslag &apos;{0}&apos; is niet gelukt. Bekijk het foutrapport door op details te klikken.
        ///{1}.
        /// </summary>
        internal static string DuneErosionBoundaryCalculationService_Calculate_Error_in_dune_erosion_0_calculation_click_details_for_last_error_report_1 {
            get {
                return ResourceManager.GetString("DuneErosionBoundaryCalculationService_Calculate_Error_in_dune_erosion_0_calculati" +
                        "on_click_details_for_last_error_report_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De berekening voor duinafslag &apos;{0}&apos; is niet gelukt. Er is geen foutrapport beschikbaar..
        /// </summary>
        internal static string DuneErosionBoundaryCalculationService_Calculate_Error_in_dune_erosion_0_calculation_no_error_report {
            get {
                return ResourceManager.GetString("DuneErosionBoundaryCalculationService_Calculate_Error_in_dune_erosion_0_calculati" +
                        "on_no_error_report", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duinafslag berekening voor locatie &apos;{0}&apos; is niet geconvergeerd..
        /// </summary>
        internal static string DuneErosionBoundaryCalculationService_CreateDuneLocationOutput_Calculation_for_location_0_not_converged {
            get {
                return ResourceManager.GetString("DuneErosionBoundaryCalculationService_CreateDuneLocationOutput_Calculation_for_lo" +
                        "cation_0_not_converged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Locatie &apos;{0}&apos; komt overeen met een duinen locatie, maar het formaat van de naam is niet volgens verwachting..
        /// </summary>
        internal static string DuneErosionDataSynchronizationService_SetDuneLocations_Location_0_is_dune_location_but_name_is_not_according_format {
            get {
                return ResourceManager.GetString("DuneErosionDataSynchronizationService_SetDuneLocations_Location_0_is_dune_locatio" +
                        "n_but_name_is_not_according_format", resourceCulture);
            }
        }
    }
}
