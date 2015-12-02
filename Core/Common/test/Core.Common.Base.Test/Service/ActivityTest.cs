﻿using Core.Common.Base.Service;
using NUnit.Framework;

namespace Core.Common.Base.Test.Service
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
            Assert.AreEqual(ActivityState.None, activity.State);
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
            protected override void OnRun()
            {
            }

            protected override void OnCancel()
            {
            }

            protected override void OnFinish()
            {
            }
        }
    }
}