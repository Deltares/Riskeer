## Plan: Replace Rhino.Mocks with NSubstitute

Migrate test projects from Rhino.Mocks to NSubstitute in controlled phases: establish an API translation baseline, convert shared test utilities first, run pilot conversions in high-usage test suites, then scale by domain and finally remove Rhino package references. This reduces regression risk around strict/replay semantics while keeping CI green and making future test maintenance easier.

### Steps
1. [ ] Inventory Rhino usage and package references across `Riskeer/**/*.cs` and `Riskeer/**/*.csproj`, tagging `MockRepository`, `Expect`, `Stub`, `Arg`, `ReplayAll`, `VerifyAll`, `WhenCalled`, `IgnoreArguments`, `Repeat`.
2. [ ] Define and publish a migration rulebook in `DEPENDENCIES.md` with approved `NSubstitute` patterns and unsupported Rhino idioms.
3. [ ] Convert shared helpers first (for example `Riskeer/Common/test/Riskeer.Common.Data.TestUtil/AssessmentSectionTestHelper.cs`) to stop propagating `MockRepository` usage.
4. [ ] Execute a pilot in two heavy suites (`Riskeer/WaveImpactAsphaltCover/test/Riskeer.WaveImpactAsphaltCover.Service.Test`, `Riskeer/Common/test/Riskeer.Common.IO.Test`), then refine rules from real failures.
5. [ ] Roll out by domain test folders (Service -> Plugin -> Integration -> Forms/Data), updating each `*.csproj` from `RhinoMocks` to `NSubstitute`.
6. [ ] Remove remaining Rhino references and enforce a gate that blocks new `using Rhino.Mocks;` in `Riskeer.sln` scope.

### API Mapping
| Rhino.Mocks pattern | NSubstitute equivalent | Notes |
|---|---|---|
| `new MockRepository(); repo.StrictMock<T>()/Stub<T>()` | `Substitute.For<T>()` | Prefer explicit `Received()` assertions over strict replay behavior. |
| `obj.Expect(x => x.Foo(a)).Return(v)` | `obj.Foo(a).Returns(v)` | For sequences use `Returns(v1, v2, ...)`. |
| `obj.Stub(x => x.Foo(a)).Return(v)` | `obj.Foo(a).Returns(v)` | Same API; behavior-driven naming only. |
| `Arg<T>.Is.Anything` / `IgnoreArguments()` | `Arg.Any<T>()` | Use targeted matchers instead of broad ignore when possible. |
| `Arg<T>.Is.NotNull` / `Arg<T>.Matches(...)` | `Arg.Is<T>(...)` | Keep predicate close to assertion for readability. |
| `WhenCalled(inv => ...)` | `Returns(ci => ...)` or `When(x => x.Foo(...)).Do(ci => ...)` | Use `Do` for side effects on `void` calls. |
| `ReplayAll()` | _remove_ | NSubstitute has no record/replay phase. |
| `VerifyAll()` | `Received()` / `DidNotReceive()` | Assert only behavior that matters to test intent. |
| `Repeat.Twice()/Once()/Never()` | `Received(2/1/0)` | Use exact counts only where contract requires it. |
| `Throw(new Ex())` | `Returns(_ => throw new Ex())` or `When(...).Do(_ => throw ...)` | Pick based on return type vs `void`. |

### Phased Rollout
1. **Phase 0 - Baseline:** create inventory and migration rulebook; freeze new Rhino usage.
2. **Phase 1 - Foundations:** migrate shared `*TestUtil` helpers and common fixtures first.
3. **Phase 2 - Pilot:** migrate selected high-usage suites, capture edge cases (strict expectations, callbacks, argument matching).
4. **Phase 3 - Scale-out:** migrate remaining test projects by bounded domain batches with small PRs.
5. **Phase 4 - Decommission:** remove `RhinoMocks` package references and add policy checks preventing reintroduction.

### Verification Gates
1. **Gate A (Baseline):** Rhino usage inventory approved; all migration rules mapped for discovered APIs.
2. **Gate B (Per phase):** no `using Rhino.Mocks;` in migrated scope; project builds and its tests pass.
3. **Gate C (Pilot exit):** translated callback/argument/count patterns validated in pilot suites.
4. **Gate D (Rollout exit):** all targeted `*.csproj` use `NSubstitute`; no `RhinoMocks` `PackageReference` remains.
5. **Gate E (Final):** repository-wide search finds zero Rhino APIs and CI remains green for full test matrix.

### Further Considerations
1. Should strict-mock intent be preserved exactly (Option A) or simplified to intent-focused `Received()` assertions (Option B)?
2. Choose rollout slicing: by subsystem ownership (Option A), by test type layer (Option B), or by risk/complexity ranking (Option C).
3. Decide enforcement timing: immediate fail on new Rhino references (Option A) versus warning-only during pilot (Option B).

