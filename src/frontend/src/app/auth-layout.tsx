import { Link, Outlet } from "react-router-dom"
import { ChefHat } from "lucide-react"
import { AUTH_HERO } from "@/shared/lib/food-image"

/** Split sign-in screen: food photography on one side, the form on the other. */
export function AuthLayout() {
  return (
    <div className="theme-market grid min-h-svh bg-background lg:grid-cols-2">
      <section className="relative hidden overflow-hidden lg:block">
        <img
          src={AUTH_HERO}
          alt=""
          className="absolute inset-0 h-full w-full object-cover"
        />
        <div className="absolute inset-0 bg-linear-to-t from-black/75 via-black/25 to-black/10" />
        <div className="absolute bottom-0 left-0 max-w-md p-10 text-white">
          <p className="mb-3 flex items-center gap-2 text-sm font-medium tracking-wide text-white/80">
            <ChefHat className="size-4" />
            Sofra
          </p>
          <h1 className="text-4xl font-semibold tracking-tight">
            Dinner, from kitchens that earned their place.
          </h1>
          <p className="mt-3 text-sm text-white/80">
            Customers order. Restaurants cook. Administrators keep the market honest.
          </p>
        </div>
      </section>
      <section className="flex items-center justify-center px-4 py-10">
        <div className="w-full max-w-sm">
          <Link
            to="/restaurants"
            className="mb-6 flex items-center gap-2 text-lg font-semibold lg:hidden"
          >
            <span className="flex size-9 items-center justify-center rounded-xl bg-primary text-primary-foreground">
              <ChefHat className="size-4" />
            </span>
            Sofra
          </Link>
          <Outlet />
        </div>
      </section>
    </div>
  )
}
