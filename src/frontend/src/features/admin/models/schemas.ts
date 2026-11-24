import { z } from "zod"

/**
 * The admin feature's contracts: application review (FR-47, FR-48), restaurant
 * policing (FR-49), order oversight (FR-45, FR-51), accounts (FR-50) and the
 * platform dashboard.
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const adminApplicationSchema = z.object({
  id: z.guid(),
  restaurantName: z.string(),
  address: z.string(),
  phone: z.string(),
  description: z.string(),
  status: z.string(),
  rejectionReason: z.string().nullable(),
  createdAt: z.iso.datetime(),
  reviewedAt: z.iso.datetime().nullable(),
  applicantEmail: z.string(),
})

export const adminRestaurantSchema = z.object({
  id: z.guid(),
  name: z.string(),
  address: z.string(),
  phone: z.string(),
  description: z.string(),
  status: z.string(),
  acceptingOrders: z.boolean(),
  ownerEmail: z.string(),
})

export const decideApplicationRequestSchema = z.object({
  status: z.string(),
  reason: z.string().nullable(),
})

export const updateRestaurantStatusRequestSchema = z.object({
  status: z.string(),
})

export const adminAccountSchema = z.object({
  id: z.guid(),
  email: z.string(),
  phoneNumber: z.string().nullable(),
  locked: z.boolean(),
  roles: z.array(z.string()),
})

export const adminDashboardSchema = z.object({
  totalRestaurants: z.number().int(),
  approvedRestaurants: z.number().int(),
  suspendedRestaurants: z.number().int(),
  pendingApplications: z.number().int(),
  totalCustomers: z.number().int(),
  totalOrders: z.number().int(),
  pendingOrders: z.number().int(),
  totalRevenue: z.number(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type AdminApplication = z.infer<typeof adminApplicationSchema>
export type AdminRestaurant = z.infer<typeof adminRestaurantSchema>
export type DecideApplicationRequest = z.infer<
  typeof decideApplicationRequestSchema
>
export type UpdateRestaurantStatusRequest = z.infer<
  typeof updateRestaurantStatusRequestSchema
>
export type AdminAccount = z.infer<typeof adminAccountSchema>
export type AdminDashboard = z.infer<typeof adminDashboardSchema>

/** Filter for the administrator's platform-wide order list. */
export interface AdminOrderFilter {
  restaurantId: string | null
  customerId: string | null
  status: string | null
  from: string | null
  to: string | null
  pageNumber: number
  pageSize: number
}
