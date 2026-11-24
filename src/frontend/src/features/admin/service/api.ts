import { z } from "zod"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import { orderDetailsResponseSchema, pagedOrderSummariesSchema, type OrderDetails, type OrderSummariesPage } from "@/features/orders"
import {
  adminAccountSchema,
  adminApplicationSchema,
  adminDashboardSchema,
  adminRestaurantSchema,
  type AdminAccount,
  type AdminApplication,
  type AdminDashboard,
  type AdminRestaurant,
  type DecideApplicationRequest,
  type UpdateRestaurantStatusRequest,
} from "../models/schemas"
import type { AdminOrderFilter } from "../models/schemas"

/**
 * The admin feature's data module: application review (FR-47, FR-48),
 * restaurant policing (FR-49), order oversight (FR-45, FR-51), accounts
 * (FR-50) and the platform dashboard.
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const adminQueryKeys = {
  applications: (status: string | null, search: string | null) =>
    ["admin", "applications", status, search] as const,
  restaurants: (status: string | null, search: string | null) =>
    ["admin", "restaurants", status, search] as const,
  orders: (filter: AdminOrderFilter) => ["admin", "orders", filter] as const,
  orderDetails: (orderId: string) => ["admin", "orders", orderId] as const,
  accounts: (role: string | null, search: string | null) =>
    ["admin", "accounts", role, search] as const,
  dashboard: () => ["admin", "dashboard"] as const,
  payments: (status: string | null, method: string | null) =>
    ["admin", "payments", status, method] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function fetchApplications(
  status: string | null,
  search: string | null
): Promise<AdminApplication[]> {
  const params = new URLSearchParams()
  if (status) params.set("status", status)
  if (search) params.set("search", search)
  const response = await api.get(
    `/admin/restaurant-applications?${params.toString()}`
  )
  return z.array(adminApplicationSchema).parse(response.data.items)
}

export async function decideApplication(
  applicationId: string,
  request: DecideApplicationRequest
): Promise<AdminApplication> {
  const response = await api.post(
    `/admin/restaurant-applications/${applicationId}/status`,
    request
  )
  return adminApplicationSchema.parse(response.data)
}

export async function fetchAdminRestaurants(
  status: string | null,
  search: string | null
): Promise<AdminRestaurant[]> {
  const params = new URLSearchParams()
  if (status) params.set("status", status)
  if (search) params.set("search", search)
  const response = await api.get(`/admin/restaurants?${params.toString()}`)
  return z.array(adminRestaurantSchema).parse(response.data.items)
}

export async function updateRestaurantStatus(
  restaurantId: string,
  request: UpdateRestaurantStatusRequest
): Promise<AdminRestaurant> {
  const response = await api.post(
    `/admin/restaurants/${restaurantId}/status`,
    request
  )
  return adminRestaurantSchema.parse(response.data)
}

export async function fetchAdminOrders(
  filter: AdminOrderFilter
): Promise<OrderSummariesPage> {
  const params = new URLSearchParams()
  if (filter.restaurantId) params.set("restaurantId", filter.restaurantId)
  if (filter.customerId) params.set("customerId", filter.customerId)
  if (filter.status) params.set("status", filter.status)
  if (filter.from) params.set("from", filter.from)
  if (filter.to) params.set("to", filter.to)
  params.set("pageNumber", String(filter.pageNumber))
  params.set("pageSize", String(filter.pageSize))

  const response = await api.get(`/admin/orders?${params.toString()}`)
  return pagedOrderSummariesSchema.parse(response.data)
}

export async function fetchAdminOrderDetails(
  orderId: string
): Promise<OrderDetails> {
  const response = await api.get(`/admin/orders/${orderId}`)
  return orderDetailsResponseSchema.parse(response.data)
}

export async function fetchAccounts(
  role: string | null,
  search: string | null
): Promise<AdminAccount[]> {
  const params = new URLSearchParams()
  if (role) params.set("role", role)
  if (search) params.set("search", search)
  const response = await api.get(`/admin/accounts?${params.toString()}`)
  return z.array(adminAccountSchema).parse(response.data.items)
}

export async function setAccountLock(
  accountId: string,
  locked: boolean
): Promise<void> {
  await api.patch(`/admin/accounts/${accountId}/lock`, { locked })
}

export async function fetchPlatformDashboard(): Promise<AdminDashboard> {
  const response = await api.get("/admin/dashboard")
  return adminDashboardSchema.parse(response.data)
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** Admin applications list (status + search filters). Disabled when signed out. */
export function useApplicationsQuery(status: string | null, search: string | null) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: adminQueryKeys.applications(status, search),
    queryFn: () => fetchApplications(status, search),
    enabled: authenticated,
  })
}

/** Admin restaurants list (status + search filters). */
export function useAdminRestaurantsQuery(
  status: string | null,
  search: string | null
) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: adminQueryKeys.restaurants(status, search),
    queryFn: () => fetchAdminRestaurants(status, search),
    enabled: authenticated,
  })
}

/** Admin platform-wide orders list. */
export function useAdminOrdersQuery(filter: AdminOrderFilter) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: adminQueryKeys.orders(filter),
    queryFn: () => fetchAdminOrders(filter),
    enabled: authenticated,
  })
}

/** Admin accounts list (role filter + search). */
export function useAccountsQuery(role: string | null, search: string | null) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: adminQueryKeys.accounts(role, search),
    queryFn: () => fetchAccounts(role, search),
    enabled: authenticated,
  })
}

/** One order's full details for administrator oversight. */
export function useAdminOrderDetails(orderId: string) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: adminQueryKeys.orderDetails(orderId),
    queryFn: () => fetchAdminOrderDetails(orderId),
    enabled: authenticated && orderId !== "",
  })
}

/** Platform dashboard statistics. */
export function useAdminDashboardQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: adminQueryKeys.dashboard(),
    queryFn: fetchPlatformDashboard,
    enabled: authenticated,
  })
}

// ─── Mutations ───────────────────────────────────────────────────────────────

/** Decide an application: approve or reject with a reason (FR-48). */
export function useDecideApplicationMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      applicationId,
      request,
    }: {
      applicationId: string
      request: DecideApplicationRequest
    }) => decideApplication(applicationId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "applications"] })
      queryClient.invalidateQueries({ queryKey: ["admin", "restaurants"] })
      queryClient.invalidateQueries({ queryKey: adminQueryKeys.dashboard() })
    },
  })
}

/** Suspend or reinstate a restaurant (FR-49, BR-09). */
export function useUpdateRestaurantStatusMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      restaurantId,
      request,
    }: {
      restaurantId: string
      request: UpdateRestaurantStatusRequest
    }) => updateRestaurantStatus(restaurantId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "restaurants"] })
      queryClient.invalidateQueries({ queryKey: adminQueryKeys.dashboard() })
    },
  })
}

/** Lock or unlock a user account (FR-50). */
export function useSetAccountLockMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      accountId,
      locked,
    }: {
      accountId: string
      locked: boolean
    }) => setAccountLock(accountId, locked),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin", "accounts"] })
    },
  })
}

// ─── Payments oversight (read models) ────────────────────────────────────────

const adminPaymentSchema = z.object({
  id: z.guid(),
  orderId: z.guid(),
  amount: z.number(),
  paymentMethod: z.string(),
  status: z.string(),
  paidAt: z.iso.datetime().nullable(),
})

const adminPaymentPageSchema = z.object({
  items: z.array(adminPaymentSchema),
  pageNumber: z.number().int(),
  pageSize: z.number().int(),
  totalCount: z.number().int(),
})

export type AdminPayment = z.infer<typeof adminPaymentSchema>
export type AdminPaymentPage = z.infer<typeof adminPaymentPageSchema>

export async function fetchAdminPayments(
  status: string | null,
  method: string | null
): Promise<AdminPaymentPage> {
  const params = new URLSearchParams()
  if (status) params.set("status", status)
  if (method) params.set("method", method)
  params.set("pageNumber", "1")
  params.set("pageSize", "50")

  const response = await api.get(`/admin/payments?${params.toString()}`)
  return adminPaymentPageSchema.parse(response.data)
}

/** Admin payments list (status + method filters). */
export function useAdminPaymentsQuery(status: string | null, method: string | null) {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: adminQueryKeys.payments(status, method),
    queryFn: () => fetchAdminPayments(status, method),
    enabled: authenticated,
  })
}

// payments filter state lives in the page (plain client state)
export interface AdminPaymentFilter {
  status: string | null
  method: string | null
}
