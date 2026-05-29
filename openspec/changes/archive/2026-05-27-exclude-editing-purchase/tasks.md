## 1. Vue Component Updates

- [x] 1.1 Add the `filteredCompras` computed property inside the `computed` block in `frontend/src/App.vue` to filter out the purchase record currently matching `this.formCompra.id` when in `editMode`.
- [x] 1.2 Update the `filteredCountText` computed property in `frontend/src/App.vue` to evaluate `this.filteredCompras.length` instead of `this.compras.length` so the displayed count matches visible table rows.
- [x] 1.3 Update the purchase table container empty states and loop rows in `frontend/src/App.vue` to bind to `filteredCompras` instead of `compras`.

## 2. Verification and Compilation

- [x] 2.1 Execute `npm run build` inside the `frontend/` directory to verify clean production compilation of the updated Vue SPA.
