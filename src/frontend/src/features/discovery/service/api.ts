import { useQuery } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import {
  pagedFoodCategoriesSchema,
  pagedPublicRestaurantsSchema,
  restaurantMenuSchema,
  publicRestaurantSchema,
  type FoodCategoriesPage,
  type PublicRestaurant,
  type PublicRestaurantsPage,
  type RestaurantBrowserFilter,
  type RestaurantMenu,
} from "../models/schemas"

/**
 * The discovery feature's data module: the public, unauthenticated browsing of
 * approved restaurants, their menus and the category catalog that powers the
 * discovery filters.
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const discoveryQueryKeys = {
  restaurants: (filter: RestaurantBrowserFilter) =>
    ["discovery", "restaurants", filter] as const,
  restaurantDetails: (restaurantId: string) =>
    ["discovery", "restaurants", restaurantId] as const,
  restaurantMenu: (restaurantId: string) =>
    ["discovery", "restaurants", restaurantId, "menu"] as const,
  foodCategories: (search: string | null) =>
    ["discovery", "food-categories", search] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

function toQueryString(
  params: Record<string, string | number | boolean | null | undefined>
): string {
  const search = new URLSearchParams()
  for (const [key, value] of Object.entries(params)) {
    if (value !== null && value !== undefined && value !== "") {
      search.set(key, String(value))
    }
  }
  const query = search.toString()
  return query ? `?${query}` : ""
}

export async function fetchRestaurants(
  filter: RestaurantBrowserFilter
): Promise<PublicRestaurantsPage> {
  const response = await api.get(
    "/restaurants" +
      toQueryString({
        search: filter.search,
        categoryId: filter.categoryId,
        acceptingOrders: filter.acceptingOrders,
        pageNumber: filter.pageNumber,
        pageSize: filter.pageSize,
      })
  )
  return pagedPublicRestaurantsSchema.parse(response.data)
}

export async function fetchFoodCategories(
  search: string | null
): Promise<FoodCategoriesPage> {
  const response = await api.get("/food-categories" + toQueryString({ search }))
  return pagedFoodCategoriesSchema.parse(response.data)
}

export async function fetchRestaurantDetails(
  restaurantId: string
): Promise<PublicRestaurant> {
  const response = await api.get(`/restaurants/${restaurantId}`)
  return publicRestaurantSchema.parse(response.data)
}

export async function fetchRestaurantMenu(
  restaurantId: string
): Promise<RestaurantMenu> {
  const response = await api.get(`/restaurants/${restaurantId}/menu`)
  return restaurantMenuSchema.parse(response.data)
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** Public restaurant browsing — enabled for everyone, no auth guard. */
export function useRestaurantsQuery(filter: RestaurantBrowserFilter) {
  return useQuery({
    queryKey: discoveryQueryKeys.restaurants(filter),
    queryFn: () => fetchRestaurants(filter),
  })
}

export function useFoodCategoriesQuery(search: string | null) {
  return useQuery({
    queryKey: discoveryQueryKeys.foodCategories(search),
    queryFn: () => fetchFoodCategories(search),
  })
}

export function useRestaurantDetailsQuery(restaurantId: string) {
  return useQuery({
    queryKey: discoveryQueryKeys.restaurantDetails(restaurantId),
    queryFn: () => fetchRestaurantDetails(restaurantId),
    enabled: restaurantId !== "",
  })
}

export function useRestaurantMenuQuery(restaurantId: string) {
  return useQuery({
    queryKey: discoveryQueryKeys.restaurantMenu(restaurantId),
    queryFn: () => fetchRestaurantMenu(restaurantId),
    enabled: restaurantId !== "",
  })
}
