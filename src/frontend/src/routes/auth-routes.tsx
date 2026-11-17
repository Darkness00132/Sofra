import type { RouteObject } from "react-router-dom"
import { ForgotPasswordPage } from "@/pages/auth/forgot-password-page"
import { LoginPage } from "@/pages/auth/login-page"
import { RegisterPage } from "@/pages/auth/register-page"
import { ResetPasswordPage } from "@/pages/auth/reset-password-page"

/** Route subtree for the authentication flow. */
export const authRoutes: RouteObject[] = [
  { path: "login", element: <LoginPage /> },
  { path: "register", element: <RegisterPage /> },
  { path: "forgot-password", element: <ForgotPasswordPage /> },
  { path: "reset-password", element: <ResetPasswordPage /> },
]
