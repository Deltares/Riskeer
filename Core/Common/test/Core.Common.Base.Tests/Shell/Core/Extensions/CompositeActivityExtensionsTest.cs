using System.Linq;
using Core.Common.BaseDelftTools.Extensions;
using Core.Common.BaseDelftTools.Workflow;
using Core.Common.Utils.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Tests.Shell.Core.Extensions
{
    [TestFixture]
    public class CompositeActivityExtensionsTest
    {
        [Test]
        public void GetAllActivitiesRecursive_CompositeActivityWithoutChildren_ReturnOnlyCompositeActivity()
        {
            // setup
            var activitiesList = new EventedList<IActivity>();

            var mocks = new MockRepository();
            var compositeActivityMock = mocks.StrictMock<ICompositeActivity>();
            compositeActivityMock.Expect(ca => ca.Activities).Return(activitiesList);
            mocks.ReplayAll();

            // call
            var allActivities = compositeActivityMock.GetAllActivitiesRecursive<IActivity>().ToArray();

            // assert
            CollectionAssert.AreEqual(new[]
            {
                compositeActivityMock
            }, allActivities);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAllActivitiesRecursive_CompositeActivityWithoutChildrenNotMatchingRequestedActivityType_ReturnEmptyEnummerable()
        {
            // setup
            var activitiesList = new EventedList<IActivity>();

            var mocks = new MockRepository();
            var compositeActivityMock = mocks.StrictMock<ICompositeActivity>();
            compositeActivityMock.Expect(ca => ca.Activities).Return(activitiesList);
            mocks.ReplayAll();

            // call
            var allActivities = compositeActivityMock.GetAllActivitiesRecursive<ISomeActivity>().ToArray();

            // assert
            CollectionAssert.IsEmpty(allActivities);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAllActivitiesRecursive_CompositeActivityWithChildren_ReturnCompositeActivityAndChildren()
        {
            // setup
            var activitiesList = new EventedList<IActivity>();

            var mocks = new MockRepository();
            var compositeActivityMock = mocks.StrictMock<ICompositeActivity>();
            compositeActivityMock.Expect(ca => ca.Activities).Return(activitiesList);
            var childActivity1 = mocks.StrictMock<IActivity>();
            var childActivity2 = mocks.StrictMock<ISomeActivity>();
            mocks.ReplayAll();

            activitiesList.AddRange(new[]
            {
                childActivity1,
                childActivity2
            });

            // call
            var allActivities = compositeActivityMock.GetAllActivitiesRecursive<IActivity>().ToArray();

            // assert
            var expected = new[]
            {
                compositeActivityMock,
                childActivity1,
                childActivity2
            };
            CollectionAssert.AreEqual(expected, allActivities);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAllActivitiesRecursive_CompositeActivityWithSomeChildrenMatching_ReturnOnlyMatchingChildren()
        {
            // setup
            var activitiesList = new EventedList<IActivity>();

            var mocks = new MockRepository();
            var compositeActivityMock = mocks.StrictMock<ICompositeActivity>();
            compositeActivityMock.Expect(ca => ca.Activities).Return(activitiesList);
            var childActivity1 = mocks.StrictMock<IActivity>();
            var childActivity2 = mocks.StrictMock<ISomeActivity>();
            mocks.ReplayAll();

            activitiesList.AddRange(new[]
            {
                childActivity1,
                childActivity2
            });

            // call
            var allActivities = compositeActivityMock.GetAllActivitiesRecursive<ISomeActivity>().ToArray();

            // assert
            CollectionAssert.AreEqual(new[]
            {
                childActivity2
            }, allActivities);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAllActivitiesRecursive_NestedCompositeActivities_ReturnAllActivities()
        {
            // setup
            var activitiesList1 = new EventedList<IActivity>();
            var activitiesList2 = new EventedList<IActivity>();
            var activitiesList3 = new EventedList<IActivity>();

            var mocks = new MockRepository();
            var compositeActivityMock1 = mocks.StrictMock<ICompositeActivity>();
            compositeActivityMock1.Expect(ca => ca.Activities).Return(activitiesList1);

            var childActivity1 = mocks.StrictMock<ICompositeActivity>();
            childActivity1.Expect(ca => ca.Activities).Return(activitiesList2);

            var childActivity2 = mocks.StrictMock<ICompositeActivity>();
            childActivity2.Expect(ca => ca.Activities).Return(activitiesList3);

            var sub1ChildActivity1 = mocks.StrictMock<IActivity>();
            var sub1ChildActivity2 = mocks.StrictMock<IActivity>();

            var sub2ChildActivity1 = mocks.StrictMock<IActivity>();
            var sub2ChildActivity2 = mocks.StrictMock<IActivity>();

            mocks.ReplayAll();

            activitiesList1.AddRange(new[]
            {
                childActivity1,
                childActivity2
            });
            activitiesList2.AddRange(new[]
            {
                sub1ChildActivity1,
                sub1ChildActivity2
            });
            activitiesList3.AddRange(new[]
            {
                sub2ChildActivity1,
                sub2ChildActivity2
            });

            // call
            var allActivities = compositeActivityMock1.GetAllActivitiesRecursive<IActivity>().ToArray();

            // assert
            var expected = new[]
            {
                compositeActivityMock1,
                childActivity1,
                sub1ChildActivity1,
                sub1ChildActivity2,
                childActivity2,
                sub2ChildActivity1,
                sub2ChildActivity2
            };
            CollectionAssert.AreEqual(expected, allActivities);
            mocks.VerifyAll();
        }

        public interface ISomeActivity : IActivity {}
    }
}