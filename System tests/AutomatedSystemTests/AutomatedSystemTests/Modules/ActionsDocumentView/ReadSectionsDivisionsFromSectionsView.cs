/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 04/12/2020
 * Time: 17:48
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Newtonsoft.Json;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ReadSectionsDivisionsFromSectionsView.
    /// </summary>
    [TestModule("F63ED64A-DB92-435D-A38E-21A7DC8E6F54", ModuleType.UserCode, 1)]
    public class ReadSectionsDivisionsFromSectionsView : ITestModule
    {
        
        
        string _trajectAssessmentInformation = "";
        [TestVariable("b4271fc1-5724-4698-85e2-3434ee79f78f")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformation; }
            set { _trajectAssessmentInformation = value; }
        }
        
        
        string _labelFM = "";
        [TestVariable("f6ff4e12-a192-49bd-9f9e-85c8899a2f7b")]
        public string labelFM
        {
            get { return _labelFM; }
            set { _labelFM = value; }
        }
        
        
        string _groupFM = "";
        [TestVariable("7d468079-12d4-43ac-bd2c-bf6ec1396763")]
        public string groupFM
        {
            get { return _groupFM; }
            set { _groupFM = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadSectionsDivisionsFromSectionsView()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0;
            
            var trajectAssessmentInformation = BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var tableSectionsDivisions = repo.RiskeerMainWindow.DocumentViewContainerUncached.TableVakindeling;

            var rowsData = tableSectionsDivisions.Rows;
            
            // Indeces of the properties of the section.
            Row rowHeader = rowsData[0];
            int indexName = rowHeader.Cells.ToList().FindIndex(c => c.Element.GetAttributeValueText("AccessibleValue")=="Vaknaam");
            int indexStartDistance = rowHeader.Cells.ToList().FindIndex(c => c.Element.GetAttributeValueText("AccessibleValue")=="Metrering van* [m]");
            int indexEndDistance = rowHeader.Cells.ToList().FindIndex(c => c.Element.GetAttributeValueText("AccessibleValue")=="Metrering tot* [m]");
            int indexLength = rowHeader.Cells.ToList().FindIndex(c => c.Element.GetAttributeValueText("AccessibleValue")=="Lengte* [m]");
            int indexNVak = rowHeader.Cells.ToList().FindIndex(c => c.Element.GetAttributeValueText("AccessibleValue")=="Nvak* [-]");
            
            rowsData.RemoveAt(0);
            FailureMechanismAssessmentInformation fmAssessmentInformation = 
                new FailureMechanismAssessmentInformation();
            fmAssessmentInformation.Label = labelFM;
            fmAssessmentInformation.Group = Int32.Parse(groupFM);
            foreach (var row in rowsData) {
                var cellsDataInRow = row.Cells.ToList();
                Section section = new Section();
                cellsDataInRow[indexName].Select();
                section.Name = cellsDataInRow[indexName].Element.GetAttributeValueText("AccessibleValue");
                cellsDataInRow[indexStartDistance].Select();
                section.StartDistance = Double.Parse(cellsDataInRow[indexStartDistance].Element.GetAttributeValueText("AccessibleValue"));
                cellsDataInRow[indexEndDistance].Select();
                section.EndDistance = Double.Parse(cellsDataInRow[indexEndDistance].Element.GetAttributeValueText("AccessibleValue"));
                cellsDataInRow[indexLength].Select();
                section.Length = Double.Parse(cellsDataInRow[indexLength].Element.GetAttributeValueText("AccessibleValue"));
                if (indexNVak!=-1) {
                    cellsDataInRow[indexNVak].Select();
                    section.Nvak = Double.Parse(cellsDataInRow[indexNVak].Element.GetAttributeValueText("AccessibleValue"));
                }
                fmAssessmentInformation.SectionList.Add(section);
            }
            // Information for this FM not existing yet
            trajectAssessmentInformation.ListFMsAssessmentInformation.Add(fmAssessmentInformation);
            trajectAssessmentInformationString = JsonConvert.SerializeObject(trajectAssessmentInformation, Formatting.Indented);
            Report.Log(ReportLevel.Info, "Done!");
        }
        
        private TrajectAssessmentInformation BuildAssessmenTrajectInformation(string trajectAssessmentInformationString)
        {
            TrajectAssessmentInformation trajectAssessmentInformation;
            if (trajectAssessmentInformationString=="") {
                trajectAssessmentInformation = new TrajectAssessmentInformation();
            } else {
                var error = false;
                trajectAssessmentInformation = JsonConvert.DeserializeObject<TrajectAssessmentInformation>(trajectAssessmentInformationString, new JsonSerializerSettings
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
    
    
    /// <summary>
    /// A section (vak) is a subdivision of a traject. The set of sections is independent for each FM.
    /// </summary>
    public class Section
    {
        public Section()
        {
            this.CombinedAssessmentProbability ="-";
            this.CombinedAssessmentLabel = "";
            this.Nvak = Double.NaN;
        }
        
        /// <summary>
        /// Label for the combined assessment of the section (Iv, IIv, IIIv, ...)
        /// </summary>
        public string CombinedAssessmentLabel {get; set;}
        
        /// <summary>
        /// Probability associated to the combined assessment of the section (it exists for some FMs).
        /// </summary>
        public string CombinedAssessmentProbability {get; set;}
        
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
    
    public class FailureMechanismAssessmentInformation
    {
        public FailureMechanismAssessmentInformation()
        {
            this.SectionList = new List<Section>();
            this.AssessmentProbability = "-";
        }
        
        /// <summary>
        /// Label of the FM
        /// </summary>
        public string Label{get; set;}
        
        /// <summary>
        /// The group to which the FM belongs.
        /// </summary>
        public int Group {get; set;}
        
        /// <summary>
        /// The label for the assessment of this FM regarding the entire traject (It, IIt, ..., VIIt)
        /// </summary>
        public string AssessmentLabel {get; set;}
        
        /// <summary>
        /// The probability assigned to the entire traject assessment, if this value exists.
        /// </summary>
        public string AssessmentProbability {get; set;}
        
        public List<Section> SectionList {get; set;}
    }
    
    public class TrajectAssessmentInformation
    {
        public TrajectAssessmentInformation()
        {
            ListFMsAssessmentInformation = new List<FailureMechanismAssessmentInformation>();
        }
        public List<FailureMechanismAssessmentInformation> ListFMsAssessmentInformation {get; set;}
    }
}
