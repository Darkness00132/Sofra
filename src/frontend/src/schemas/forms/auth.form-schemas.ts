import { z } from "zod"

/**
 * UX-facing form schemas, run by TanStack Form through its Standard Schema
 * support. Wire-level request shapes live in schemas/api — form schemas are
 * deliberately stricter (required fields, password policy, confirm match).
 * Password rules mirror the backend Identity policy (minimum 6 characters).
 */

export const loginFormSchema = z.object({
  email: z.email("Enter a valid email address."),
  password: z.string().min(1, "Password is required."),
})

export const registerFormSchema = z
  .object({
    email: z.email("Enter a valid email address."),
    password: z.string().min(6, "Password must be at least 6 characters long."),
    confirmPassword: z.string().min(1, "Confirm the password."),
  })
  .refine((data) => data.password === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Passwords do not match.",
  })

export const forgotPasswordFormSchema = z.object({
  email: z.email("Enter a valid email address."),
})

export const resetPasswordFormSchema = z
  .object({
    email: z.email("Enter a valid email address."),
    otp: z.string().regex(/^\d{6}$/, "Enter the 6-digit code from the email."),
    newPassword: z
      .string()
      .min(6, "Password must be at least 6 characters long."),
    confirmPassword: z.string().min(1, "Confirm the password."),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Passwords do not match.",
  })
