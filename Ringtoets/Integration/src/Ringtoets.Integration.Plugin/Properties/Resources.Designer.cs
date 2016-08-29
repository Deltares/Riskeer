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

namespace Ringtoets.Integration.Plugin.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Integration.Plugin.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Opmerkingen.
        /// </summary>
        public static string Comment_DisplayName {
            get {
                return ResourceManager.GetString("Comment_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dijkprofiel locaties.
        /// </summary>
        public static string DikeProfilesImporter_DisplayName {
            get {
                return ResourceManager.GetString("DikeProfilesImporter_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen geldige voorland- en dijkprofieldata vinden voor dijkprofiel locatie met ID: {0}.
        /// </summary>
        public static string DikeProfilesImporter_GetMatchingDikeProfileData_no_dikeprofiledata_for_location_0_ {
            get {
                return ResourceManager.GetString("DikeProfilesImporter_GetMatchingDikeProfileData_no_dikeprofiledata_for_location_0" +
                        "_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dijkprofielen importeren is afgebroken. Geen data ingelezen..
        /// </summary>
        public static string DikeProfilesImporter_HandleUserCancellingImport_dikeprofile_import_aborted {
            get {
                return ResourceManager.GetString("DikeProfilesImporter_HandleUserCancellingImport_dikeprofile_import_aborted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen referentielijn beschikbaar. Geen data ingelezen..
        /// </summary>
        public static string DikeProfilesImporter_Import_no_referenceline_import_aborted {
            get {
                return ResourceManager.GetString("DikeProfilesImporter_Import_no_referenceline_import_aborted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} 
        ///Er is geen vakindeling geïmporteerd..
        /// </summary>
        public static string FailureMechanismSectionsImporter_CriticalErrorMessage_0_No_sections_imported {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_CriticalErrorMessage_0_No_sections_imported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vakindeling importeren afgebroken. Geen data ingelezen..
        /// </summary>
        public static string FailureMechanismSectionsImporter_Import_cancelled_no_data_read {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_Import_cancelled_no_data_read", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vakindeling komt niet overeen met de huidige referentielijn..
        /// </summary>
        public static string FailureMechanismSectionsImporter_Import_Imported_sections_do_not_correspond_to_current_referenceline {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_Import_Imported_sections_do_not_correspond_to_cu" +
                        "rrent_referenceline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen referentielijn beschikbaar om een vakindeling voor te definiëren..
        /// </summary>
        public static string FailureMechanismSectionsImporter_Import_Required_referenceline_missing {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_Import_Required_referenceline_missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geïmporteerde data toevoegen aan het toetsspoor..
        /// </summary>
        public static string FailureMechanismSectionsImporter_ProgressText_Adding_imported_data_to_failureMechanism {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_ProgressText_Adding_imported_data_to_failureMech" +
                        "anism", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen vakindeling..
        /// </summary>
        public static string FailureMechanismSectionsImporter_ProgressText_Reading_file {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_ProgressText_Reading_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Valideren ingelezen vakindeling..
        /// </summary>
        public static string FailureMechanismSectionsImporter_ProgressText_Validating_imported_sections {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_ProgressText_Validating_imported_sections", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand heeft geen vakindeling..
        /// </summary>
        public static string FailureMechanismSectionsImporter_ReadFile_File_is_empty {
            get {
                return ResourceManager.GetString("FailureMechanismSectionsImporter_ReadFile_File_is_empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap Foreshore {
            get {
                object obj = ResourceManager.GetObject("Foreshore", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voorlandprofiel locaties.
        /// </summary>
        public static string ForeshoreProfilesImporter_DisplayName {
            get {
                return ResourceManager.GetString("ForeshoreProfilesImporter_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen geldige voorlandprofieldata vinden voor voorlandprofiel locatie met ID: {0}.
        /// </summary>
        public static string ForeshoreProfilesImporter_GetMatchingForeshoreProfileData_no_foreshoreprofiledata_for_location_0_ {
            get {
                return ResourceManager.GetString("ForeshoreProfilesImporter_GetMatchingForeshoreProfileData_no_foreshoreprofiledata" +
                        "_for_location_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voorlandprofielen importeren is afgebroken. Geen data ingelezen..
        /// </summary>
        public static string ForeshoreProfilesImporter_HandleUserCancellingImport_foreshoreprofile_import_aborted {
            get {
                return ResourceManager.GetString("ForeshoreProfilesImporter_HandleUserCancellingImport_foreshoreprofile_import_abor" +
                        "ted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Het bestand wordt overgeslagen..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_ErrorMessage_0_file_skipped {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ErrorMessage_0_file_skipped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De hydraulische randvoorwaardenlocaties zijn ingelezen..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_Import_All_hydraulic_locations_read {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_Import_All_hydraulic_locations_read", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Er zijn geen hydraulische randvoorwaarden locaties geëxporteerd..
        /// </summary>
        public static string HydraulicBoundaryLocationsExporter_Error_Exception_0_no_HydraulicBoundaryLocations_exported {
            get {
                return ResourceManager.GetString("HydraulicBoundaryLocationsExporter_Error_Exception_0_no_HydraulicBoundaryLocation" +
                        "s_exported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LocationID
        ///300130
        ///300131
        ///300189
        ///300353
        ///300354
        ///300355
        ///300357
        ///300358
        ///300359
        ///300360
        ///300361
        ///300362
        ///300363
        ///300364
        ///300365
        ///300366
        ///300367
        ///300368
        ///300369
        ///300370
        ///300371
        ///300372
        ///300373
        ///300374
        ///300375
        ///300376
        ///300608
        ///300609
        ///300610
        ///300611
        ///300612
        ///300633
        ///300634
        ///300635
        ///300636
        ///300637
        ///300657
        ///300658
        ///300659
        ///300660
        ///300661
        ///300662
        ///300663
        ///300664
        ///300665
        ///300703
        ///300704
        ///300745
        ///300748
        ///300761
        ///300762
        ///300765
        ///300766
        ///300767
        ///300824
        ///300825
        ///300826
        ///300828
        ///300829
        ///300830
        ///300864
        ///300865
        ///301595
        ///301596
        ///301597
        ///301598
        ///301599
        ///301600
        ///301601
        ///301602
        ///301603
        ///3016 [rest of string was truncated]&quot;;.
        /// </summary>
        public static string HydraulicBoundaryLocationsFilterList {
            get {
                return ResourceManager.GetString("HydraulicBoundaryLocationsFilterList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een profiel locatie met ID &apos;{0}&apos; ligt niet op de referentielijn. Locatie wordt overgeslagen..
        /// </summary>
        public static string ProfilesImporter_AddNextDikeProfileLocation_0_skipping_location_outside_referenceline {
            get {
                return ResourceManager.GetString("ProfilesImporter_AddNextDikeProfileLocation_0_skipping_location_outside_reference" +
                        "line", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profiel locatie met ID &apos;{0}&apos; is opnieuw ingelezen..
        /// </summary>
        public static string ProfilesImporter_AddNextDikeProfileLocation_Location_with_id_0_already_read {
            get {
                return ResourceManager.GetString("ProfilesImporter_AddNextDikeProfileLocation_Location_with_id_0_already_read", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fout bij het lezen van profiel op regel {0}. {1} Dit profiel wordt overgeslagen..
        /// </summary>
        public static string ProfilesImporter_GetDikeProfileLocationReadResult_Error_reading_Profile_LineNumber_0_Error_1_The_Profile_is_skipped {
            get {
                return ResourceManager.GetString("ProfilesImporter_GetDikeProfileLocationReadResult_Error_reading_Profile_LineNumbe" +
                        "r_0_Error_1_The_Profile_is_skipped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van profiel locatie..
        /// </summary>
        public static string ProfilesImporter_GetDikeProfileLocationReadResult_reading_profilelocation {
            get {
                return ResourceManager.GetString("ProfilesImporter_GetDikeProfileLocationReadResult_reading_profilelocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Meerdere profieldata definities gevonden voor profiel &apos;{0}&apos;. Bestand &apos;{1}&apos; wordt overgeslagen..
        /// </summary>
        public static string ProfilesImporter_LogDuplicateDikeProfileData_Multiple_DikeProfileData_found_for_DikeProfile_0_File_1_skipped {
            get {
                return ResourceManager.GetString("ProfilesImporter_LogDuplicateDikeProfileData_Multiple_DikeProfileData_found_for_D" +
                        "ikeProfile_0_File_1_skipped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van profieldata uit een prfl bestand..
        /// </summary>
        public static string ProfilesImporter_ReadDikeProfileData_reading_profile_data {
            get {
                return ResourceManager.GetString("ProfilesImporter_ReadDikeProfileData_reading_profile_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van profieldata..
        /// </summary>
        public static string ProfilesImporter_ReadDikeProfileData_reading_profiledata {
            get {
                return ResourceManager.GetString("ProfilesImporter_ReadDikeProfileData_reading_profiledata", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Profieldata specificeert een damwand waarde ongelijk aan 0. Bestand wordt overgeslagen: {0}.
        /// </summary>
        public static string ProfilesImporter_ReadDikeProfileData_sheet_piling_not_zero_skipping_0_ {
            get {
                return ResourceManager.GetString("ProfilesImporter_ReadDikeProfileData_sheet_piling_not_zero_skipping_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van profiel locaties uit een shapebestand..
        /// </summary>
        public static string ProfilesImporter_ReadProfileLocations_reading_profilelocations {
            get {
                return ResourceManager.GetString("ProfilesImporter_ReadProfileLocations_reading_profilelocations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nieuw.
        /// </summary>
        public static string RingtoetsRibbon_GroupBox_New {
            get {
                return ResourceManager.GetString("RingtoetsRibbon_GroupBox_New", resourceCulture);
            }
        }
    }
}
