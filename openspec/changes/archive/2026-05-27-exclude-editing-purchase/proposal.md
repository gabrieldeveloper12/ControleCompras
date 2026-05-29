## Why

When editing a purchase record, having it concurrently displayed in the registration table below can cause user confusion, since they see both the active form editing values and the unchanged database row at the same time. Temporarily hiding the purchase being edited from the list of visible table rows provides a much cleaner editing state. The row seamlessly disappears when "Edit" is clicked, and reappears updated upon saving or returns unchanged if canceled.

## What Changes

- **Hiding Active Editing Purchase**: Create a computed property `filteredCompras` in the Vue template that filters out the purchase currently being edited (matching `formCompra.id`).
- **Dynamic Table Binding**: Bind the main purchase table and empty table placeholders to `filteredCompras` instead of `compras`.

## Capabilities

### New Capabilities

<!-- None -->

### Modified Capabilities

- `shopping-dashboard`: Modify the `Formulário de Lançamento de Compra` requirement to state that the purchase being edited must be temporarily excluded from the visible purchases table list during active edit mode.

## Impact

- **Affected Code**: `frontend/src/App.vue` (the purchase table template iteration and Vuecomputed script block).
- **APIs**: None.
- **Dependencies**: None.
