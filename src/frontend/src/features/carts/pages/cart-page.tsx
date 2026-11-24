import { Minus, Plus, ShoppingBag, Trash2 } from "lucide-react"
import { Link } from "react-router-dom"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { CART_HERO } from "@/shared/lib/food-image"
import { Button, buttonVariants } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useCartQuery } from "../service/api"
import {
  useClearCartMutation,
  useRemoveCartItemMutation,
  useUpdateCartItemMutation,
} from "../service/api"

/**
 * The customer's cart (FR-19…FR-24): one restaurant's items with prices and
 * subtotal, quantity changes, removal and the FR-22 clear flow.
 */
export function CartPage() {
  const cart = useCartQuery()
  const updateItem = useUpdateCartItemMutation()
  const removeItem = useRemoveCartItemMutation()
  const clear = useClearCartMutation()
  const busy = updateItem.isPending || removeItem.isPending || clear.isPending

  return (
    <main className="mx-auto flex w-full max-w-2xl flex-col gap-4 px-4 py-8">
      <SurfaceHeader
        icon={ShoppingBag}
        kicker="Customer"
        title="Your cart"
        description="One restaurant at a time. Clear the cart to order somewhere else."
        image={CART_HERO}
      />

      {cart.isLoading ? (
        <PageLoader />
      ) : cart.isError ? (
        <p className="text-destructive">Could not load your cart.</p>
      ) : !cart.data || cart.data.items.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col gap-3">
            <p className="text-muted-foreground">Your cart is empty.</p>
            <Link
              to="/restaurants"
              className={buttonVariants({ variant: "outline" })}
            >
              Browse restaurants
            </Link>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardHeader>
            <CardTitle>{cart.data.restaurantName}</CardTitle>
            <CardDescription>
              Everything here is from one restaurant.
            </CardDescription>
          </CardHeader>
          <CardContent className="flex flex-col gap-3">
            {cart.data.items.map((item) => (
              <div
                key={item.foodItemId}
                className="flex items-center justify-between gap-4 border-b pb-3 last:border-b-0"
              >
                <div>
                  <p className="font-medium">{item.foodItemName}</p>
                  <p className="text-sm text-muted-foreground">
                    {item.unitPrice} × {item.quantity} = {item.lineTotal}
                  </p>
                </div>
                <div className="flex items-center gap-2">
                  <Button
                    variant="outline"
                    disabled={busy}
                    onClick={() =>
                      updateItem.mutate({
                        foodItemId: item.foodItemId,
                        request: { quantity: item.quantity - 1 },
                      })
                    }
                  >
                    <Minus />
                  </Button>
                  <span className="min-w-6 text-center">{item.quantity}</span>
                  <Button
                    variant="outline"
                    disabled={busy}
                    onClick={() =>
                      updateItem.mutate({
                        foodItemId: item.foodItemId,
                        request: { quantity: item.quantity + 1 },
                      })
                    }
                  >
                    <Plus />
                  </Button>
                  <Button
                    variant="outline"
                    disabled={busy}
                    onClick={() => removeItem.mutate(item.foodItemId)}
                  >
                    <Trash2 />
                    Remove
                  </Button>
                </div>
              </div>
            ))}
            <p className="text-right font-medium">
              Subtotal: {cart.data.subtotal}
            </p>
          </CardContent>
          <CardFooter className="justify-between">
            <Button
              variant="outline"
              disabled={busy}
              onClick={() => clear.mutate()}
            >
              Clear cart
            </Button>
            <Link to="/checkout" className={buttonVariants({})}>
              Checkout
            </Link>
          </CardFooter>
        </Card>
      )}
    </main>
  )
}
