import { useState } from "react"
import { FileCheck2 } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { Input } from "@/shared/components/ui/input"
import { useApplicationsQuery, useDecideApplicationMutation } from "../service/api"

/**
 * Administrator review of restaurant applications (FR-47, FR-48): approve
 * materializes the restaurant and grants the owner the Restaurant role;
 * rejection records a reason.
 */
export function ApplicationsReviewPage() {
  const [status, setStatus] = useState<string | null>("PendingApproval")
  const [search, setSearch] = useState("")
  const applications = useApplicationsQuery(status, search)
  const decide = useDecideApplicationMutation()

  return (
    <div className="mx-auto flex w-full max-w-3xl flex-col gap-4">
      <SurfaceHeader
        icon={FileCheck2}
        kicker="Administrator"
        title="Applications"
        description="Approve a kitchen onto the marketplace, or reject it with a reason."
      />

      <div className="flex flex-col gap-2">
        <select
          className="rounded-md border bg-background px-3 py-2 text-sm"
          value={status ?? ""}
          onChange={(event) => setStatus(event.target.value || null)}
          aria-label="Filter by status"
        >
          <option value="">All statuses</option>
          <option value="PendingApproval">PendingApproval</option>
          <option value="Approved">Approved</option>
          <option value="Rejected">Rejected</option>
        </select>
        <Input
          placeholder="Search by restaurant name…"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
      </div>

      {applications.isLoading ? (
        <PageLoader />
      ) : applications.isError ? (
        <p className="text-destructive">Could not load applications.</p>
      ) : (applications.data?.length ?? 0) === 0 ? (
        <p className="text-muted-foreground">No applications in this view.</p>
      ) : (
        applications.data!.map((application) => (
          <Card key={application.id}>
            <CardHeader>
              <CardTitle>{application.restaurantName}</CardTitle>
              <CardDescription>
                {application.applicantEmail} · {application.status} ·{" "}
                {new Date(application.createdAt).toLocaleString()}
              </CardDescription>
            </CardHeader>
            <CardContent className="flex flex-col gap-2">
              <p className="text-sm">{application.description}</p>
              <p className="text-sm text-muted-foreground">
                {application.address} · {application.phone}
              </p>
              {application.status === "PendingApproval" ? (
                <div className="flex gap-2">
                  <Button
                    disabled={decide.isPending}
                    onClick={() =>
                      decide.mutate({
                        applicationId: application.id,
                        request: { status: "Approved", reason: null },
                      })
                    }
                  >
                    Approve
                  </Button>
                  <Button
                    variant="destructive"
                    disabled={decide.isPending}
                    onClick={() =>
                      decide.mutate({
                        applicationId: application.id,
                        request: {
                          status: "Rejected",
                          reason: "Rejected by administrator",
                        },
                      })
                    }
                  >
                    Reject
                  </Button>
                </div>
              ) : null}
            </CardContent>
          </Card>
        ))
      )}
    </div>
  )
}
