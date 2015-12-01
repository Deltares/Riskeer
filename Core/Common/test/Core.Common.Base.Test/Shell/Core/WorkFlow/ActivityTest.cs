using System;
using Core.Common.Base.Workflow;
using NUnit.Framework;

namespace Core.Common.Base.Test.Shell.Core.WorkFlow
{
    [TestFixture]
    public class ActivityTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var activity = new SimpleActivity();

            // assert
            Assert.IsNull(activity.Name);
            Assert.AreEqual(ActivityStatus.None, activity.Status);
            Assert.IsNull(activity.ProgressText);
        }

        [Test]
        public void Name_SetAndGetValue_ReturnSetValue()
        {
            // setup & call
            const string someName = "Some name";
            var activity = new SimpleActivity
            {
                Name = someName
            };

            // assert
            Assert.AreEqual(someName, activity.Name);
        }

        private class SimpleActivity : Activity
        {
            /// <summary>
            /// Sets the implementation of <see cref="OnInitialize"/>.
            /// </summary>
            public Action OnInitializeInjection { private get; set; }

            /// <summary>
            /// Sets the implementation of <see cref="OnCancel"/>.
            /// </summary>
            public Action OnCancelInjection { private get; set; }

            protected override void OnInitialize()
            {
                OnInitializeInjection();
            }

            protected override void OnExecute()
            {
                throw new NotImplementedException();
            }

            protected override void OnCancel()
            {
                OnCancelInjection();
            }

            protected override void OnCleanUp()
            {
                throw new NotImplementedException();
            }

            protected override void OnFinish()
            {
                throw new NotImplementedException();
            }
        }
    }
}