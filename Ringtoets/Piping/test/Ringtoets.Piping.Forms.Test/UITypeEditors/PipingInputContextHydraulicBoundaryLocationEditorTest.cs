using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.UITypeEditors;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextHydraulicBoundaryLocationEditorTest
    {
        [Test]
        public void EditValue_NoCurrentItemInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase)
                                 .Return(hydraulicBoundaryDatabase)
                                 .Repeat.AtLeastOnce();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
            };
            var pipingInputContext = new PipingInputContext(pipingInput,
                                                            calculationItem,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            failureMechanism,
                                                            assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = pipingInputContext
            };

            var editor = new PipingInputContextHydraulicBoundaryLocationEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase)
                                 .Return(hydraulicBoundaryDatabase)
                                 .Repeat.AtLeastOnce();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var inputParametersContext = new PipingInputContext(pipingInput,
                                                                calculationItem,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = inputParametersContext
            };

            var editor = new PipingInputContextHydraulicBoundaryLocationEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, result);

            mockRepository.VerifyAll();
        }

        private class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
        {
            public TestHydraulicBoundaryLocation() : base(0, string.Empty, 0, 0) {}
        }
    }
}