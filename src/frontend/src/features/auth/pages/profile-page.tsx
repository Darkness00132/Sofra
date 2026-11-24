import { UserRound } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { FormTextField } from "@/shared/components/form-text-field"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useProfileQuery } from "../service/api"
import { useUpdateProfileForm } from "../hooks/use-update-profile-form"

/**
 * The signed-in user's profile (FR-03): view the account and update the
 * phone number. Email changes are a product decision (PRD §10) — not offered.
 */
export function ProfilePage() {
  const profile = useProfileQuery()
  const { form, formError } = useUpdateProfileForm()

  return (
    <main className="mx-auto flex w-full max-w-md flex-col gap-4 px-4 py-10">
      <SurfaceHeader
        icon={UserRound}
        kicker="Customer"
        title="Your profile"
        description={profile.data?.email ?? "The phone number restaurants can reach you on."}
      />
      <Card className="w-full">
        <CardHeader>
          <CardTitle>Profile</CardTitle>
          <CardDescription>{profile.data?.email}</CardDescription>
        </CardHeader>
        <CardContent>
          {profile.isLoading ? (
            <PageLoader />
          ) : (
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
              <form.Field name="phoneNumber">
                {(field) => (
                  <FormTextField
                    id={field.name}
                    label="Phone number"
                    value={field.state.value}
                    errors={field.state.meta.errors}
                    inputMode="text"
                    onBlur={field.handleBlur}
                    onChange={(value) => field.handleChange(value)}
                  />
                )}
              </form.Field>
              {profile.data?.roles.length ? (
                <p className="text-sm text-muted-foreground">
                  Roles: {profile.data.roles.join(", ")}
                </p>
              ) : null}
              <form.Subscribe selector={(state) => state.isSubmitting}>
                {(submitting) => (
                  <Button type="submit" disabled={submitting}>
                    {submitting ? "Saving…" : "Save"}
                  </Button>
                )}
              </form.Subscribe>
            </form>
          )}
        </CardContent>
      </Card>
    </main>
  )
}
