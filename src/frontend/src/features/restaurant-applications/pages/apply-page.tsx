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
import { useApplyForm } from "../hooks/use-apply-form"

/**
 * Apply for a Sofra storefront: the application goes to administrators for
 * review (FR-11); the storefront is created only after approval (BR-01).
 */
export function ApplyPage() {
  const { form, formError } = useApplyForm()

  return (
    <main className="mx-auto flex min-h-svh max-w-md items-center justify-center p-4">
      <Card className="w-full">
        <CardHeader>
          <CardTitle>Apply for a Sofra storefront</CardTitle>
          <CardDescription>
            Administrators review every application before the restaurant goes
            live (BR-01).
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
            <form.Field name="restaurantName">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Restaurant name"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Field name="address">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Address"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Field name="phone">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Phone"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Field name="description">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Description"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Subscribe selector={(state) => state.isSubmitting}>
              {(isSubmitting) => (
                <Button type="submit" disabled={isSubmitting}>
                  {isSubmitting ? "Submitting…" : "Submit application"}
                </Button>
              )}
            </form.Subscribe>
          </form>
        </CardContent>
        <CardFooter>
          <Link
            to="/restaurants/my-applications"
            className="text-sm hover:underline"
          >
            View my applications
          </Link>
        </CardFooter>
      </Card>
    </main>
  )
}
