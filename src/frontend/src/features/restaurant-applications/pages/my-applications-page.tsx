import { Link } from "react-router-dom"
import { PageLoader } from "@/shared/components/page-loader"
import { buttonVariants } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useMyApplicationsQuery } from "../service/api"

/**
 * Tracks the signed-in user's own applications and their review status
 * (FR-47 tracking side).
 */
export function MyApplicationsPage() {
  const applications = useMyApplicationsQuery()

  return (
    <main className="mx-auto flex min-h-svh max-w-2xl flex-col gap-4 p-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">My applications</h1>
        <Link
          to="/restaurants/apply"
          className={buttonVariants({})}
        >
          New application
        </Link>
      </div>

      {applications.isLoading ? (
        <PageLoader />
      ) : applications.isError ? (
        <p className="text-destructive">Could not load your applications.</p>
      ) : (applications.data?.length ?? 0) === 0 ? (
        <p className="text-muted-foreground">
          You have not applied yet. Submit an application to open a storefront.
        </p>
      ) : (
        applications.data!.map((application) => (
          <Card key={application.id}>
            <CardHeader>
              <CardTitle>{application.restaurantName}</CardTitle>
              <CardDescription>
                Submitted{" "}
                {new Date(application.createdAt).toLocaleString()} ·{" "}
                {application.status}
              </CardDescription>
            </CardHeader>
            <CardContent className="flex flex-col gap-1 text-sm">
              <p>{application.description}</p>
              {application.rejectionReason ? (
                <p className="text-destructive">
                  Rejection reason: {application.rejectionReason}
                </p>
              ) : null}
              {application.reviewedAt ? (
                <p className="text-muted-foreground">
                  Reviewed {new Date(application.reviewedAt).toLocaleString()}
                </p>
              ) : null}
            </CardContent>
          </Card>
        ))
      )}
    </main>
  )
}
