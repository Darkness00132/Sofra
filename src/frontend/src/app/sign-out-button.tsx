import { LogOut } from "lucide-react"
import { useNavigate } from "react-router-dom"
import { useLogoutMutation } from "@/features/auth/service/api"
import { Button } from "@/shared/components/ui/button"
import { useAuthStore } from "@/shared/stores/auth.store"

export function SignOutButton({
  variant = "outline",
  className,
}: {
  variant?: "outline" | "ghost" | "secondary"
  className?: string
}) {
  const refreshToken = useAuthStore((state) => state.refreshToken)
  const logout = useLogoutMutation()
  const navigate = useNavigate()

  return (
    <Button
      variant={variant}
      size="sm"
      className={className}
      disabled={!refreshToken || logout.isPending}
      onClick={() => {
        if (refreshToken) {
          logout.mutate({ refreshToken })
          navigate("/login", { replace: true })
        }
      }}
    >
      <LogOut />
      {logout.isPending ? "Signing out…" : "Sign out"}
    </Button>
  )
}
