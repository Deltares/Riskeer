using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Merge
{
    public partial class AssessmentSectionProviderDialog : DialogBase, IMergeDataProvider
    {
        public AssessmentSectionProviderDialog(IEnumerable<AssessmentSection> assessmentSections,
                                               IWin32Window dialogParent)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 500, 350)
        {
            if (assessmentSections == null)
            {
                throw new ArgumentNullException(nameof(assessmentSections));
            }

            InitializeComponent();
        }

        protected override Button GetCancelButton()
        {
            throw new NotImplementedException();
        }

        public AssessmentSection SelectedAssessmentSection { get; }
        public IEnumerable<IFailureMechanism> SelectedFailureMechanisms { get; }
        public bool SelectData(IEnumerable<AssessmentSection> assessmentSections)
        {
            throw new NotImplementedException();
        }
    }
}