import { z } from "zod"

/**
 * The orders feature's contracts: checkout (preview + placement), the
 * customer's order tracking and cancellation (FR-25…FR-42).
 * Backend enums serialize as strings.
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const previewRequestSchema = z.object({
  fulfilmentType: z.string(),
  deliveryAddress: z.string().nullable(),
})

export const orderPreviewResponseSchema = z.object({
  restaurantId: z.guid(),
  restaurantName: z.string(),
  subtotal: z.number(),
  tax: z.number(),
  deliveryFee: z.number(),
  total: z.number(),
})

export const placeOrderRequestSchema = z.object({
  fulfilmentType: z.string(),
  paymentMethod: z.string(),
  deliveryAddress: z.string().nullable(),
})

export const placeOrderResponseSchema = z.object({
  id: z.guid(),
  status: z.string(),
  fulfilmentType: z.string(),
  paymentMethod: z.string(),
  subtotal: z.number(),
  tax: z.number(),
  deliveryFee: z.number(),
  total: z.number(),
  deliveryAddress: z.string().nullable(),
  createdAt: z.iso.datetime(),
})

export const orderItemSchema = z.object({
  foodItemId: z.guid(),
  foodItemName: z.string(),
  unitPrice: z.number(),
  quantity: z.number().int(),
  lineTotal: z.number(),
})

export const orderStatusHistorySchema = z.object({
  status: z.string(),
  changedByUserId: z.guid().nullable(),
  reason: z.string().nullable(),
  changedAt: z.iso.datetime(),
})

export const orderDetailsResponseSchema = z.object({
  id: z.guid(),
  customerId: z.guid(),
  restaurantId: z.guid(),
  restaurantName: z.string(),
  status: z.string(),
  fulfilmentType: z.string(),
  deliveryStatus: z.string(),
  paymentMethod: z.string(),
  paymentStatus: z.string().nullable(),
  subtotal: z.number(),
  tax: z.number(),
  deliveryFee: z.number(),
  discount: z.number(),
  total: z.number(),
  deliveryAddress: z.string().nullable(),
  createdAt: z.iso.datetime(),
  completedAt: z.iso.datetime().nullable(),
  items: z.array(orderItemSchema),
  statusHistory: z.array(orderStatusHistorySchema),
})

export const orderSummarySchema = z.object({
  id: z.guid(),
  restaurantId: z.guid(),
  restaurantName: z.string(),
  status: z.string(),
  fulfilmentType: z.string(),
  deliveryStatus: z.string(),
  paymentMethod: z.string(),
  total: z.number(),
  createdAt: z.iso.datetime(),
})

export const pagedOrderSummariesSchema = z.object({
  items: z.array(orderSummarySchema),
  pageNumber: z.number().int(),
  pageSize: z.number().int(),
  totalCount: z.number().int(),
})

export const cancelOrderRequestSchema = z.object({
  reason: z.string(),
  note: z.string().nullable(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type PreviewRequest = z.infer<typeof previewRequestSchema>
export type OrderPreview = z.infer<typeof orderPreviewResponseSchema>
export type PlaceOrderRequest = z.infer<typeof placeOrderRequestSchema>
export type PlaceOrderResponse = z.infer<typeof placeOrderResponseSchema>
export type OrderItem = z.infer<typeof orderItemSchema>
export type OrderStatusHistory = z.infer<typeof orderStatusHistorySchema>
export type OrderDetails = z.infer<typeof orderDetailsResponseSchema>
export type OrderSummary = z.infer<typeof orderSummarySchema>
export type OrderSummariesPage = z.infer<typeof pagedOrderSummariesSchema>
export type CancelOrderRequest = z.infer<typeof cancelOrderRequestSchema>

/** Client-side filter state for the customer's order list. */
export interface MyOrdersFilter {
  status: string | null
  from: string | null
  to: string | null
  pageNumber: number
  pageSize: number
}

// ─── Form schema (checkout) ──────────────────────────────────────────────────

export const checkoutFormSchema = z
  .object({
    fulfilmentType: z.enum(["Pickup", "Delivery"]),
    paymentMethod: z.enum(["CashOnDelivery", "OnlinePayment"]),
    deliveryAddress: z.string(),
  })
  .refine(
    (data) =>
      data.fulfilmentType === "Pickup" || data.deliveryAddress.trim() !== "",
    { path: ["deliveryAddress"], message: "Delivery address is required." }
  )
