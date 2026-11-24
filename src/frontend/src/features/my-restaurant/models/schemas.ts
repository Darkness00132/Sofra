import { z } from "zod"

/**
 * The my-restaurant feature's contracts: the owner's storefront management —
 * info (FR-12), order acceptance (FR-17) and the storefront picture.
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const myRestaurantResponseSchema = z.object({
  id: z.guid(),
  name: z.string(),
  address: z.string(),
  phone: z.string(),
  description: z.string(),
  status: z.string(),
  acceptingOrders: z.boolean(),
  imageUrl: z.string().nullable(),
})

export const updateMyRestaurantRequestSchema = z.object({
  name: z.string(),
  address: z.string(),
  phone: z.string(),
  description: z.string(),
})

export const setAcceptingOrdersRequestSchema = z.object({
  acceptingOrders: z.boolean(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type MyRestaurant = z.infer<typeof myRestaurantResponseSchema>
export type UpdateMyRestaurantRequest = z.infer<
  typeof updateMyRestaurantRequestSchema
>
export type SetAcceptingOrdersRequest = z.infer<
  typeof setAcceptingOrdersRequestSchema
>

// ─── Form schema (stricter than the wire) ────────────────────────────────────

export const myRestaurantFormSchema = z.object({
  name: z.string().min(1, "Restaurant name is required."),
  address: z.string().min(1, "Address is required."),
  phone: z.string().min(1, "Phone is required."),
  description: z.string().min(1, "Description is required."),
})
