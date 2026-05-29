## Context

The application currently has no user authentication mechanisms. The goal is to provide specific user dashboard features, so we need a foundational authentication system. 

## Goals / Non-Goals

**Goals:**
- Provide frontend routes and views for Login, Signup, and Forgot Password.
- Define the UI architecture for these screens (Centralized card component).
- Implement a state management store for storing JWT and user authentication status.

**Non-Goals:**
- Backend API implementation (this is strictly frontend UI and auth store integration).
- OAuth providers (Google, Facebook) - only Email and Password are included in this initial phase.

## Decisions

- **Store:** We will use Pinia for state management (`AuthStore`), as it's the standard for modern Vue 3 applications.
- **Routing:** Vue Router will handle individual routes (`/login`, `/signup`, `/forgot-password`) rather than a single component with internal conditional rendering, to allow direct linking.
- **Styling Strategy:** Use the existing CSS strategy with a standard central card layout.
- **Token Management:** The JWT token will be stored in `localStorage` for persistence across page reloads, and managed via `AuthStore`. A service module (`AuthService`) will handle API communication.

## Risks / Trade-offs

- **Security with localStorage:** Storing JWTs in `localStorage` can be vulnerable to XSS attacks. 
  - *Mitigation:* Ensure strict sanitization and consider HttpOnly cookies for long-term security in a future backend iteration.
