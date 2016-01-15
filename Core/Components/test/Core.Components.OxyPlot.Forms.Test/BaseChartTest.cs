using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.OxyPlot.Forms.Test
{
    [TestFixture]
    public class BaseChartTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var chart = new BaseChart();

            // Assert
            Assert.IsInstanceOf<Control>(chart);
            Assert.AreEqual(75, chart.MinimumSize.Height);
            Assert.AreEqual(50, chart.MinimumSize.Width);
        }

        [Test]
        public void Data_SetToNull_EmptyData()
        {
            // Setup
            var mocks = new MockRepository();
            var chart = new BaseChart();
            var dataMock = new PointData(new Collection<Tuple<double, double>>());
            mocks.ReplayAll();

            chart.Data = new [] { dataMock };

            // Call
            chart.Data = null;
            
            // Assert
            Assert.IsEmpty(chart.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void Data_NotNull_DataSet()
        {
            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double,double>>());
            var lineData = new LineData(new Collection<Tuple<double,double>>());
            var areaData = new AreaData(new Collection<Tuple<double,double>>());

            // Call
            chart.Data = new ChartData[] { pointData, lineData, areaData };

            // Assert
            CollectionAssert.AreEqual(new ChartData[] {pointData, lineData, areaData}, chart.Data);
        }

        [Test]
        public void Data_NotKnownChartData_ThrowsNotSupportedException()
        {
            // Setup
            var chart = new BaseChart();
            var testData = new TestChartData();

            // Call
            TestDelegate test = () => chart.Data = new ChartData[] { testData };

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void Data_UpdateReturnedValue_DoesNotAlterData()
        {

            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double, double>>());
            chart.Data = new ChartData[] { pointData };

            var data = chart.Data;

            // Call
            data.Remove(pointData);
            data.Add(pointData);

            // Assert
            CollectionAssert.AreEqual(new ChartData[] { pointData }, chart.Data);
        }

        [Test]
        public void Data_SetNewData_NewDataSet()
        {

            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double, double>>());
            var otherPointData = new PointData(new Collection<Tuple<double, double>>());
            chart.Data = new ChartData[] { pointData };

            // Call
            chart.Data = new ChartData[] { otherPointData };

            // Assert
            CollectionAssert.AreEqual(new ChartData[] { otherPointData }, chart.Data);
        }

        [Test]
        public void SetVisibility_SerieNotOnChart_ThrowsInvalidOperationException()
        {
            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double, double>>());

            // Call
            TestDelegate test = () => chart.SetVisibility(pointData, true);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetVisibility_SerieOnChart_SetsDataVisibility(bool visibility)
        {
            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double, double>>());
            chart.Data = new ChartData[] { pointData };

            // Call
            chart.SetVisibility(pointData, visibility);

            // Assert
            Assert.AreEqual(visibility, pointData.IsVisible);
        }

        [Test]
        public void SetVisibility_ForNull_ThrowsArgumentNullException()
        {
            // Setup
            var chart = new BaseChart();

            // Call
            TestDelegate test = () => chart.SetVisibility(null, true);
            
            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void SetPosition_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var chart = new BaseChart();

            // Call
            TestDelegate test = () => chart.SetPosition(null, new Random(21).Next());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void SetPosition_SerieNotOnChart_ThrowsInvalidOperationException()
        {
            // Setup
            BaseChart chart = CreateTestBaseChart();

            // Call
            TestDelegate test = () => chart.SetPosition(new TestChartData(), 0);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(50)]
        public void SetPosition_SerieOnChartPositionOutsideRange_ThrowsInvalidOperationException(int position)
        {
            // Setup
            BaseChart chart = CreateTestBaseChart();

            // Call
            TestDelegate test = () => chart.SetPosition(new TestChartData(), position);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void SetPosition_SerieOnChartPositionInRange_SetsNewPosition(int position)
        {
            // Setup
            BaseChart chart = CreateTestBaseChart();
            var testElement = chart.Data.ElementAt(new Random(21).Next(0,3));

            // Call
            chart.SetPosition(testElement, position);

            // Assert
            Assert.AreSame(testElement, chart.Data.ElementAt(position));
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observable = new BaseChart();
            observable.Attach(observer);

            // Call
            observable.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObserver_AttachedObserverDetachedAgain_ObserverNoLongerNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observable = new BaseChart();
            observable.Attach(observer);
            observable.Detach(observer);

            // Call
            observable.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void NotifyObservers_MultipleObserversDetachingOrAttachingOthers_NoUpdatesForAttachedAndDetachedObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var testObservable = new BaseChart();

            var observer1 = mocks.Stub<IObserver>();
            var observer2 = mocks.Stub<IObserver>();
            var observer3 = mocks.Stub<IObserver>();
            var observer4 = mocks.Stub<IObserver>();
            var observer5 = mocks.Stub<IObserver>();
            var observer6 = mocks.Stub<IObserver>();

            testObservable.Attach(observer1);
            testObservable.Attach(observer2);
            testObservable.Attach(observer3);
            testObservable.Attach(observer4);
            testObservable.Attach(observer6);

            observer1.Expect(o => o.UpdateObserver());
            observer2.Expect(o => o.UpdateObserver()).Do((Action)(() => testObservable.Detach(observer3)));
            observer3.Expect(o => o.UpdateObserver()).Repeat.Never(); // A detached observer should no longer be updated
            observer4.Expect(o => o.UpdateObserver()).Do((Action)(() => testObservable.Attach(observer5)));
            observer5.Expect(o => o.UpdateObserver()).Repeat.Never(); // An attached observer should not be updated either
            observer6.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            // Call
            testObservable.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        private static BaseChart CreateTestBaseChart()
        {
            return new BaseChart
            {
                Data = new ChartData[]
                {
                    new LineData(new List<Tuple<double,double>>()), 
                    new PointData(new List<Tuple<double,double>>()), 
                    new AreaData(new List<Tuple<double,double>>())
                }
            };
        }
    }
}