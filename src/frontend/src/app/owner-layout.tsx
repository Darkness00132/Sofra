import {
  ChefHat,
  ClipboardList,
  LayoutDashboard,
  Store,
  UtensilsCrossed,
} from "lucide-react"
import type { LucideIcon } from "lucide-react"
import { NavLink, Outlet } from "react-router-dom"
import { KITCHEN_HERO } from "@/shared/lib/food-image"
import { cn } from "@/shared/lib/utils"
import { useAuthStore } from "@/shared/stores/auth.store"
import { ProfileSync } from "./profile-sync"
import { SignOutButton } from "./sign-out-button"

const NAV: { to: string; label: string; icon: LucideIcon }[] = [
  { to: "/restaurant/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { to: "/restaurants/me", label: "Storefront", icon: Store },
  { to: "/menus", label: "Menu", icon: UtensilsCrossed },
  { to: "/restaurant/orders", label: "Orders", icon: ClipboardList },
]

function navClass({ isActive }: { isActive: boolean }) {
  return cn(
    "flex items-center gap-2 rounded-xl px-3 py-2.5 text-sm",
    isActive
      ? "bg-sidebar-primary font-medium text-sidebar-primary-foreground"
      : "text-sidebar-foreground/80 hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
  )
}

/** Kitchen portal: espresso rail, bronze highlights, owner tools only. */
export function OwnerLayout() {
  const user = useAuthStore((state) => state.user)

  return (
    <div className="theme-kitchen flex min-h-svh bg-background text-foreground">
      <aside className="hidden w-72 shrink-0 flex-col bg-sidebar text-sidebar-foreground md:flex">
        <div className="relative h-36 overflow-hidden">
          <img src={KITCHEN_HERO} alt="" className="h-full w-full object-cover" />
          <div className="absolute inset-0 bg-linear-to-t from-sidebar via-sidebar/70 to-black/20" />
          <div className="absolute right-4 bottom-4 left-4 flex items-center gap-2">
            <span className="flex size-9 items-center justify-center rounded-xl bg-sidebar-primary text-sidebar-primary-foreground">
              <ChefHat className="size-4" />
            </span>
            <div>
              <p className="text-sm font-semibold">Sofra Kitchen</p>
              <p className="text-xs text-sidebar-foreground/70">Restaurant owner</p>
            </div>
          </div>
        </div>
        <nav className="flex flex-1 flex-col gap-1 p-4">
          {NAV.map((item) => (
            <NavLink key={item.to} to={item.to} className={navClass}>
              <item.icon className="size-4" />
              {item.label}
            </NavLink>
          ))}
        </nav>
        <div className="flex flex-col gap-3 border-t border-sidebar-border p-4">
          <p className="truncate px-2 text-xs text-sidebar-foreground/70">
            {user?.email}
          </p>
          <SignOutButton
            variant="secondary"
            className="border-transparent bg-sidebar-accent text-sidebar-accent-foreground hover:bg-sidebar-accent/80"
          />
        </div>
      </aside>

      <div className="flex min-w-0 flex-1 flex-col">
        <header className="flex items-center justify-between gap-3 border-b bg-card px-4 py-3 md:hidden">
          <p className="flex items-center gap-2 text-sm font-semibold">
            <ChefHat className="size-4 text-primary" />
            Sofra Kitchen
          </p>
          <SignOutButton variant="ghost" />
        </header>
        <nav className="flex gap-2 overflow-x-auto border-b bg-card px-3 py-2 md:hidden">
          {NAV.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                cn(
                  "flex shrink-0 items-center gap-1.5 rounded-full px-3 py-1.5 text-xs",
                  isActive ? "bg-primary text-primary-foreground" : "bg-muted"
                )
              }
            >
              <item.icon className="size-3.5" />
              {item.label}
            </NavLink>
          ))}
        </nav>
        <main className="flex-1 p-4 md:p-8">
          <ProfileSync />
          <Outlet />
        </main>
      </div>
    </div>
  )
}
