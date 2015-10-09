using System.Linq;
using DelftTools.Shell.Core.Extensions;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace DeltaShell.Tests.Core.Extensions
{
    [TestFixture]
    public class CompositeActivityExtensionsTest
    {
        [Test]
        public void GetAllActivitiesRecursive()
        {
            var mocks = new MockRepository();
            var activity1 = mocks.Stub<IActivity>();
            var activity2 = mocks.Stub<IActivity>();
            var activity3 = mocks.Stub<IActivity>();
            var activity4 = mocks.Stub<IActivity>();
            var activity5 = mocks.Stub<IActivity>();
            var activity6 = mocks.Stub<IActivity>();

            mocks.ReplayAll();

            var compositeActivity = new ParallelActivity
            {
                Activities = new EventedList<IActivity>
                {
                    new SequentialActivity
                    {
                        Activities = new EventedList<IActivity>
                        {
                            activity1, activity2
                        }
                    },
                    new SequentialActivity
                    {
                        Activities = new EventedList<IActivity>
                        {
                            activity3, activity4, activity5
                        }
                    },
                    activity6
                }
            };

            Assert.AreEqual(9, compositeActivity.GetAllActivitiesRecursive<IActivity>().Count());
        }
    }
}