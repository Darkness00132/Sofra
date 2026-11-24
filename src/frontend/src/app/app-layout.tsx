import { ChefHat, ClipboardList, Compass, ShoppingBag, Store, UserRound } from "lucide-react"
import { Link, NavLink, Outlet } from "react-router-dom"
import { useCartQuery } from "@/features/carts"
import { buttonVariants } from "@/shared/components/ui/button"
import { cn } from "@/shared/lib/utils"
import {
  selectIsAuthenticated,
  useAuthStore,
} from "@/shared/stores/auth.store"
import { ProfileSync } from "./profile-sync"
import { SignOutButton } from "./sign-out-button"

const linkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    "flex items-center gap-2 rounded-full px-3 py-1.5 text-sm transition-colors",
    isActive
      ? "bg-primary text-primary-foreground shadow-sm"
      : "text-foreground/80 hover:bg-accent"
  )

/**
 * Customer marketplace shell. Restaurant owners and administrators use their
 * own layouts, so this chrome only offers shopping.
 */
export function AppLayout() {
  const authenticated = useAuthStore(selectIsAuthenticated)
  const user = useAuthStore((state) => state.user)
  const cart = useCartQuery()
  const itemCount = cart.data?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0

  return (
    <div className="theme-market flex min-h-svh flex-col bg-background text-foreground">
      <header className="sticky top-0 z-30 border-b border-border/80 bg-background/90 backdrop-blur">
        <div className="mx-auto flex h-16 max-w-6xl items-center gap-3 px-4">
          <Link to="/restaurants" className="flex items-center gap-2 font-semibold">
            <span className="flex size-9 items-center justify-center rounded-2xl bg-primary text-primary-foreground shadow-sm">
              <ChefHat className="size-4" />
            </span>
            <span className="leading-tight">
              <span className="block text-lg tracking-tight">Sofra</span>
              <span className="hidden text-[10px] font-medium tracking-[0.18em] text-primary uppercase sm:block">
                Marketplace
              </span>
            </span>
          </Link>

          <nav className="ml-4 hidden items-center gap-1 md:flex">
            <NavLink to="/restaurants" end className={linkClass}>
              <Compass className="size-4" />
              Explore
            </NavLink>
            {authenticated ? (
              <>
                <NavLink to="/orders" className={linkClass}>
                  <ClipboardList className="size-4" />
                  Orders
                </NavLink>
                <NavLink to="/restaurants/my-applications" className={linkClass}>
                  <Store className="size-4" />
                  Open a restaurant
                </NavLink>
              </>
            ) : null}
          </nav>

          <div className="ml-auto flex items-center gap-2">
            {authenticated ? (
              <>
                <NavLink
                  to="/cart"
                  className={buttonVariants({ variant: "outline", size: "sm" })}
                >
                  <ShoppingBag />
                  Cart
                  {itemCount > 0 ? (
                    <span className="rounded-full bg-primary px-1.5 text-[11px] text-primary-foreground">
                      {itemCount}
                    </span>
                  ) : null}
                </NavLink>
                <NavLink
                  to="/profile"
                  className={buttonVariants({ variant: "ghost", size: "sm" })}
                >
                  <UserRound />
                  <span className="hidden max-w-36 truncate sm:inline">
                    {user?.email}
                  </span>
                </NavLink>
                <SignOutButton className="hidden sm:inline-flex" />
              </>
            ) : (
              <>
                <Link to="/login" className={buttonVariants({ variant: "ghost", size: "sm" })}>
                  Sign in
                </Link>
                <Link to="/register" className={buttonVariants({ size: "sm" })}>
                  Create account
                </Link>
              </>
            )}
          </div>
        </div>
      </header>

      <ProfileSync />
      <div className="flex-1">
        <Outlet />
      </div>

      <footer className="hidden border-t bg-card/70 md:block">
        <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-5 text-sm text-muted-foreground">
          <p className="flex items-center gap-2">
            <ChefHat className="size-4 text-primary" />
            Sofra marketplace — order from approved kitchens.
          </p>
          <p>Customers shop. Owners cook. Admins keep the market honest.</p>
        </div>
      </footer>

      {authenticated ? (
        <nav className="sticky bottom-0 z-30 grid grid-cols-4 border-t bg-background/95 backdrop-blur md:hidden">
          <NavLink to="/restaurants" end className={mobileLink}>
            <Compass className="size-4" />
            Explore
          </NavLink>
          <NavLink to="/cart" className={mobileLink}>
            <ShoppingBag className="size-4" />
            Cart
          </NavLink>
          <NavLink to="/orders" className={mobileLink}>
            <ClipboardList className="size-4" />
            Orders
          </NavLink>
          <NavLink to="/profile" className={mobileLink}>
            <UserRound className="size-4" />
            Profile
          </NavLink>
        </nav>
      ) : null}
    </div>
  )
}

function mobileLink({ isActive }: { isActive: boolean }) {
  return cn(
    "flex flex-col items-center gap-1 py-2 text-[11px]",
    isActive ? "text-primary" : "text-muted-foreground"
  )
}
