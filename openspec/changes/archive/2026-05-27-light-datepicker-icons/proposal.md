## Why

Currently, native datepicker indicators (`input type="date"` and `input type="month"`) render as dark gray SVG glyphs by default in Chromium-based browsers. In our glassmorphic premium dark theme, these icons blend into the input text area backgrounds, making them barely visible to the user. Enhancing the color and visibility of these picker indicators using native CSS Webkit pseudo-elements with filters guarantees high visual contrast, premium aesthetic alignment, and perfect legibility.

## What Changes

- **Light Datepicker Icons**: Invert browser-default dark calendar selectors to white using CSS filters (`filter: invert(1)`).
- **Legibility Opacity**: Set a polished resting opacity (`0.75`) that matches other text controls (`--text-secondary`).
- **Interactive Hover Feedbacks**: Scale up the icon slightly and light it to 100% opacity and brightness when the cursor hovers over it.

## Capabilities

### New Capabilities

<!-- None -->

### Modified Capabilities

- `shopping-dashboard`: Modify the `Adaptação de Formulários e Modais de Toque` requirement to add visual legibility expectations for browser-native calendar and datepicker indicators, ensuring high visual contrast in dark mode.

## Impact

- **Affected Code**: `frontend/src/style.css` (appends custom styles to `.input-control`).
- **APIs**: None.
- **Dependencies**: None. Purely CSS-native.
