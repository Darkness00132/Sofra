import { z } from "zod"

/**
 * The discovery feature's contracts: the public, unauthenticated browsing of
 * approved restaurants and their menus. Backend enums serialize as strings.
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const publicRestaurantSchema = z.object({
  id: z.guid(),
  name: z.string(),
  address: z.string(),
  phone: z.string(),
  description: z.string(),
  acceptingOrders: z.boolean(),
  imageUrl: z.string().nullable(),
})

export const pagedPublicRestaurantsSchema = z.object({
  items: z.array(publicRestaurantSchema),
  pageNumber: z.number().int(),
  pageSize: z.number().int(),
  totalCount: z.number().int(),
})

export const menuItemSchema = z.object({
  id: z.guid(),
  name: z.string(),
  description: z.string(),
  price: z.number(),
  availability: z.string(),
  isAvailable: z.boolean(),
  imageUrl: z.string().nullable(),
})

export const menuTypeSchema = z.object({
  id: z.guid(),
  foodCategoryId: z.guid(),
  name: z.string(),
  description: z.string().nullable(),
  items: z.array(menuItemSchema),
})

export const restaurantMenuSchema = z.object({
  restaurantId: z.guid(),
  restaurantName: z.string(),
  menuTypes: z.array(menuTypeSchema),
})

export const foodCategorySchema = z.object({
  id: z.guid(),
  name: z.string(),
  description: z.string().nullable(),
})

export const pagedFoodCategoriesSchema = z.object({
  items: z.array(foodCategorySchema),
  pageNumber: z.number().int(),
  pageSize: z.number().int(),
  totalCount: z.number().int(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type PublicRestaurant = z.infer<typeof publicRestaurantSchema>
export type PublicRestaurantsPage = z.infer<typeof pagedPublicRestaurantsSchema>
export type RestaurantMenuItem = z.infer<typeof menuItemSchema>
export type RestaurantMenuType = z.infer<typeof menuTypeSchema>
export type RestaurantMenu = z.infer<typeof restaurantMenuSchema>
export type FoodCategory = z.infer<typeof foodCategorySchema>
export type FoodCategoriesPage = z.infer<typeof pagedFoodCategoriesSchema>

/** Client-side filter state for the discovery list (query-string bound). */
export interface RestaurantBrowserFilter {
  search: string
  categoryId: string | null
  acceptingOrders: boolean | null
  pageNumber: number
  pageSize: number
}
