import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import {
  menuItemResponseSchema,
  menuTypeResponseSchema,
  myMenuResponseSchema,
  type AddMenuItemRequest,
  type AddMenuTypeRequest,
  type MenuItem,
  type MenuType,
  type MyMenu,
  type SetMenuItemAvailabilityRequest,
  type UpdateMenuItemRequest,
  type UpdateMenuTypeRequest,
} from "../models/schemas"

/**
 * The menus feature's data module: the owner's menu management — menu types,
 * food items, availability and pictures (FR-13…FR-16).
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const menuQueryKeys = {
  myMenu: () => ["menus", "my-menu"] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function fetchMyMenu(): Promise<MyMenu> {
  const response = await api.get("/restaurants/me/menu")
  return myMenuResponseSchema.parse(response.data)
}

export async function addMenuType(
  request: AddMenuTypeRequest
): Promise<MenuType> {
  const response = await api.post("/restaurants/me/menu/types", request)
  return menuTypeResponseSchema.parse(response.data)
}

export async function updateMenuType(
  menuTypeId: string,
  request: UpdateMenuTypeRequest
): Promise<MenuType> {
  const response = await api.put(`/restaurants/me/menu/types/${menuTypeId}`, request)
  return menuTypeResponseSchema.parse(response.data)
}

export async function removeMenuType(menuTypeId: string): Promise<void> {
  await api.delete(`/restaurants/me/menu/types/${menuTypeId}`)
}

export async function addMenuItem(
  request: AddMenuItemRequest
): Promise<MenuItem> {
  const response = await api.post("/restaurants/me/menu/items", request)
  return menuItemResponseSchema.parse(response.data)
}

export async function updateMenuItem(
  foodItemId: string,
  request: UpdateMenuItemRequest
): Promise<MenuItem> {
  const response = await api.put(`/restaurants/me/menu/items/${foodItemId}`, request)
  return menuItemResponseSchema.parse(response.data)
}

export async function removeMenuItem(foodItemId: string): Promise<void> {
  await api.delete(`/restaurants/me/menu/items/${foodItemId}`)
}

export async function setMenuItemAvailability(
  foodItemId: string,
  request: SetMenuItemAvailabilityRequest
): Promise<MenuItem> {
  const response = await api.patch(
    `/restaurants/me/menu/items/${foodItemId}/availability`,
    request
  )
  return menuItemResponseSchema.parse(response.data)
}

export async function uploadMenuItemImage(
  foodItemId: string,
  image: File
): Promise<MenuItem> {
  const data = new FormData()
  data.append("Image", image)
  const response = await api.post(
    `/restaurants/me/menu/items/${foodItemId}/image`,
    data
  )
  return menuItemResponseSchema.parse(response.data)
}

export async function removeMenuItemImage(foodItemId: string): Promise<MenuItem> {
  const response = await api.delete(
    `/restaurants/me/menu/items/${foodItemId}/image`
  )
  return menuItemResponseSchema.parse(response.data)
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** The owner's full menu — includes unavailable items (management view). */
export function useMyMenuQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: menuQueryKeys.myMenu(),
    queryFn: fetchMyMenu,
    enabled: authenticated,
  })
}

// ─── Mutations ───────────────────────────────────────────────────────────────

export function useAddMenuTypeMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: AddMenuTypeRequest) => addMenuType(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useUpdateMenuTypeMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      menuTypeId,
      request,
    }: {
      menuTypeId: string
      request: UpdateMenuTypeRequest
    }) => updateMenuType(menuTypeId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useRemoveMenuTypeMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (menuTypeId: string) => removeMenuType(menuTypeId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useAddMenuItemMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: AddMenuItemRequest) => addMenuItem(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useUpdateMenuItemMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      foodItemId,
      request,
    }: {
      foodItemId: string
      request: UpdateMenuItemRequest
    }) => updateMenuItem(foodItemId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useRemoveMenuItemMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (foodItemId: string) => removeMenuItem(foodItemId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useSetMenuItemAvailabilityMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      foodItemId,
      request,
    }: {
      foodItemId: string
      request: SetMenuItemAvailabilityRequest
    }) => setMenuItemAvailability(foodItemId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useUploadMenuItemImageMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ foodItemId, image }: { foodItemId: string; image: File }) =>
      uploadMenuItemImage(foodItemId, image),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}

export function useRemoveMenuItemImageMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (foodItemId: string) => removeMenuItemImage(foodItemId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuQueryKeys.myMenu() })
    },
  })
}
