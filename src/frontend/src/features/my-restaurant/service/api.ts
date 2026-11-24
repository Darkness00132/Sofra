import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import { myRestaurantResponseSchema } from "../models/schemas"
import type {
  MyRestaurant,
  SetAcceptingOrdersRequest,
  UpdateMyRestaurantRequest,
} from "../models/schemas"

/**
 * The my-restaurant feature's data module: the owner's storefront (FR-12),
 * order acceptance (FR-17) and the storefront picture.
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const myRestaurantQueryKeys = {
  myRestaurant: () => ["my-restaurant"] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function fetchMyRestaurant(): Promise<MyRestaurant> {
  const response = await api.get("/restaurants/me")
  return myRestaurantResponseSchema.parse(response.data)
}

export async function updateMyRestaurant(
  request: UpdateMyRestaurantRequest
): Promise<MyRestaurant> {
  const response = await api.put("/restaurants/me", request)
  return myRestaurantResponseSchema.parse(response.data)
}

export async function setAcceptingOrders(
  request: SetAcceptingOrdersRequest
): Promise<MyRestaurant> {
  const response = await api.patch("/restaurants/me", request)
  return myRestaurantResponseSchema.parse(response.data)
}

export async function uploadRestaurantImage(image: File): Promise<MyRestaurant> {
  const data = new FormData()
  data.append("Image", image)
  const response = await api.post(`/restaurants/me/image`, data)
  return myRestaurantResponseSchema.parse(response.data)
}

export async function removeRestaurantImage(): Promise<MyRestaurant> {
  const response = await api.delete(`/restaurants/me/image`)
  return myRestaurantResponseSchema.parse(response.data)
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** The owner's restaurant. Disabled while signed out (FR-12). */
export function useMyRestaurantQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: myRestaurantQueryKeys.myRestaurant(),
    queryFn: fetchMyRestaurant,
    enabled: authenticated,
  })
}

// ─── Mutations ───────────────────────────────────────────────────────────────

export function useUpdateMyRestaurantMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: UpdateMyRestaurantRequest) =>
      updateMyRestaurant(request),
    onSuccess: (restaurant) => {
      queryClient.setQueryData(myRestaurantQueryKeys.myRestaurant(), restaurant)
    },
  })
}

export function useSetAcceptingOrdersMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: SetAcceptingOrdersRequest) =>
      setAcceptingOrders(request),
    onSuccess: (restaurant) => {
      queryClient.setQueryData(myRestaurantQueryKeys.myRestaurant(), restaurant)
    },
  })
}

export function useUploadRestaurantImageMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ image }: { image: File }) => uploadRestaurantImage(image),
    onSuccess: (restaurant) => {
      queryClient.setQueryData(myRestaurantQueryKeys.myRestaurant(), restaurant)
    },
  })
}

export function useRemoveRestaurantImageMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: () => removeRestaurantImage(),
    onSuccess: (restaurant) => {
      queryClient.setQueryData(myRestaurantQueryKeys.myRestaurant(), restaurant)
    },
  })
}
