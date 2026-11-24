import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import {
  orderDetailsResponseSchema,
  orderPreviewResponseSchema,
  pagedOrderSummariesSchema,
  placeOrderResponseSchema,
  type CancelOrderRequest,
  type MyOrdersFilter,
  type OrderDetails,
  type OrderSummariesPage,
  type PlaceOrderRequest,
  type PlaceOrderResponse,
  type PreviewRequest,
  type OrderPreview,
} from "../models/schemas"

/**
 * The orders feature's data module: checkout preview (FR-25), placement
 * (FR-28…FR-31), tracking (FR-36) and cancellation (FR-39…FR-41).
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const orderQueryKeys = {
  myOrders: (filter: MyOrdersFilter) => ["orders", "my", filter] as const,
  orderDetails: (orderId: string) => ["orders", orderId] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function previewOrder(
  request: PreviewRequest
): Promise<OrderPreview> {
  const response = await api.post("/orders/preview", request)
  return orderPreviewResponseSchema.parse(response.data)
}

export async function placeOrder(
  request: PlaceOrderRequest
): Promise<PlaceOrderResponse> {
  const response = await api.post("/orders", request)
  return placeOrderResponseSchema.parse(response.data)
}

export async function fetchMyOrders(
  filter: MyOrdersFilter
): Promise<OrderSummariesPage> {
  const params = new URLSearchParams()
  if (filter.status) params.set("status", filter.status)
  if (filter.from) params.set("from", filter.from)
  if (filter.to) params.set("to", filter.to)
  params.set("pageNumber", String(filter.pageNumber))
  params.set("pageSize", String(filter.pageSize))

  const response = await api.get(`/orders?${params.toString()}`)
  return pagedOrderSummariesSchema.parse(response.data)
}

export async function fetchOrderDetails(orderId: string): Promise<OrderDetails> {
  const response = await api.get(`/orders/${orderId}`)
  return orderDetailsResponseSchema.parse(response.data)
}

export async function cancelOrder(
  orderId: string,
  request: CancelOrderRequest
): Promise<OrderDetails> {
  const response = await api.post(`/orders/${orderId}/cancel`, request)
  return orderDetailsResponseSchema.parse(response.data)
}

// ─── Mutations ───────────────────────────────────────────────────────────────

/** Place the order, then invalidate every order + cart view (FR-28, FR-31). */
export function usePlaceOrderMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: PlaceOrderRequest) => placeOrder(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] })
      queryClient.invalidateQueries({ queryKey: ["cart"] })
    },
  })
}

/** Cancel one of the customer's orders (FR-39…FR-41). */
export function useCancelOrderMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      orderId,
      request,
    }: {
      orderId: string
      request: CancelOrderRequest
    }) => cancelOrder(orderId, request),
    onSuccess: (details) => {
      queryClient.invalidateQueries({ queryKey: ["orders"] })
      queryClient.setQueryData(orderQueryKeys.orderDetails(details.id), details)
    },
  })
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** The signed-in customer's own orders with filters and paging (FR-43). */
export function useMyOrdersQuery(filter: MyOrdersFilter) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: orderQueryKeys.myOrders(filter),
    queryFn: () => fetchMyOrders(filter),
    enabled: authenticated,
  })
}

/** One order's full details with its status history (FR-36). */
export function useOrderDetailsQuery(orderId: string) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: orderQueryKeys.orderDetails(orderId),
    queryFn: () => fetchOrderDetails(orderId),
    enabled: authenticated && orderId !== "",
  })
}
