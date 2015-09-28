using System;
using System.ComponentModel;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Core.Workflow.DataItems;
using log4net;

namespace DelftTools.Tests.Core.Mocks
{
    // TODO: Remove this "stub" and use real stubs in the tests
    public class TimeDependentModelBaseStub : TimeDependentModelBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TimeDependentModelBaseStub)); 
        
        public event EventHandler Executing;

        protected override void OnInitialize()
        {

        }

        protected override bool OnExecute()
        {
            if (null != Executing)
            {
                Executing(this, null);
            }

            return true;
        }

        protected override void OnDataItemRemoved(IDataItem item)
        {

        }

        protected override void OnDataItemAdded(IDataItem item)
        {

        }

        protected override void OnInputPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnInputDataChangedCallCount++;

            Log.DebugFormat("InputPropertyChanged: {0}.{1}", sender, e.PropertyName);

            base.OnInputPropertyChanged(sender, e);
        }

        /// <summary>
        /// Used by test
        /// </summary>
        public int OnInputDataChangedCallCount { get; set; }
    }
}