import { z } from "zod"

/**
 * The restaurant-orders feature's contracts: the owner's incoming orders,
 * their processing and the sales dashboard (FR-32…FR-38, FR-44).
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const updateOrderStatusRequestSchema = z.object({
  status: z.string(),
  reason: z.string().nullable(),
})

export const ownerDashboardResponseSchema = z.object({
  restaurantId: z.guid(),
  restaurantName: z.string(),
  totalOrders: z.number().int(),
  pendingOrders: z.number().int(),
  completedOrders: z.number().int(),
  totalRevenue: z.number(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type UpdateOrderStatusRequest = z.infer<
  typeof updateOrderStatusRequestSchema
>
export type OwnerDashboard = z.infer<typeof ownerDashboardResponseSchema>

/** Statuses the restaurant can move an order to. */
export const restaurantOrderStatuses = [
  "Accepted",
  "Rejected",
  "Preparing",
  "Ready",
  "Completed",
] as const
