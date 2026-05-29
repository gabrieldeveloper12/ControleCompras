## Why

When users click the edit button (✏️) on a purchase record at the bottom of the dashboard screen, the form inputs higher up on the screen are populated, but they are completely out of the viewport. This leads to a severe UX disconnect where users believe the edit button is non-functional. Adding automatic smooth scroll-into-view behavior when entering edit mode resolves this. Additionally, applying defensive date splitting protects the Vue 3 component against crashes in case of malformed dates or atypical string representations.

## What Changes

- **Automatic Scroll-Focus**: Automatically scroll the viewport smoothly to the purchase description input field upon clicking the edit button.
- **Defensive Date Splits**: Validate the date string presence before splitting it, reverting safely to today's date if missing.

## Capabilities

### New Capabilities

<!-- None -->

### Modified Capabilities

- `shopping-dashboard`: Modify the `Formulário de Lançamento de Compra` requirement to specify the automatic scroll-focus behavior upon entering edit mode, and date parsing robust safety.

## Impact

- **Affected Code**: `frontend/src/App.vue` (the `startEdit` method logic).
- **APIs**: None.
- **Dependencies**: None. Purely browser-native and Vue-native.
