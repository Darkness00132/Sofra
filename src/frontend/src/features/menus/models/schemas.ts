import { z } from "zod"

/**
 * The menus feature's contracts: the owner's menu management — menu types
 * under platform categories and their food items (FR-13…FR-16).
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const menuItemResponseSchema = z.object({
  id: z.guid(),
  name: z.string(),
  description: z.string(),
  price: z.number(),
  availability: z.string(),
  isAvailable: z.boolean(),
  imageUrl: z.string().nullable(),
})

export const menuTypeResponseSchema = z.object({
  id: z.guid(),
  foodCategoryId: z.guid(),
  name: z.string(),
  description: z.string().nullable(),
  items: z.array(menuItemResponseSchema),
})

export const myMenuResponseSchema = z.object({
  restaurantId: z.guid(),
  menuTypes: z.array(menuTypeResponseSchema),
})

export const addMenuTypeRequestSchema = z.object({
  foodCategoryId: z.guid(),
  name: z.string(),
  description: z.string().nullable(),
})

export const updateMenuTypeRequestSchema = z.object({
  name: z.string(),
  description: z.string().nullable(),
})

export const addMenuItemRequestSchema = z.object({
  menuTypeId: z.guid(),
  name: z.string(),
  description: z.string(),
  price: z.number(),
  availability: z.string(),
})

export const updateMenuItemRequestSchema = z.object({
  name: z.string(),
  description: z.string(),
  price: z.number(),
})

export const setMenuItemAvailabilityRequestSchema = z.object({
  available: z.boolean(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type MenuItem = z.infer<typeof menuItemResponseSchema>
export type MenuType = z.infer<typeof menuTypeResponseSchema>
export type MyMenu = z.infer<typeof myMenuResponseSchema>
export type AddMenuTypeRequest = z.infer<typeof addMenuTypeRequestSchema>
export type UpdateMenuTypeRequest = z.infer<typeof updateMenuTypeRequestSchema>
export type AddMenuItemRequest = z.infer<typeof addMenuItemRequestSchema>
export type UpdateMenuItemRequest = z.infer<typeof updateMenuItemRequestSchema>
export type SetMenuItemAvailabilityRequest = z.infer<
  typeof setMenuItemAvailabilityRequestSchema
>
