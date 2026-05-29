## 1. Input UI and State Setup

- [x] 1.1 Update the "Valor (R$)" input field in `frontend/src/App.vue` to `type="text"` and set `inputmode="decimal"`. Change its `v-model` mapping from `formCompra.valor` to a new reactive string property `valorExibido`.
- [x] 1.2 Bind the `@focus` event to `onValorFocus` and the `@blur` event to `onValorBlur` on the input element.
- [x] 1.3 Add the `valorExibido` string property initialized to `""` in the reactive `data()` model of `App.vue`.

## 2. Event Handlers and Formatting Utilities

- [x] 2.1 Implement the `formatDecimalBrl(val)` helper method under `methods` in `App.vue` to format raw numbers to Brazilian currency strings without the `R$` prefix (e.g. `2.5` to `2,50`).
- [x] 2.2 Implement the `onValorBlur` event handler in `App.vue` to sanitize user input, substitute comma separators, parse the numeric value, and re-format the display value using Brazilian notation.
- [x] 2.3 Implement the `onValorFocus` event handler in `App.vue` to clean the BRL presentation characters (thousands separator dots and non-numeric symbols) on focus, keeping the value edit-friendly.

## 3. CRUD Integration and Normalisation

- [x] 3.1 Modify `startEdit(compra)` in `App.vue` to automatically format the raw numeric purchase value (`compra.valor`) and populate `valorExibido`.
- [x] 3.2 Modify `cancelEdit()` in `App.vue` to reset `valorExibido` to an empty string.
- [x] 3.3 Modify `saveCompra()` in `App.vue` to retrieve `valorExibido`, normalise commas into dots, validate the parsed float (ensuring it is a positive number > 0), and correctly inject the floating-point value into the JSON POST/PUT payload.

## 4. Verification and Compilation

- [x] 4.1 Execute `npm run build` inside the `frontend/` directory to verify compile-time safety and compiler compliance of the updated Options API Vue codebase.
