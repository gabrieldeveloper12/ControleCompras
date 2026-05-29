## Why

The current currency/price input field ("Valor") uses `type="number"`, which does not provide an optimal typing experience for Brazilian users. It restricts custom decimal separators (commas) and displays incomplete formatting (e.g. showing "2" instead of "2,00"). By replacing this with a tolerant text input configured with `inputmode="decimal"`, mobile devices will display a friendly numeric decimal keypad, and typing integer numbers or numbers with commas will automatically format to a polished currency representation (e.g., "2" -> "2,00" or "2,5" -> "2,50") upon blur.

## What Changes

- **Tolerant Monetary Input Field**: Replace the standard `type="number"` with a text input (`type="text"`) that supports the browser-native `inputmode="decimal"` virtual keyboard behavior.
- **Auto-Formatting on Blur**: Implement a blur event handler that formats standard inputs (like `2`, `2.5`, `2,5`, `2,00`) into BRL currency styling (`R$ 2,00` or `2,00` displayed inside the field).
- **Graceful Focus Reset**: Restore a clean numeric string representation upon focusing the field to allow easy edits without fighting with custom BRL prefixes or formatters.
- **Normalisation on Save**: Automatically sanitize inputs by substituting commas with dots so the C# API correctly parses the decimal value before persisting it in SQLite.

## Capabilities

### New Capabilities

<!-- None -->

### Modified Capabilities

- `shopping-dashboard`: Modify the `Formulário de Lançamento de Compra` requirement to detail the advanced behavior of the `Valor` currency input field, ensuring automatic formatting on blur, mobile decimal keyboard support, and tolerance for Brazilian currency representations (commas).

## Impact

- **Affected Code**: `frontend/src/App.vue` (purchase registration form template and scripting logic).
- **APIs**: No changes to the backend APIs since inputs are normalised to standard decimal formats before being sent.
- **Dependencies**: None. Works completely with native Vue 3 features and browser capabilities.
