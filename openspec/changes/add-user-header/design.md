## Context

The current `Header.vue` displays a text logo on the left and a simple connection status tag on the right. To improve personalization and make the app resemble a complete premium personal SaaS system, we will group the right controls under a modern `.header-right` layout and include an interactive settings gear and user profile avatar.

## Goals / Non-Goals

**Goals:**
- Implement a grouped layout in `Header.vue` containing the connection status, a settings gear button (`⚙️`), and a circular avatar.
- Create smooth CSS transformations (`transform: rotate(180deg)`) and primary-glow highlights on the gear button when hovered.
- Emit a custom event `@open-settings` from the header component, listening to it in `App.vue` to open the Category Management modal directly.

**Non-Goals:**
- Modifying backend server session contexts (it remains a simulated personal local-only session).
- Inserting heavy SVG graphic asset libraries (using sleek browser emojis and standard CSS borders/glassmorphism).

## Decisions

### Decision 1: Group Controls in a `.header-right` Flex Container
- **Description**: Wrap the status indicator, gear button, and avatar in a flex container with `gap: 1.25rem` and `align-items: center` in `Header.vue`.
- **Rationale**: Keeps the right side clean, well-aligned, and fully responsive across both desktop and mobile column flows.

### Decision 2: Custom Vue Component Event Emittance for Settings Modal Trigger
- **Description**: Bind a click handler to the gear button in `Header.vue` that executes `this.$emit('open-settings')`.
- **Rationale**:
  - Unifies the application settings; instead of duplicate modal logic or heavy global state managers (like Pinia), standard Vue event mapping handles the trigger elegantly.

## Risks / Trade-offs

- **[Risk] Small tap targets on mobile screen overlays** → **[Mitigation]** The gear button and avatar wrapper will have generous padding and cursor pointers (`min-width: 36px`, `min-height: 36px`) to ensure comfortable tap sizes on tactile devices.
