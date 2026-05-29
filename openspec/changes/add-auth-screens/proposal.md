## Why

We currently don't have user authentication screens in the application. To allow users to securely access their accounts and specific dashboard features, we need a complete authentication flow. This sets the foundation for personal user data management.

## What Changes

- Add a Login screen with email and password fields.
- Add a Sign Up screen with email, password, and confirm password fields.
- Add a Forgot Password screen requesting an email address to send a recovery link.
- Implement isolated routes for each authentication view (`/login`, `/signup`, `/forgot-password`).
- Set up a Pinia store (`AuthStore`) to manage user state and JWT tokens.
- Add an AuthService to handle API communications for auth routes.

## Capabilities

### New Capabilities
- `user-authentication`: Covers login, sign-up, and password recovery interfaces, including state management for JWT tokens.

### Modified Capabilities

## Impact

- **Frontend Routing:** Adds new top-level routes to Vue Router.
- **State Management:** Introduces a new Pinia store for authentication.
- **UI Components:** Creates new view components with a centralized card layout design.
- **Dependencies:** May require installing or using existing validation libraries for form inputs.
