// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingInputContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var stochasticSoilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            var context = new PipingInputContext(pipingInput, surfaceLines, stochasticSoilModels, assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<PipingInput>>(context);
            Assert.AreSame(pipingInput, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            CollectionAssert.AreEqual(surfaceLines, context.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(stochasticSoilModels, context.AvailableStochasticSoilModels);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var stochasticSoilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            // Call
            TestDelegate call = () => new PipingInputContext(input, surfaceLines, stochasticSoilModels, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het traject mag niet 'null' zijn.");
        }

        [Test]
        public void SetStochasticSoilModelAndStochasticSoilProfileForSurfaceLine_NewStochasticSoilModelWithOneStochasticSoilProfile_SetsStochasticSoilProfile()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var stochasticSoilProfile = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            };

            var stochasticSoilModel = new StochasticSoilModel(1, "Model A", "Model B")
            {
                Geometry =
                {
                    new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                },
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile
                }
            };

            var pipingInput = new PipingInput(new GeneralPipingInput());

            pipingInput.SurfaceLine = surfaceLine;

            PipingInputContext pipingInputContext = new PipingInputContext(pipingInput, new[]
            {
                surfaceLine
            }, new[]
            {
                stochasticSoilModel
            }, assessmentSection);

            // Call
            pipingInputContext.SetStochasticSoilModelAndStochasticSoilProfileForSurfaceLine();

            // Assert
            Assert.AreEqual(stochasticSoilModel, pipingInputContext.WrappedData.StochasticSoilModel);
            Assert.AreEqual(stochasticSoilProfile, pipingInputContext.WrappedData.StochasticSoilProfile);

            mocks.VerifyAll();
        }

        [Test]
        public void SetStochasticSoilModelAndStochasticSoilProfileForSurfaceLine_NewStochasticSoilModelWithMultipleStochasticSoilProfiles_SetsStochasticSoilProfileToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var stochasticSoilModel = new StochasticSoilModel(1, "Model A", "Model B")
            {
                Geometry =
                {
                    new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                    {
                        SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                        {
                            new PipingSoilLayer(-5.0),
                            new PipingSoilLayer(-2.0),
                            new PipingSoilLayer(1.0)
                        }, SoilProfileType.SoilProfile1D, 1)
                    },
                    new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                    {
                        SoilProfile = new PipingSoilProfile("Profile 2", -10.0, new[]
                        {
                            new PipingSoilLayer(-5.0),
                            new PipingSoilLayer(-2.0),
                            new PipingSoilLayer(1.0)
                        }, SoilProfileType.SoilProfile1D, 2)
                    }
                }
            };

            var pipingInput = new PipingInput(new GeneralPipingInput());
            pipingInput.SurfaceLine = surfaceLine;

            PipingInputContext pipingInputContext = new PipingInputContext(pipingInput, new[]
            {
                surfaceLine
            }, new[]
            {
                stochasticSoilModel
            }, assessmentSection);

            // Call
            pipingInputContext.SetStochasticSoilModelAndStochasticSoilProfileForSurfaceLine();

            // Assert
            Assert.AreEqual(stochasticSoilModel, pipingInputContext.WrappedData.StochasticSoilModel);
            Assert.IsNull(pipingInputContext.WrappedData.StochasticSoilProfile);

            mocks.VerifyAll();
        }

        [Test]
        public void SetStochasticSoilModelAndStochasticSoilProfileForSurfaceLine_MatchingStochasticSoilProfileAlreadySet_DoesNotSetsStochasticSoilProfileToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

           

            var stochasticSoilProfile = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            };

            var stochasticSoilModel = new StochasticSoilModel(1, "Model A", "Model B")
            {
                Geometry =
                {
                    new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                },
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile,
                    new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                    {
                        SoilProfile = new PipingSoilProfile("Profile 2", -10.0, new[]
                        {
                            new PipingSoilLayer(-5.0),
                            new PipingSoilLayer(-2.0),
                            new PipingSoilLayer(1.0)
                        }, SoilProfileType.SoilProfile1D, 2)
                    }
                }
            };

            var pipingInput = new PipingInput(new GeneralPipingInput());
            pipingInput.SurfaceLine = surfaceLine;

            PipingInputContext pipingInputContext = new PipingInputContext(pipingInput, new[]
            {
                surfaceLine
            }, new[]
            {
                stochasticSoilModel
            }, assessmentSection);

            // Precondition
            pipingInputContext.WrappedData.StochasticSoilModel = stochasticSoilModel;
            pipingInputContext.WrappedData.StochasticSoilProfile = stochasticSoilProfile;

            // Call
            pipingInputContext.SetStochasticSoilModelAndStochasticSoilProfileForSurfaceLine();

            // Assert
            Assert.AreEqual(stochasticSoilModel, pipingInputContext.WrappedData.StochasticSoilModel);
            Assert.AreEqual(stochasticSoilProfile, pipingInputContext.WrappedData.StochasticSoilProfile);

            mocks.VerifyAll();
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToPipingInput()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());
            var context = new PipingInputContext(pipingInput,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 assessmentSectionMock);

            // Call
            context.Attach(observer);

            // Assert
            pipingInput.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromPipingInput()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());
            var context = new PipingInputContext(pipingInput,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 assessmentSectionMock);

            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            pipingInput.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToPipingInput_NotificationCorrectlyPropagated()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());
            var context = new PipingInputContext(pipingInput,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                 assessmentSectionMock);

            pipingInput.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mocks.VerifyAll();
        }
    }
}