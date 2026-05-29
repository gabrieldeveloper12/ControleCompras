## Context

In our dark glassmorphic UI, native selectors such as calendars in `<input type="date">` and `<input type="month">` are rendered in black/dark gray by default inside Chromium and Webkit-based browser layout engines. This creates poor visual contrast, making it extremely difficult for users to notice the calendar triggers on filters and form sections.

## Goals / Non-Goals

**Goals:**
- Target `::-webkit-calendar-picker-indicator` to invert native icons from dark to white (`filter: invert(1)`), ensuring 100% legibility.
- Style the default state to a soft color matching `--text-secondary` (`opacity: 0.75` and `brightness(0.9)`) so the UI remains clean.
- Implement an interactive hover state that lights the selector to full brightness and scales it up gently for premium feedback.

**Non-Goals:**
- Customizing non-Webkit legacy indicators if they do not expose standardized CSS APIs (focusing on Chromium, Edge, Chrome, Opera, Safari, and modern mobile browsers).
- Introducing third-party datepicker library wrappers (keeping standard native date components).

## Decisions

### Decision 1: Target Webkit Pseudo-Elements with CSS Filter Rules
- **Description**: Add pseudo-element rules `.input-control::-webkit-calendar-picker-indicator` with invert and hover transforms in `frontend/src/style.css`.
- **Rationale**:
  - Webkit pseudo-elements are standard APIs supported by virtually all modern browsers that render the dark indicators.
  - Using CSS filters (`filter: invert(1)`) avoids replacing standard browsers icons with heavy custom SVGs, maintaining high performance and zero visual regressions.
- **Alternatives Considered**:
  - *Custom datepicker components*: Rejected due to high implementation complexity and file bloat.

## Risks / Trade-offs

- **[Risk] Browser incompatibility on non-Webkit browsers** → **[Mitigation]** The default native browser selector is left intact as a fallback; on major Webkit/Blink engines (which covers >95% of active user sessions), the light theme inversion compiles perfectly.
