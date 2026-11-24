import type { LucideIcon } from "lucide-react"

interface SurfaceHeaderProps {
  icon: LucideIcon
  kicker: string
  title: string
  description?: string
  image?: string
}

/** Role-page title. A photo marks a home surface; an icon marks an inner page. */
export function SurfaceHeader({
  icon: Icon,
  kicker,
  title,
  description,
  image,
}: SurfaceHeaderProps) {
  if (image) {
    return (
      <header className="overflow-hidden rounded-3xl border bg-card shadow-sm">
        <div className="relative h-40 md:h-52">
          <img src={image} alt="" className="h-full w-full object-cover" />
          <div className="absolute inset-0 bg-linear-to-r from-black/80 via-black/50 to-black/15" />
          <div className="absolute inset-0 flex flex-col justify-end gap-1 p-5 text-white md:p-7">
            <p className="flex items-center gap-2 text-xs font-medium tracking-[0.16em] text-white/80 uppercase">
              <Icon className="size-3.5" />
              {kicker}
            </p>
            <h1 className="text-3xl font-semibold tracking-tight">{title}</h1>
            {description ? (
              <p className="max-w-xl text-sm text-white/80">{description}</p>
            ) : null}
          </div>
        </div>
      </header>
    )
  }

  return (
    <header className="flex items-start gap-3">
      <span className="flex size-11 shrink-0 items-center justify-center rounded-2xl bg-primary/10 text-primary">
        <Icon className="size-5" />
      </span>
      <div>
        <p className="text-xs font-medium tracking-[0.16em] text-primary uppercase">
          {kicker}
        </p>
        <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
        {description ? (
          <p className="text-sm text-muted-foreground">{description}</p>
        ) : null}
      </div>
    </header>
  )
}
