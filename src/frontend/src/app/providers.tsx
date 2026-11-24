import type { ReactNode } from "react"
import { QueryClientProvider } from "@tanstack/react-query"
import { ThemeProvider } from "@/shared/components/theme-provider"
import { TooltipProvider } from "@/shared/components/ui/tooltip"
import { Toaster } from "@/shared/components/ui/sonner"
import { queryClient } from "@/shared/api/query-client"

/** Composition of all cross-cutting React providers. */
export function Providers({ children }: { children: ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider defaultTheme="light">
        <TooltipProvider>{children}</TooltipProvider>
        <Toaster />
      </ThemeProvider>
    </QueryClientProvider>
  )
}
