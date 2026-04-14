## Plan: Replace Rhino.Mocks with NSubstitute

Migrate test projects from Rhino.Mocks to NSubstitute in controlled phases using a dual-stack transition: shared test helpers expose both Rhino.Mocks and NSubstitute implementations until repository usage of Rhino is zero. This reduces regression risk around strict/replay semantics while keeping CI green and allowing incremental suite migration.

### Steps
1. [ ] Inventory Rhino usage and package references across `Riskeer/**/*.cs` and `Riskeer/**/*.csproj`, tagging `MockRepository`, `Expect`, `Stub`, `Arg`, `ReplayAll`, `VerifyAll`, `WhenCalled`, `IgnoreArguments`, `Repeat`.
2. [ ] Define and publish a migration rulebook in `DEPENDENCIES.md` with approved `NSubstitute` patterns and unsupported Rhino idioms.
3. [ ] Convert shared helpers first (for example `Riskeer/Common/test/Riskeer.Common.Data.TestUtil/AssessmentSectionTestHelper.cs`) to dual-stack helpers: keep Rhino-based overloads and add/keep NSubstitute overloads.
4. [ ] Execute a pilot in two heavy suites (`Riskeer/WaveImpactAsphaltCover/test/Riskeer.WaveImpactAsphaltCover.Service.Test`, `Riskeer/Common/test/Riskeer.Common.IO.Test`), then refine rules from real failures.
5. [ ] Roll out by domain test folders (Service -> Plugin -> Integration -> Forms/Data), updating each `*.csproj` from `RhinoMocks` to `NSubstitute`.
6. [ ] When Rhino usage reaches zero in tests, remove Rhino helper overloads and Rhino package references, then enforce a gate that blocks new `using Rhino.Mocks;` in `Riskeer.sln` scope.

### Dual-Stack Policy (Temporary)
1. Test helper projects may reference both `RhinoMocks` and `NSubstitute` during migration.
2. Helper APIs must keep Rhino-compatible overloads for existing tests and NSubstitute-first overloads for new tests.
3. No helper should remove Rhino overloads until repository-wide Rhino call sites are zero.
4. Per migration batch, Rhino usage must be non-increasing.

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
2. **Phase 1 - Foundations (Dual-Stack):** migrate shared `*TestUtil` helpers and common fixtures to provide both Rhino and NSubstitute implementations.
3. **Phase 2 - Pilot:** migrate selected high-usage suites to NSubstitute, capture edge cases (strict expectations, callbacks, argument matching), while helpers remain dual-stack.
4. **Phase 3 - Scale-out:** migrate remaining test projects by bounded domain batches with small PRs; track Rhino burn-down per batch.
5. **Phase 4 - Decommission:** after Rhino usage is zero, remove Rhino helper overloads and `RhinoMocks` package references, then add policy checks preventing reintroduction.

### Verification Gates
1. **Gate A (Baseline):** Rhino usage inventory approved; all migration rules mapped for discovered APIs.
2. **Gate B (Per phase):** Rhino usage in migrated scope is non-increasing; project builds and its tests pass.
3. **Gate C (Pilot exit):** translated callback/argument/count patterns validated in pilot suites.
4. **Gate D (Rollout exit):** all targeted migrated suites use `NSubstitute`; remaining Rhino usage is explicitly tracked and accepted only in not-yet-migrated suites/helpers.
5. **Gate E (Final):** repository-wide search finds zero Rhino APIs and zero Rhino package references, then CI remains green for full test matrix.

### Further Considerations
1. Should strict-mock intent be preserved exactly (Option A) or simplified to intent-focused `Received()` assertions (Option B)?
2. Choose rollout slicing: by subsystem ownership (Option A), by test type layer (Option B), or by risk/complexity ranking (Option C).
3. Decide enforcement timing: immediate fail on new Rhino references (Option A) versus warning-only during pilot (Option B).

