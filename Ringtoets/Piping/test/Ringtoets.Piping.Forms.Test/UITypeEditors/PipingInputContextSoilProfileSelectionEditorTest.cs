using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Test.PresentationObjects;
using Ringtoets.Piping.Forms.UITypeEditors;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextSoilProfileSelectionEditorTest
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

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            var pipingInputContext = new PipingInputContext(pipingInput,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            new[]
                                                            {
                                                                new TestStochasticSoilModel()
                                                            },
                                                            assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = pipingInputContext
            };

            var editor = new PipingInputContextSoilProfileSelectionEditor();
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

            var soilProfile = new TestPipingSoilProfile();
            var stochasticSoilModel = new StochasticSoilModel(0, string.Empty, string.Empty)
            {
                Geometry = 
                {
                    new Point2D(0,2),
                    new Point2D(4,2)
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 0)
                    {
                        SoilProfile = soilProfile
                    }
                }
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new []
            {
                new Point3D(2, 1, 0), 
                new Point3D(2, 3, 0)
            });

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };
            var inputParametersContext = new PipingInputContext(pipingInput,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                new[]
                                                                {
                                                                    stochasticSoilModel
                                                                },
                                                                assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = inputParametersContext
            };

            var editor = new PipingInputContextSoilProfileSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(soilProfile, result);

            mockRepository.VerifyAll();
        }
    }
}