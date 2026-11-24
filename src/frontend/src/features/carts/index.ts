/**
 * Public surface of the carts feature — other features (e.g. discovery's
 * add-to-cart buttons) may import only from this file.
 */
export { useCartQuery, useAddCartItemMutation } from "./service/api"
export type { Cart, CartItem } from "./models/schemas"
