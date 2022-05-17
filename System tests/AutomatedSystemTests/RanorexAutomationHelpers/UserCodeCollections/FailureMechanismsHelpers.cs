/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 16/05/2022
 * Time: 16:25
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;



namespace Ranorex_Automation_Helpers.UserCodeCollections
{
    /// <summary>
    /// Description of FailureMechanismsHelpers.
    /// </summary>
        public class Section
    {
        public Section()
        {
            this.CalculationFailureProbPerSection ="-";
            this.AssemblyGroup = "";
            this.Nvak = Double.NaN;
        }
        
        /// <summary>
        /// Label for the combined assessment of the section (Iv, IIv, IIIv, ...)
        /// </summary>
        public string AssemblyGroup {get; set;}
        
        /// <summary>
        /// Probability associated to the combined assessment of the profile (it exists for some FMs).
        /// </summary>
        public string CalculationFailureProbPerProfile {get; set;}

        /// <summary>
        /// Probability associated to the combined assessment of the section (it exists for some FMs).
        /// </summary>
        public string CalculationFailureProbPerSection {get; set;}
        
        /// <summary>
        /// The name of the section (vak).
        /// </summary>
        public string Name {get; set;}
        
        /// <summary>
        /// The distance along the reference line at which the setion starts.
        /// </summary>
        public double StartDistance {get; set;}
        
        /// <summary>
        /// The distance along the reference line at which the setion ends.
        /// </summary>
        public double EndDistance {get; set;}
        
        /// <summary>
        /// The length of the section along the reference line.
        /// </summary>
        public double Length {get; set;}
        
        /// <summary>
        /// The value of the parameter NVak, if it exists.
        /// </summary>
        public double Nvak {get; set;}
        
    }
    
    public class FailureMechanismResultInformation
    {
        public FailureMechanismResultInformation()
        {
            this.SectionList = new List<Section>();
            this.FailureProbability = "-";
        }
        
        /// <summary>
        /// Label of the FM
        /// </summary>
        public string Label{get; set;}
        
        /// <summary>
        /// The label for the assessment of this FM regarding the entire traject (It, IIt, ..., VIIt)
        /// </summary>
        public string AssemblyGroup {get; set;}
        
        /// <summary>
        /// The probability assigned to the entire traject assessment, if this value exists.
        /// </summary>
        public string FailureProbability {get; set;}
        
        public List<Section> SectionList {get; set;}
    }
    
    public class TrajectResultInformation
    {
        public TrajectResultInformation()
        {
            ListFMsResultInformation = new List<FailureMechanismResultInformation>();
            UpperLimitsSecurityBoundaries = new List<string>();
        }
        
        public List<FailureMechanismResultInformation> ListFMsResultInformation {get; set;}
        
        public List<string> UpperLimitsSecurityBoundaries {get; set;}
        
        public static TrajectResultInformation BuildAssessmenTrajectInformation(string trajectAssessmentInformationString)
        {
            TrajectResultInformation trajectAssessmentInformation;
            if (trajectAssessmentInformationString=="") {
                trajectAssessmentInformation = new TrajectResultInformation();
            } else {
                var error = false;
                trajectAssessmentInformation = JsonConvert.DeserializeObject<TrajectResultInformation>(trajectAssessmentInformationString, new JsonSerializerSettings
                {
                    Error = (s, e) =>
                    {
                        error = true;
                        e.ErrorContext.Handled = true;
                    }
                }
            );
                if (error==true) {
                    
                    Report.Log(ReportLevel.Error, "error unserializing json string for trajectAssessmentInformationString: " + trajectAssessmentInformationString);
                }
                
            }
            return trajectAssessmentInformation;
        }
        
    }
    
    
}
