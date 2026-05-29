## 1. Header Component Edits

- [x] 1.1 Update `frontend/src/components/Header.vue` template to wrap the status indicator and add a new settings gear button (`⚙️`) with `@click="triggerSettings"`, and a circular user avatar (`👤`) placeholder.
- [x] 1.2 Add CSS styling rules for `.header-right` flex alignment, `.settings-btn` 180-degree hover rotation transition, and `.user-avatar` glassmorphic rings inside `Header.vue`.
- [x] 1.3 Add the `triggerSettings()` method in `Header.vue` script section to execute `this.$emit('open-settings')` upon click.

## 2. Main Dashboard Integration

- [x] 2.1 Modify the `<Header :is-online="isOnline" />` element in `frontend/src/App.vue` template to listen for the custom `@open-settings` event and bind it to `openCategoryModal`.

## 3. Verification and Compilation

- [x] 3.1 Execute `npm run build` inside the `frontend/` directory to verify clean production compilation of the updated Vue components.

