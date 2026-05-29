## Context

When editing a purchase record, the record currently remains visible in the registration table below. Having both the form being edited and the static row simultaneously visible causes user confusion. We can improve visual focus by temporarily hiding the active row being edited from the table.

## Goals / Non-Goals

**Goals:**
- Provide a clean computed property `filteredCompras` in Vue 3 that filters out the record matching `formCompra.id` if `editMode` is true.
- Bind the table loops and empty states to `filteredCompras`.

**Non-Goals:**
- Removing the purchase permanently from state or mutating the backend data (it remains in memory in the `compras` array).
- Affecting overall dashboard statistics (total expenses, donut chart values), which must still represent all records.

## Decisions

### Decision 1: Create a Vue Computed Property instead of mutating state
- **Description**: Add `filteredCompras()` inside `computed` in `App.vue` that filters the `compras` array, and bind the purchase table and row counts to it.
- **Rationale**:
  - Computed properties are highly declarative, automatically react to state shifts (like entering or canceling edit mode), and require zero manual state mutations.
  - Hiding the item only in the presentation layer preserves database data integrity and charts statistics.
- **Alternatives Considered**:
  - *Splicing items from `compras` array inside `startEdit`*: Rejected because it requires complex rollback logic on cancel/save and risks data loss.

## Risks / Trade-offs

- **[Risk] Visual jank when row disappears** → **[Mitigation]** The table already uses smooth layout rendering, and hiding a single row is highly standard and instant.
