## Why

To enhance the visual profile of the ControleCompras application and make it feel like a personalized production-ready SaaS product, the dashboard header should include dynamic settings controls and a simulated logged-in user profile. Option B (a sleek settings gear next to a circular user avatar) provides a highly premium and clean aesthetic. The gear will feature a rotation hover micro-animation, and clicking it will automatically trigger the Categories Management Modal to unify dashboard configurations, while the avatar represents a logged-in premium account with responsive tooltip indicators.

## What Changes

- **Grouped Header Right Panel**: Group the API status indicator and new user controls into a single `.header-right` container in the header template.
- **Interactive Settings Gear**: Add a clickable gear button (`⚙️`) with transition hover rotation (`transform: rotate(180deg)`) and primary-glow colors. Bind the click handler to emit an event that opens the categories modal in `App.vue`.
- **Circular User Avatar**: Add a rounded, premium border-glass avatar container containing a simulated user initials or emoji placeholder (`👤`) displaying the user name on hover.

## Capabilities

### New Capabilities

<!-- None -->

### Modified Capabilities

- `shopping-dashboard`: Add a new requirement for header settings and simulated account presentation, detailing the interactive gear transition behavior and category management modal linking.

## Impact

- **Affected Code**: 
  - `frontend/src/components/Header.vue` (HTML template and style updates).
  - `frontend/src/App.vue` (event handler to open the category modal from the header component).
- **APIs**: None. No backend session changes are needed.
- **Dependencies**: None.
