import { LoaderCircle } from "lucide-react"

/** Full-viewport loading placeholder for route-level suspense and queries. */
export function PageLoader() {
  return (
    <div
      className="flex min-h-svh items-center justify-center p-4"
      role="status"
      aria-label="Loading"
    >
      <LoaderCircle className="size-6 animate-spin text-muted-foreground" />
    </div>
  )
}
