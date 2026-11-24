import { ArrowLeft, MapPin, Phone, Plus, ShoppingBag } from "lucide-react"
import { Link, useParams } from "react-router-dom"
import { toast } from "sonner"
import { useAddCartItemMutation } from "@/features/carts"
import { PageLoader } from "@/shared/components/page-loader"
import { Badge } from "@/shared/components/ui/badge"
import { Button, buttonVariants } from "@/shared/components/ui/button"
import { coverImage, dishImage } from "@/shared/lib/food-image"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import { useRestaurantDetailsQuery, useRestaurantMenuQuery } from "../service/api"

/**
 * Public details of one approved restaurant plus its available menu (FR-07,
 * FR-10). Signed-in customers can add available items to the cart.
 */
export function RestaurantDetailsPage() {
  const { restaurantId = "" } = useParams()
  const details = useRestaurantDetailsQuery(restaurantId)
  const menu = useRestaurantMenuQuery(restaurantId)
  const authenticated = useAuthStore(selectIsAuthenticated)
  const addItem = useAddCartItemMutation()

  if (details.isLoading || menu.isLoading) {
    return (
      <main className="p-6">
        <PageLoader />
      </main>
    )
  }

  if (details.isError || menu.isError || !details.data || !menu.data) {
    return (
      <main className="mx-auto flex max-w-3xl flex-col gap-4 p-6">
        <p className="text-destructive">The restaurant was not found.</p>
        <Link to="/restaurants" className={buttonVariants({ variant: "outline", className: "w-fit" })}>
          Back to restaurants
        </Link>
      </main>
    )
  }

  const restaurant = details.data
  const canOrder = restaurant.acceptingOrders

  return (
    <main className="mx-auto flex max-w-5xl flex-col gap-6 px-4 py-6">
      <Link
        to="/restaurants"
        className={buttonVariants({ variant: "ghost", size: "sm", className: "w-fit" })}
      >
        <ArrowLeft />
        All restaurants
      </Link>

      <section className="overflow-hidden rounded-3xl border bg-card shadow-sm">
        <div className="relative h-56 md:h-72">
          <img
            src={coverImage(restaurant.imageUrl, restaurant.name)}
            alt=""
            className="h-full w-full object-cover"
          />
          <div className="absolute inset-0 bg-linear-to-t from-black/70 to-transparent" />
          <div className="absolute right-4 bottom-4 left-4 text-white">
            <Badge variant={canOrder ? "default" : "secondary"}>
              {canOrder ? "Accepting orders" : "Not accepting orders"}
            </Badge>
            <h1 className="mt-2 text-3xl font-semibold tracking-tight">{restaurant.name}</h1>
          </div>
        </div>
        <div className="grid gap-3 p-5 text-sm md:grid-cols-3">
          <p className="md:col-span-3 text-muted-foreground">{restaurant.description}</p>
          <p className="flex items-center gap-2">
            <MapPin className="size-4 text-primary" />
            {restaurant.address}
          </p>
          <p className="flex items-center gap-2">
            <Phone className="size-4 text-primary" />
            {restaurant.phone}
          </p>
        </div>
      </section>

      <div className="flex items-end justify-between gap-3">
        <h2 className="text-2xl font-semibold tracking-tight">Menu</h2>
        {authenticated ? (
          <Link to="/cart" className={buttonVariants({ variant: "outline", size: "sm" })}>
            <ShoppingBag />
            View cart
          </Link>
        ) : (
          <Link
            to={`/login?redirectTo=/restaurants/${restaurant.id}`}
            className={buttonVariants({ size: "sm" })}
          >
            Sign in to order
          </Link>
        )}
      </div>

      {menu.data.menuTypes.length === 0 ? (
        <p className="text-muted-foreground">
          This restaurant has no available items right now.
        </p>
      ) : (
        <div className="flex flex-col gap-8">
          {menu.data.menuTypes.map((menuType) => (
            <section key={menuType.id} className="flex flex-col gap-3">
              <div>
                <h3 className="text-lg font-semibold">{menuType.name}</h3>
                {menuType.description ? (
                  <p className="text-sm text-muted-foreground">{menuType.description}</p>
                ) : null}
              </div>
              <div className="grid gap-3">
                {menuType.items.map((item) => (
                  <article
                    key={item.id}
                    className="flex gap-4 rounded-2xl border bg-card p-3 shadow-sm"
                  >
                    <img
                      src={dishImage(item.imageUrl, item.name)}
                      alt=""
                      className="size-20 shrink-0 rounded-xl object-cover sm:size-24"
                    />
                    <div className="flex min-w-0 flex-1 flex-col gap-1">
                      <div className="flex items-start justify-between gap-3">
                        <p className="font-medium">{item.name}</p>
                        <p className="font-semibold tabular-nums">{item.price}</p>
                      </div>
                      <p className="line-clamp-2 text-sm text-muted-foreground">
                        {item.description}
                      </p>
                      <div className="mt-auto flex items-center justify-between gap-2 pt-2">
                        <Badge variant={item.isAvailable ? "secondary" : "outline"}>
                          {item.isAvailable ? "Available" : "Unavailable"}
                        </Badge>
                        {authenticated && canOrder && item.isAvailable ? (
                          <Button
                            size="sm"
                            disabled={addItem.isPending}
                            onClick={() =>
                              addItem.mutate(
                                { foodItemId: item.id, quantity: 1 },
                                {
                                  onSuccess: () => toast.success(`Added ${item.name}`),
                                  onError: () => toast.error("Could not add this item to the cart."),
                                }
                              )
                            }
                          >
                            <Plus />
                            Add
                          </Button>
                        ) : null}
                      </div>
                    </div>
                  </article>
                ))}
              </div>
            </section>
          ))}
        </div>
      )}
    </main>
  )
}
