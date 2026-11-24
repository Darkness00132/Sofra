import { Link } from "react-router-dom"
import { FormTextField } from "@/shared/components/form-text-field"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useResetPasswordForm } from "../hooks/use-reset-password-form"

export function ResetPasswordPage() {
  const { form, formError } = useResetPasswordForm()

  return (
    <main>
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Reset your password</CardTitle>
          <CardDescription>
            Enter the code from your email and a new password.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form
            onSubmit={(event) => {
              event.preventDefault()
              form.handleSubmit()
            }}
            className="flex flex-col gap-4"
          >
            {formError ? (
              <p className="text-sm text-destructive">{formError}</p>
            ) : null}
            <form.Field name="email">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Email"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  type="email"
                  autoComplete="email"
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Field name="otp">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Reset code"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  inputMode="numeric"
                  autoComplete="one-time-code"
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Field name="newPassword">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="New password"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  type="password"
                  autoComplete="new-password"
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Field name="confirmPassword">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Confirm password"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  type="password"
                  autoComplete="new-password"
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Subscribe selector={(state) => state.isSubmitting}>
              {(isSubmitting) => (
                <Button type="submit" disabled={isSubmitting}>
                  {isSubmitting ? "Resetting…" : "Reset password"}
                </Button>
              )}
            </form.Subscribe>
          </form>
        </CardContent>
        <CardFooter>
          <Link to="/login" className="text-sm hover:underline">
            Back to sign in
          </Link>
        </CardFooter>
      </Card>
    </main>
  )
}
