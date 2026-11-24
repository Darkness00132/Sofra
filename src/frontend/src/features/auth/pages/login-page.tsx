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
import { useLoginForm } from "../hooks/use-login-form"

export function LoginPage() {
  const { form, formError } = useLoginForm()

  return (
    <main>
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Sign in to Sofra</CardTitle>
          <CardDescription>Welcome back.</CardDescription>
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
            <form.Field name="password">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Password"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  type="password"
                  autoComplete="current-password"
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Subscribe selector={(state) => state.isSubmitting}>
              {(isSubmitting) => (
                <Button type="submit" disabled={isSubmitting}>
                  {isSubmitting ? "Signing in…" : "Sign in"}
                </Button>
              )}
            </form.Subscribe>
          </form>
        </CardContent>
        <CardFooter className="justify-between">
          <Link
            to="/forgot-password"
            className="text-sm text-muted-foreground hover:underline"
          >
            Forgot password?
          </Link>
          <Link to="/register" className="text-sm hover:underline">
            Create account
          </Link>
        </CardFooter>
      </Card>
    </main>
  )
}
