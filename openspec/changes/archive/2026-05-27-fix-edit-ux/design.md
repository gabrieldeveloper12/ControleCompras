## Context

The purchase form is positioned at the top of the interface in mobile and stacked column viewports, whereas the table of purchase records is at the bottom. When users trigger "Edit" (✏️) on a table record, the browser screen remains static at the bottom table row, offering zero feedback and making the application feel unresponsive. Furthermore, the `compra.data.split` function lacks default fallbacks, making it fragile if date values are malformed or missing.

## Goals / Non-Goals

**Goals:**
- Implement a smooth, browser-native scroll transition when entering edit mode, focusing the viewport automatically on the description input field.
- Add defensive fallback checks around `compra.data` splitting to prevent JavaScript console exceptions.

**Non-Goals:**
- Adding heavy JQuery or external animation scroll libraries (utilizing standard native browser `Element.scrollIntoView` API).
- Overhauling the core layout columns; we preserve the approved design, solving the UX mismatch through simple and effective interactions.

## Decisions

### Decision 1: Use Element.scrollIntoView with Smooth Scrolling
- **Description**: Add a scroll execution `document.getElementById('descricao')?.scrollIntoView({ behavior: 'smooth', block: 'center' });` inside `startEdit(compra)`.
- **Rationale**:
  - `scrollIntoView` with `{ behavior: 'smooth' }` is fully standard in modern browsers.
  - Positioning the form description input at the `'center'` of the viewport guarantees high visibility on all device screen sizes.
- **Alternatives Considered**:
  - *`window.scrollTo` with calculated offsets*: Rejected because `scrollIntoView` is much more declarative and is automatically responsive to layout variations.

### Decision 2: Protect splitting logic defensively
- **Description**: Rewrite date split mapping to evaluate truthiness before executing `.split()`.
- **Rationale**: Prevents unexpected client-side crashes if any atypical or seeded SQLite data records are missing date structures.

## Risks / Trade-offs

- **[Risk] Quick double-clicking scroll lag** → **[Mitigation]** The native smooth scroll is lightweight and handled natively by the rendering thread, avoiding layout jank.
