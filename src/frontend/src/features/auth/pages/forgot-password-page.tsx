import { Link } from "react-router-dom"
import { FormTextField } from "@/shared/components/form-text-field"
import { Button, buttonVariants } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useForgotPasswordForm } from "../hooks/use-forgot-password-form"

export function ForgotPasswordPage() {
  const { form, formError, submittedEmail, clearSubmittedEmail } =
    useForgotPasswordForm()

  return (
    <main>
      <Card className="w-full max-w-sm">
        {submittedEmail ? (
          <>
            <CardHeader>
              <CardTitle>Check your inbox</CardTitle>
              <CardDescription>
                If an account exists for {submittedEmail}, a 6-digit code valid
                for 10 minutes has been sent.
              </CardDescription>
            </CardHeader>
            <CardFooter className="gap-3">
              <Link
                to={`/reset-password?email=${encodeURIComponent(submittedEmail)}`}
                className={buttonVariants()}
              >
                Reset password
              </Link>
              <Button variant="ghost" onClick={clearSubmittedEmail}>
                Use a different email
              </Button>
            </CardFooter>
          </>
        ) : (
          <>
            <CardHeader>
              <CardTitle>Forgot your password?</CardTitle>
              <CardDescription>
                We will email you a 6-digit reset code.
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
                <form.Subscribe selector={(state) => state.isSubmitting}>
                  {(isSubmitting) => (
                    <Button type="submit" disabled={isSubmitting}>
                      {isSubmitting ? "Sending…" : "Send reset code"}
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
          </>
        )}
      </Card>
    </main>
  )
}
