import type { RouteObject } from "react-router-dom"
import { ForgotPasswordPage } from "./pages/forgot-password-page"
import { LoginPage } from "./pages/login-page"
import { ProfilePage } from "./pages/profile-page"
import { RegisterPage } from "./pages/register-page"
import { ResetPasswordPage } from "./pages/reset-password-page"

/** Unauthenticated screens. The app router mounts these under AuthLayout. */
export const guestAuthRoutes: RouteObject[] = [
  { path: "login", element: <LoginPage /> },
  { path: "register", element: <RegisterPage /> },
  { path: "forgot-password", element: <ForgotPasswordPage /> },
  { path: "reset-password", element: <ResetPasswordPage /> },
]

/** Signed-in profile. The app router mounts this under the customer shell. */
export const profileRoute: RouteObject = {
  path: "profile",
  element: <ProfilePage />,
}
