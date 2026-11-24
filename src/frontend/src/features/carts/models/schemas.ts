import { z } from "zod"

/**
 * The cart feature's contracts: the signed-in customer's cart — one
 * restaurant's items with prices and subtotal (FR-19…FR-24).
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const cartItemResponseSchema = z.object({
  foodItemId: z.guid(),
  foodItemName: z.string(),
  unitPrice: z.number(),
  quantity: z.number().int(),
  lineTotal: z.number(),
})

export const cartResponseSchema = z.object({
  restaurantId: z.guid().nullable(),
  restaurantName: z.string().nullable(),
  items: z.array(cartItemResponseSchema),
  subtotal: z.number(),
})

export const addCartItemRequestSchema = z.object({
  foodItemId: z.guid(),
  quantity: z.number().int(),
})

export const updateCartItemRequestSchema = z.object({
  quantity: z.number().int(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type CartItem = z.infer<typeof cartItemResponseSchema>
export type Cart = z.infer<typeof cartResponseSchema>
export type AddCartItemRequest = z.infer<typeof addCartItemRequestSchema>
export type UpdateCartItemRequest = z.infer<typeof updateCartItemRequestSchema>
