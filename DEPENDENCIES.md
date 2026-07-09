# Dependencies
The following table shows the libraries used in the repository.

## NSubstitute Migration Rulebook

Active migration from Rhino.Mocks to NSubstitute. All new test code must use NSubstitute. Rhino.Mocks overloads in shared helpers (`*TestUtil`) are kept until repository-wide Rhino usage is zero.

| Rhino.Mocks pattern | NSubstitute equivalent |
|---|---|
| `new MockRepository(); repo.StrictMock<T>()/Stub<T>()` | `Substitute.For<T>()` |
| `obj.Stub(x => x.Foo(a)).Return(v)` | `obj.Foo(a).Returns(v)` |
| `obj.Expect(x => x.Foo(a)).Return(v)` | `obj.Foo(a).Returns(v)` + `obj.Received().Foo(a)` at verify |
| `obj.Expect(x => x.VoidFoo(a))` | _(remove)_ + `obj.Received().VoidFoo(a)` at verify |
| `Arg<T>.Is.Anything` / `IgnoreArguments()` | `Arg.Any<T>()` |
| `WhenCalled(inv => ...)` / `.Do((Action)...)` | `.When(x => x.Foo()).Do(_ => ...)` |
| `Repeat.Never()` | `obj.DidNotReceive().Foo()` |
| `Repeat.Any()` | _(remove verification)_ |
| `Repeat.AtLeastOnce()` | `obj.Received().Foo()` |
| `Repeat.Twice()` | `obj.Received(2).Foo()` |
| `.Throw(new Ex())` on non-void | `.Returns(_ => throw new Ex())` |
| `.Throw(new Ex())` on void | `.When(x => x.VoidFoo()).Do(_ => throw new Ex())` |
| `ReplayAll()` | _(remove)_ |
| `VerifyAll()` | explicit `Received()` / `DidNotReceive()` calls |

**Structure preservation rules**
- Keep the original test layout (`Setup` / `Call` / `Assert`) intact wherever possible.
- When migrating `Repeat.Times(n)` / `Repeat.Once()` / `Repeat.Twice()`, keep the count intent near the original setup location by introducing a local `const`/variable there (for example `const int numberOfChangedProperties = 5;`) and reuse that variable in the later `Received(...)` assertion.
- Avoid moving verification intent unnecessarily far from its original location; prefer minimal, local edits over broader rewrites.
- Only introduce extra `Received()` assertions when they replace original Rhino verification intent.

**Scope:** ~400+ test files across Core and Riskeer. **Status:** Phase 2 - pilot complete; Phase 3 in progress (Core test suites migrated; Riskeer domain suites pending).



For each library the version and used license is shown. For the full license text of the library, refer to the [Licenses subfolder](licenses).

| Library                                   | Version     | License      | Source                                                                          |
|-------------------------------------------|-------------|--------------|---------------------------------------------------------------------------------|
| AssemblyTool                              | 23.1.1      | LGPL-3.0     | https://github.com/Deltares/wbi-assemblage-rekenkern                            |
| AvalonDock                                | 2.0.2000    | BSD-3-Clause | https://www.google.com/search?q=AvalonDock                                      |
| BruTile.Desktop                           | 3.1.3       | Apache-2.0   | https://github.com/BruTile/BruTile                                              |
| BruTile                                   | 3.1.3       | Apache-2.0   | https://github.com/BruTile/BruTile                                              |
| ControlzEx                                | 4.4.0       | MIT          | https://github.com/ControlzEx/ControlzEx                                        |
| DotSpatial.Controls                       | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Data                           | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Data.Forms                     | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Extensions                     | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.GeoAPI                         | 1.7.4.3     | MIT          | https://github.com/DotSpatial/GeoAPI                                            |
| DotSpatial.Modeling.Forms                 | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Mono                           | 1.9.0       | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.NetTopologySuite               | 1.14.4      | BSD-3-Clause | https://github.com/DotSpatial/NetTopologySuiteV1                                |
| DotSpatial.NTSExtension                   | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Projections                    | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Projections.Forms              | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Serialization                  | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Symbology                      | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Symbology.Forms                | 2.0.0-rc1   | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| DotSpatial.Topology                       | 1.9.0       | MIT          | https://github.com/DotSpatial/DotSpatial                                        |
| D-Stability                               | 20.2.1      | LGPL-3.0     | https://github.com/Deltares/D-GEO-Suite-Stability                               |
| EntityFramework                           | 6.4.4       | Apache-2.0   | https://github.com/dotnet/ef6                                                   |
| FontAwesome.Sharp                         | 5.15.3      | Apache-2.0   | https://github.com/awesome-inc/FontAwesome.Sharp                                |
| GeoAPI                                    | 1.7.4       | LGPL-2.1     | https://github.com/NetTopologySuite/GeoAPI                                      |
| GraphSharp                                | 1.1.0       | Apache-2.0   | https://www.nuget.org/packages/GraphSharp/                                      |
| Hydra-Ring                                | 25.2.2      | AGPL-3.0     | https://github.com/Deltares/Hydra-Ring                                          |
| log4net                                   | 2.0.12      | Apache-2.0   | https://github.com/apache/logging-log4net                                       |
| MacroStability                            | 22.1.1      | AGPL-3.0     | https://github.com/Deltares/D-GEO-Suite-Stability-Kernel-Wrapper                |
| MahApps.Metro                             | 2.4.4       | MIT          | https://github.com/MahApps/MahApps.Metro                                        |
| MathNet.Numerics                          | 3.19.0      | MIT          | https://github.com/mathnet/mathnet-numerics                                     |
| MathNet.Spatial                           | 0.3.0       | MIT          | https://github.com/mathnet/mathnet-spatial                                      |
| Microsoft.Xaml.Behaviors.Wpf              | 1.1.31      | MIT          | https://github.com/Microsoft/XamlBehaviorsWpf                                   |
| MSBuildConfigurationDefaults              | 1.0.1       | MIT          | https://github.com/pedrolamas/MSBuildConfigurationDefaults                      |
| NetTopologySuite                          | 1.14.0      | LGPL-2.1     | https://github.com/NetTopologySuite/NetTopologySuite                            |
| NUnit                                     | 3.8.1       | MIT          | https://github.com/nunit/nunit                                                  |
| OxyPlot.Core                              | 1.0.0       | MIT          | https://github.com/oxyplot/oxyplot                                              |
| OxyPlot.WindowsForms                      | 1.0.0       | MIT          | https://github.com/oxyplot/oxyplot                                              |
| Piping                                    | 16.2.1.4574 | AGPL-3.0     | https://repos.deltares.nl/repos/FailureMechanisms/FailureMechanisms/DikesPiping |
| QuickGraph                                | 3.6.61119.7 | MS-PL        | https://www.nuget.org/packages/QuickGraph                                       |
| NSubstitute                               | 4.4.0       | BSD-3-Clause | https://github.com/nsubstitute/NSubstitute                                     |
| RhinoMocks                                | 3.6.1       | BSD-3-Clause | https://github.com/hibernating-rhinos/rhino-mocks (migration to NSubstitute in progress; kept for dual-stack helpers) |
| SmartThreadPool.dll                       | 2.2.4       | MS-PL        | https://github.com/amibar/SmartThreadPool                                       |
| Stub.System.Data.SQLite.Core.NetFramework | 1.0.117     | MS-PL        | https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki          |
| System.Data.SQLite.Core                   | 1.0.117     | MS-PL        | https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki          |
| System.Data.SQLite.EF6                    | 1.0.117     | MS-PL        | https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki          |
| WixToolset.Heat                           | 5.0.2       | MS-RL        | https://github.com/wixtoolset                                                   |
| WixToolset.NetFx.wixext                   | 5.0.2       | MS-RL        | https://github.com/wixtoolset                                                   |
| WixToolset.UI.wixext                      | 5.0.2       | MS-RL        | https://github.com/wixtoolset                                                   |
| WixToolset.Util.wixext                    | 5.0.2       | MS-RL        | https://github.com/wixtoolset                                                   |
| WPFExtensions                             | 1.0.0       | MS-PL        | https://www.nuget.org/packages/WPFExtensions                                    |