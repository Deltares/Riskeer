/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 06/12/2020
 * Time: 18:42
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
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
    /// Description of ValidateAssemblyTrajectView.
    /// </summary>
    [TestModule("38D69B05-5CA8-43A0-AFCF-16D6DF183738", ModuleType.UserCode, 1)]
    public class ValidateAssemblyTrajectView : ITestModule
    {
        
        
        string _trajectAssessmentInformationString = "";
        [TestVariable("4188b6a9-c5aa-4416-9731-a4fba6fa5dbb")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        
        string _categoryBoundariesTraject = "";
        [TestVariable("fb2221fa-c3ce-41c4-a074-6e2cbece5657")]
        public string categoryBoundariesTraject
        {
            get { return _categoryBoundariesTraject; }
            set { _categoryBoundariesTraject = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateAssemblyTrajectView()
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
            Delay.SpeedFactor = 0.0;
            
            var trajectAssessmentInformation = BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            var table = repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.Self;
            ValidateTableAssemblyTrajectView(table, trajectAssessmentInformation);
            
            var summaryTraject = repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Summary;
            
            string expectedAssessmentProb1and2 = CalculateAssessmentProbabilityGroups1and2(trajectAssessmentInformation);
            string actualAssessmentProb1and2 = summaryTraject.AssessmentProbabilityGroups1And2.TextValue;
            ValidateAreEqualWithMessage(actualAssessmentProb1and2, expectedAssessmentProb1and2, "Validation Assembly probability groups 1 and 2.");
            
            string expectedAssessmentLabel1and2 = CalculateAssessmentLabelGroups1and2(expectedAssessmentProb1and2, categoryBoundariesTraject);
            string actualAssessmentLabel1and2 = summaryTraject.AssessmentLabelGroups1And2.TextValue;
            ValidateAreEqualWithMessage(actualAssessmentLabel1and2, expectedAssessmentLabel1and2, "Validation Assembly Label groups 1 and 2.");
            
            string expectedAssessmentLabel3and4 = CalculateAssessmentLabelGroups3and4(trajectAssessmentInformation);
            string actualAssessmentLabel3and4 = summaryTraject.AssessmentGroups3And4.TextValue;
            ValidateAreEqualWithMessage(actualAssessmentLabel1and2, expectedAssessmentLabel1and2, "Validation Assembly Label groups 3 and 4.");
            
            string expectedSecurityAssessmentLabel = CalculateSecurityAssessmentLabel(expectedAssessmentLabel1and2, expectedAssessmentLabel3and4);
            string actualSecurityAssessmentLabel = summaryTraject.SecurityAssessment.TextValue;
            ValidateAreEqualWithMessage(actualSecurityAssessmentLabel, expectedSecurityAssessmentLabel, "Validation Security Assembly Label.");
            

        }
        private void ValidateAreEqualWithMessage(string actualValue, string expectedValue, string message)
        {
            Report.Info(message);
            Validate.AreEqual(actualValue, expectedValue);
        }

        private string CalculateAssessmentLabelGroups1and2(string expectedProb, string categoryBoundariesTraject)
        {
            if (expectedProb=="1/Oneindig") {
                return "-";
            } else {
                System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
                var boundaries = categoryBoundariesTraject.Split(';').ToList();
                var boundariesValues = boundaries.Select(bd=>1.0/Double.Parse(bd.Substring(2,bd.Length-2), currentCulture)).ToList();
                boundariesValues.Add(1.0);
                double expectedProbValue = 1.0/Double.Parse(expectedProb.Substring(2, expectedProb.Length-2), currentCulture);
                string label = "";
                var indexFirstBoundaryAbove = boundariesValues.FindIndex(bd=>bd>expectedProbValue);
                switch (indexFirstBoundaryAbove) {
                    case 0:
                        label = "It";
                        break;
                    case 1:
                        label = "IIt";
                        break;
                    case 2:
                        label = "IIIt";
                        break;
                    case 3:
                        label = "IVt";
                        break;
                    case 4:
                        label = "Vt";
                        break;
                    case 5:
                        label = "VIt";
                        break;
                }
                return label;
            }
        }

        private string CalculateAssessmentLabelGroups3and4(TrajectAssessmentInformation trjAssInfo)
        {
            Dictionary<string,int> dicAssemblyLabels = new Dictionary<string, int> {
                {"-",    0},
                {"It",   1},
                {"IIt",  2},
                {"IIIt", 3},
                {"IVt",  4},
                {"Vt",   5},
                {"VIt",  6},
                {"VIIt", 7}
            };
            int maxLabel = 0;
            string labelGroup3and4 ="";
            var trjAssInfoFMsGroup3and4 = trjAssInfo.ListFMsAssessmentInformation.Where(tai=>tai.Group==3 || tai.Group==4);
            foreach (var fmTrjAssInfo in trjAssInfoFMsGroup3and4) {
                if (dicAssemblyLabels[fmTrjAssInfo.AssessmentLabel]>maxLabel) {
                    maxLabel = dicAssemblyLabels[fmTrjAssInfo.AssessmentLabel];
                    labelGroup3and4 = fmTrjAssInfo.AssessmentLabel;
                }
            }
            return labelGroup3and4;
        }

        private string CalculateSecurityAssessmentLabel(string expectedLabel12, string expectedLabel34)
        {
            Dictionary<string,int> dicAssemblyLabels = new Dictionary<string, int> {
                {"-",    0},
                {"It",   1},
                {"IIt",  2},
                {"IIIt", 3},
                {"IVt",  4},
                {"Vt",   5},
                {"VIt",  6},
                {"VIIt", 7}
            };
            int value12 = dicAssemblyLabels[expectedLabel12];
            int value34 = dicAssemblyLabels[expectedLabel34];
            int labelValueWorst = -1;
            if (value12>value34) {
                labelValueWorst = value12;
            } else{
                labelValueWorst = value34;
            }
            
            switch (labelValueWorst) {
                case 0:
                    return "NGO";
                case 1:
                    return "A+";
                case 2:
                    return "A";
                case 3:
                    return "B";
                case 4:
                    return "C";
                case 5:
                    return "C";
                case 6:
                    return "D";
                case 7:
                    return "NGO";
                default:
                    return "ERROR";
            }
        }

        private void ValidateAssessmentProb1and2(string actualValue, string  expectedValue)
        {
            Report.Info("Validating Assembly probability groups 1 and 2...");
            Validate.AreEqual(actualValue, expectedValue);
        }

        private string CalculateAssessmentProbabilityGroups1and2(TrajectAssessmentInformation trjAssInfo)
        {
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            double productInvProbs = 1;
            int numberFmsProb = 0;
            foreach (var assInfo in trjAssInfo.ListFMsAssessmentInformation) {
                if (assInfo.Group==1 || assInfo.Group==2) {
                    string denominator = assInfo.AssessmentProbability.Substring(2, assInfo.AssessmentProbability.Length-2);
                    double currentProb = 1/Double.Parse(denominator, currentCulture);
                    productInvProbs*= 1- currentProb;
                    numberFmsProb++;
                }
            }
            if (numberFmsProb==0) {
                return "1/Oneindig";
            } else {
                double prob = 1 - productInvProbs;
                int denomInt= Convert.ToInt32(1/prob);
                string denominator = String.Format("{0:n0}", denomInt);
                return "1/" + denominator;
            }
        }

        private void ValidateTableAssemblyTrajectView(Table table, TrajectAssessmentInformation trjAssInfo)
        {
            var headerRow = table.Rows[0];
            int indexLabel = GetIndex(headerRow, "Label");
            int indexGroup = GetIndex(headerRow, "Groep");
            int indexAssessmentLabel = GetIndex(headerRow, "Toetsoordeel");
            int indexAssessmentProb = GetIndex(headerRow, "Benaderde");
            
            for (int i = 1; i < table.Rows.Count; i++) {
                var row = table.Rows[i];
                row.Cells[indexLabel].Select();
                string currentFM = GetAV(row.Cells[indexLabel]);
                var assInfo = trjAssInfo.ListFMsAssessmentInformation.Where(ai=>ai.Label==currentFM).FirstOrDefault();
                if (assInfo!=null) {
                    Report.Info("Validation for FM = " + currentFM);
                    Report.Info("Validating group...");
                    row.Cells[indexGroup].Select();
                    Validate.AreEqual(GetAV(row.Cells[indexGroup]), assInfo.Group.ToString());
                    Report.Info("Validating assessment label...");
                    row.Cells[indexAssessmentLabel].Select();
                    Validate.AreEqual(GetAV(row.Cells[indexAssessmentLabel]), assInfo.AssessmentLabel);
                    //if (assInfo.Group==1 || assInfo==2) {
                        Report.Info("Validating (approx.) probability of failure...");
                        row.Cells[indexAssessmentProb].Select();
                        Validate.AreEqual(GetAV(row.Cells[indexAssessmentProb]), assInfo.AssessmentProbability);
                    //}
                }
                
            }
        }
        
        /// <summary>
        /// Returns accessible value of a cell
        /// </summary>
        /// <param name="cl">Cell for which the accessible value is returned.</param>
        /// <returns>The accessible value fo teh cell.</returns>
        private string GetAV(Cell cl)
        {
            return cl.Element.GetAttributeValueText("AccessibleValue");
        }
        
        private int GetIndex(Row row, string name)
        {
            return row.Cells.ToList().FindIndex(cl=>GetAV(cl).Contains(name));
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
        
        private string GetBackgroundColorOfCell(Table table, int rowIndex, int columnIndex)
        {
            Ranorex.Control tableCtrl = (Ranorex.Control) table.Element;

            // Invoke Remotely  
            string colorOfCell = (string)tableCtrl.InvokeRemotely( delegate(System.Windows.Forms.Control control, object input)  
                    {
                        System.Windows.Forms.DataGridView dataGrid = (System.Windows.Forms.DataGridView) control;  
                        // Now you can access each cell:  
                        Color color = dataGrid.Rows[rowIndex].Cells[columnIndex].Style.BackColor;
                        Console.WriteLine("Color: "+color);  
                        return color.ToString();  
                     }
                     );
                  
            Report.Info("Color of Cell: "+colorOfCell);
            
            return colorOfCell;
        }
    }
}
