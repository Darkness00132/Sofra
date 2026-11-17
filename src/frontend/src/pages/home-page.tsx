import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { useLogoutMutation } from "@/state/mutations/use-logout-mutation"
import { useProfileQuery } from "@/state/queries/use-profile-query"
import { useAuthStore } from "@/state/stores/auth-store"

/**
 * Temporary composition demo for the scaffold: exercises the profile query,
 * the auth store snapshot and the logout mutation against the live API.
 */
export function HomePage() {
  const sessionUser = useAuthStore((state) => state.user)
  const refreshToken = useAuthStore((state) => state.refreshToken)
  const profile = useProfileQuery()
  const logout = useLogoutMutation()

  const user = profile.data ?? sessionUser

  return (
    <main className="mx-auto flex min-h-svh max-w-md items-center justify-center p-4">
      <Card className="w-full">
        <CardHeader>
          <CardTitle>Sofra</CardTitle>
          <CardDescription>{user?.email ?? "Signed in"}</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-1 text-sm">
          {profile.isLoading ? (
            <p className="text-muted-foreground">Loading profile…</p>
          ) : profile.isError ? (
            <p className="text-destructive">Could not load your profile.</p>
          ) : (
            <>
              <p>
                <span className="text-muted-foreground">Roles: </span>
                {user?.roles.length ? user.roles.join(", ") : "none"}
              </p>
              {user?.phoneNumber ? (
                <p>
                  <span className="text-muted-foreground">Phone: </span>
                  {user.phoneNumber}
                </p>
              ) : null}
            </>
          )}
        </CardContent>
        <CardFooter>
          <Button
            variant="outline"
            disabled={logout.isPending || !refreshToken}
            onClick={() => {
              if (refreshToken) logout.mutate({ refreshToken })
            }}
          >
            {logout.isPending ? "Signing out…" : "Sign out"}
          </Button>
        </CardFooter>
      </Card>
    </main>
  )
}
