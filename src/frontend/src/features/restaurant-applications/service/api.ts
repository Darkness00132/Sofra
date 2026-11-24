import { z } from "zod"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import {
  applicationResponseSchema,
  type ApplicationResponse,
  type ApplyApplicationRequest,
} from "../models/schemas"

/**
 * The restaurant-application feature's data module: submitting an application
 * for review and tracking its status (FR-11, FR-47).
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const applicationQueryKeys = {
  myApplications: () => ["restaurant-applications", "my"] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function submitRestaurantApplication(
  request: ApplyApplicationRequest
): Promise<ApplicationResponse> {
  const response = await api.post("/restaurants/apply", request)
  return applicationResponseSchema.parse(response.data)
}

export async function fetchMyApplications(): Promise<ApplicationResponse[]> {
  const response = await api.get("/restaurants/my-applications")
  return z.array(applicationResponseSchema).parse(response.data)
}

// ─── Mutations ───────────────────────────────────────────────────────────────

/** Submit a restaurant application for administrator review (FR-11). */
export function useApplyMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (request: ApplyApplicationRequest) =>
      submitRestaurantApplication(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: applicationQueryKeys.myApplications() })
    },
  })
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** The signed-in user's own applications, newest first (FR-47 tracking). */
export function useMyApplicationsQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: applicationQueryKeys.myApplications(),
    queryFn: fetchMyApplications,
    enabled: authenticated,
  })
}
