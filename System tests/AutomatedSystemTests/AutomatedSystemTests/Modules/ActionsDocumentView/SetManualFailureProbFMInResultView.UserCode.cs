﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Newtonsoft.Json;
using System.Linq;


using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    public partial class SetManualFailureProbFMInResultView
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void SetValueFailureProbabilityFM(RepoItemInfo textInfo, string trajectAssessmentInformationString, string currentFMLabel)
        {
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$ManualFailureProbToSet' on item 'textInfo'.", textInfo);
            textInfo.FindAdapter<Text>().Element.SetAttributeValue("AccessibleValue", ManualFailureProbToSet);
            var trajectResultInformation = BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            trajectResultInformation.ListFMsResultInformation.Where(fmItem=>fmItem.Label==currentFMLabel).FirstOrDefault().FailureProbability = ManualFailureProbToSet;
        }
        private TrajectResultInformation BuildAssessmenTrajectInformation(string trajectAssessmentInformationString)
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
