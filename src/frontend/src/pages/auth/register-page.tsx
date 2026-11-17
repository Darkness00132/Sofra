import { Link } from "react-router-dom"
import { TextField } from "@/components/shared/text-field"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { useRegisterForm } from "@/features/auth/use-register-form"

export function RegisterPage() {
  const { form, formError } = useRegisterForm()

  return (
    <main className="flex min-h-svh items-center justify-center p-4">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Create your account</CardTitle>
          <CardDescription>
            Order from approved restaurants in minutes.
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
                <TextField
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
            <form.Field name="password">
              {(field) => (
                <TextField
                  id={field.name}
                  label="Password"
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
                <TextField
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
                  {isSubmitting ? "Creating account…" : "Create account"}
                </Button>
              )}
            </form.Subscribe>
          </form>
        </CardContent>
        <CardFooter>
          <Link to="/login" className="text-sm hover:underline">
            Already have an account? Sign in
          </Link>
        </CardFooter>
      </Card>
    </main>
  )
}
