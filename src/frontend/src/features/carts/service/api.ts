import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import { cartResponseSchema } from "../models/schemas"
import type {
  AddCartItemRequest,
  Cart,
  UpdateCartItemRequest,
} from "../models/schemas"

/**
 * The cart feature's data module: the signed-in customer's cart — add items,
 * change quantities, remove, clear (FR-19…FR-24). The public restaurant menu
 * uses useAddCartItemMutation through this feature's index.
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const cartQueryKeys = {
  cart: () => ["cart"] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function fetchCart(): Promise<Cart> {
  const response = await api.get("/cart")
  return cartResponseSchema.parse(response.data)
}

export async function addCartItem(
  request: AddCartItemRequest
): Promise<Cart> {
  const response = await api.post("/cart/items", request)
  return cartResponseSchema.parse(response.data)
}

export async function updateCartItem(
  foodItemId: string,
  request: UpdateCartItemRequest
): Promise<Cart> {
  const response = await api.put(`/cart/items/${foodItemId}`, request)
  return cartResponseSchema.parse(response.data)
}

export async function removeCartItem(foodItemId: string): Promise<Cart> {
  const response = await api.delete(`/cart/items/${foodItemId}`)
  return cartResponseSchema.parse(response.data)
}

export async function clearCart(): Promise<Cart> {
  const response = await api.delete("/cart")
  return cartResponseSchema.parse(response.data)
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/** The signed-in customer's cart (FR-23). Disabled while signed out. */
export function useCartQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)

  return useQuery({
    queryKey: cartQueryKeys.cart(),
    queryFn: fetchCart,
    enabled: authenticated,
  })
}

// ─── Mutations ───────────────────────────────────────────────────────────────

export function useAddCartItemMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: AddCartItemRequest) => addCartItem(request),
    onSuccess: (cart) => {
      queryClient.setQueryData(cartQueryKeys.cart(), cart)
    },
  })
}

export function useUpdateCartItemMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      foodItemId,
      request,
    }: {
      foodItemId: string
      request: UpdateCartItemRequest
    }) => updateCartItem(foodItemId, request),
    onSuccess: (cart) => {
      queryClient.setQueryData(cartQueryKeys.cart(), cart)
    },
  })
}

export function useRemoveCartItemMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (foodItemId: string) => removeCartItem(foodItemId),
    onSuccess: (cart) => {
      queryClient.setQueryData(cartQueryKeys.cart(), cart)
    },
  })
}

export function useClearCartMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: () => clearCart(),
    onSuccess: (cart) => {
      queryClient.setQueryData(cartQueryKeys.cart(), cart)
    },
  })
}
