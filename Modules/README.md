# CRM Modular Migration

This directory is the new target architecture for `verii_verii_metivon_api`.

Canonical reference: `../docs/CRM_REFERENCE_ARCHITECTURE.md`

Migration rules for the current transition:

- New code should prefer `Modules/<Feature>` over legacy technical folders.
- Each feature module is organized as `Api`, `Application`, `Domain`, and `Infrastructure`.
- Shared cross-cutting concerns live under `Shared`.
- Legacy code remains in place until a module is migrated safely and verified.
- `_old` is reserved for fully retired legacy folders after their replacements are stable.

Daily module rules:

- Controllers only translate HTTP to service calls; no business rule, transaction, mapping, or DbContext logic belongs there.
- Services own business rules, transaction boundaries, mapping orchestration, localization messages, and `SaveChanges` timing.
- Repositories stay as data access helpers; they must not produce API responses or hide business rules.
- Read-only EF queries use `AsNoTracking` unless tracking is explicitly required.
- Client-facing contracts are DTOs; entities are not returned from controllers or services.
- List endpoints accept `PagedRequest` and return `PagedResponse<T>`.
- Paged list services use `Shared/Common/Application/PagedQueryExtensions.cs`; do not write local `CountAsync + Skip/Take` paging loops.
- Search behavior must stay Turkish/case/punctuation tolerant through shared query helpers or indexed normalized fields.
- High-volume search paths, especially stock/customer/document lists, need a perf smoke test before release.
- User-visible messages, including validation and business errors, go through module localization resources.
- New migrations use intentional names and must be reviewed with the related entity/configuration changes.
- Dirty worktree changes from another task are not reverted while migrating a module; isolate commits by logical scope.

Planned first migration candidates:

1. `Definitions`
2. `System`
3. `Customer`
4. `Identity`
5. `AccessControl`

High-risk modules that should move after the foundation is stable:

1. `Quotation`
2. `Demand`
3. `Order`
4. `Approval`
5. `Integrations`
6. `Reporting`
