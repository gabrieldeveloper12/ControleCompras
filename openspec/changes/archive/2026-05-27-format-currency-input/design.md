## Context

The current shopping tracker application utilizes a native `<input type="number">` bound directly to a numeric float field `formCompra.valor` in `App.vue`. This poses several issues:
1. Mobile devices display generic keyboards instead of a numeric/decimal virtual layout configured for decimals (forcing users to toggle layouts).
2. It does not natively support typing commas as decimal separators on all platforms, which is the standard decimal separator in Brazil.
3. The display lacks elegant formatting (e.g. showing "2" instead of "2,00"), which looks unprofessional and decreases user confidence.

To solve this, we will replace it with a text input (`type="text"`) enhanced with `inputmode="decimal"`, native event formatting on blur, and clean digits on focus.

## Goals / Non-Goals

**Goals:**
- Enable mobile browsers to automatically render a numeric decimal keyboard when focusing the "Valor" field.
- Accept typing standard commas (`,`) and periods (`.`) as decimal separators, automatically formatting the input to a two-decimal Brazilian notation (e.g., `2` becomes `2,00`, `2.5` becomes `2,50`) upon losing focus.
- Seamlessly transition from a formatted display back to a plain editable value (e.g., `2,50` instead of a locked currency prefix) when the user focuses the field.
- Ensure inputs are fully validated and normalised to floating-point numbers before sending JSON requests to the ASP.NET Core backend.

**Non-Goals:**
- Adding external heavy masked-input libraries. We want to keep page loading times fast and maintain lightweight, framework-agnostic native event logic.
- Changing the backend C# database schema or API signatures; the backend continues to receive and expect double/decimal JSON fields.

## Decisions

### Decision 1: Text Input with Custom Formatting instead of Native `<input type="number">` or Third-Party Masks
- **Description**: Replace `<input type="number">` with `<input type="text" inputmode="decimal">`. Bind it to a separate display state `valorExibido` (string) rather than directly to `formCompra.valor` (number).
- **Rationale**:
  - `type="number"` prevents using custom string formats (like comma separation or localized BRL text) and has inconsistent cursor placement control in browsers.
  - Native `inputmode="decimal"` prompts mobile operating systems (iOS and Android) to open their decimal keypad with commas/periods, maintaining a perfect mobile UX.
  - A separate `valorExibido` property acts as a presentation buffer, leaving the core REST JSON schema intact without structural regressions.
- **Alternatives Considered**:
  - *Third-party masking package*: Rejected because it introduces external dependencies, increases bundle size, and risks compatibility issues with Vue 3's rendering pipeline.
  - *Keep `<input type="number">` with decimal step*: Rejected because it doesn't allow comma decimal entry or consistent presentation.

### Decision 2: Format on Blur, Cleanse on Focus
- **Description**: Attach `@focus="onValorFocus"` and `@blur="onValorBlur"` handlers to clean up formatting on focus and restore BRL notation on blur.
- **Rationale**:
  - **On Focus**: Remove standard non-numeric symbols like `R$` or thousands-separator dots, but preserve decimal commas so users can edit the numeric value immediately.
  - **On Blur**: Standardise inputs (replace `,` with `.`), parse, and call `.toLocaleString('pt-BR')` to show a polished price like `2,50`.
- **Alternatives Considered**:
  - *Real-time formatting*: Rejected because editing values in the middle of a string while typing is frustrating, cursor positions jump unexpectedly, and users are forced to type from right-to-left.

## Risks / Trade-offs

- **[Risk] User enters non-numeric text** → **[Mitigation]** The client-side form validation parses the field on save. If the float parsing yields `NaN` or a value `<= 0`, it triggers an alert and prevents form submission.
- **[Risk] Clipboard copy-paste containing text** → **[Mitigation]** The regex replacement sanitizes typical symbols (`R$`, dots, spaces) and filters out invalid structures.
