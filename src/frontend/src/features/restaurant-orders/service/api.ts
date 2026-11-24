import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import {
  orderDetailsResponseSchema,
  pagedOrderSummariesSchema,
  type OrderDetails,
  type OrderSummariesPage,
} from "@/features/orders"
import {
  ownerDashboardResponseSchema,
  type OwnerDashboard,
  type UpdateOrderStatusRequest,
} from "../models/schemas"

/**
 * The restaurant-orders feature's data module: the owner's incoming orders,
 * their processing and the sales dashboard (FR-32…FR-38, FR-44).
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const restaurantOrderQueryKeys = {
  restaurantOrders: (status: string | null) =>
    ["restaurant-orders", status] as const,
  restaurantOrder: (orderId: string) =>
    ["restaurant-orders", orderId] as const,
  dashboard: () => ["restaurant-orders", "dashboard"] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function fetchRestaurantOrders(
  status: string | null
): Promise<OrderSummariesPage> {
  const query = status ? `?status=${encodeURIComponent(status)}` : ""
  const response = await api.get(`/restaurants/orders${query}`)
  return pagedOrderSummariesSchema.parse(response.data)
}

export async function fetchRestaurantOrder(
  orderId: string
): Promise<OrderDetails> {
  const response = await api.get(`/restaurants/orders/${orderId}`)
  return orderDetailsResponseSchema.parse(response.data)
}

export async function updateOrderStatus(
  orderId: string,
  request: UpdateOrderStatusRequest
): Promise<OrderDetails> {
  const response = await api.post(`/restaurants/orders/${orderId}/status`, request)
  return orderDetailsResponseSchema.parse(response.data)
}

export async function fetchOwnerDashboard(): Promise<OwnerDashboard> {
  const response = await api.get("/restaurants/dashboard")
  return ownerDashboardResponseSchema.parse(response.data)
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** The owner's restaurant orders (status filter covers incoming + history). */
export function useRestaurantOrdersQuery(status: string | null) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: restaurantOrderQueryKeys.restaurantOrders(status),
    queryFn: () => fetchRestaurantOrders(status),
    enabled: authenticated,
  })
}

/** One restaurant order's full details. */
export function useRestaurantOrderQuery(orderId: string) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: restaurantOrderQueryKeys.restaurantOrder(orderId),
    queryFn: () => fetchRestaurantOrder(orderId),
    enabled: authenticated && orderId !== "",
  })
}

/** The owner's sales dashboard (basic sales information). */
export function useOwnerDashboardQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: restaurantOrderQueryKeys.dashboard(),
    queryFn: fetchOwnerDashboard,
    enabled: authenticated,
  })
}

// ─── Mutations ───────────────────────────────────────────────────────────────

/**
 * Move an order to its next status (FR-33…FR-35, BR-06, BR-08). Invalidates
 * the order lists, the details and the dashboard.
 */
export function useUpdateOrderStatusMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      orderId,
      request,
    }: {
      orderId: string
      request: UpdateOrderStatusRequest
    }) => updateOrderStatus(orderId, request),
    onSuccess: (details) => {
      queryClient.invalidateQueries({ queryKey: ["restaurant-orders"] })
      queryClient.setQueryData(
        restaurantOrderQueryKeys.restaurantOrder(details.id),
        details
      )
    },
  })
}
