﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
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
        ///   Looks up a localized string similar to h(norm).
        /// </summary>
        public static string DesignWaterLevel_Description {
            get {
                return ResourceManager.GetString("DesignWaterLevel_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dijkprofiellocaties.
        /// </summary>
        public static string DikeProfilesImporter_DisplayName {
            get {
                return ResourceManager.GetString("DikeProfilesImporter_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Na het aanpassen van de norm zullen alle rekenresultaten van hydraulische randvoorwaarden en faalmechanismen verwijderd worden. Wilt u doorgaan?.
        /// </summary>
        public static string FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_dependent_data {
            get {
                return ResourceManager.GetString("FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_depen" +
                        "dent_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De resultaten van {0} berekeningen zijn verwijderd..
        /// </summary>
        public static string FailureMechanismContributionView_NormValueChanged_Results_of_NumberOfCalculations_0_calculations_cleared {
            get {
                return ResourceManager.GetString("FailureMechanismContributionView_NormValueChanged_Results_of_NumberOfCalculations" +
                        "_0_calculations_cleared", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd..
        /// </summary>
        public static string FailureMechanismContributionView_NormValueChanged_Waveheight_and_design_water_level_results_cleared {
            get {
                return ResourceManager.GetString("FailureMechanismContributionView_NormValueChanged_Waveheight_and_design_water_lev" +
                        "el_results_cleared", resourceCulture);
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
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap Foreshore {
            get {
                object obj = ResourceManager.GetObject("Foreshore", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voorlandprofiellocaties.
        /// </summary>
        public static string ForeshoreProfilesImporter_DisplayName {
            get {
                return ResourceManager.GetString("ForeshoreProfilesImporter_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Na het importeren van een aangepaste ligging van de referentielijn zullen alle geïmporteerde en berekende gegevens van faalmechanismen worden gewist.
        ///
        ///Wilt u doorgaan?.
        /// </summary>
        public static string ReferenceLineReplacementHandler_Confirm_clear_referenceLine_dependent_data {
            get {
                return ResourceManager.GetString("ReferenceLineReplacementHandler_Confirm_clear_referenceLine_dependent_data", resourceCulture);
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
        
        /// <summary>
        ///   Looks up a localized string similar to Hs(norm).
        /// </summary>
        public static string WaveHeight_Description {
            get {
                return ResourceManager.GetString("WaveHeight_Description", resourceCulture);
            }
        }
    }
}
