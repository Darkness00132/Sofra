# Sofra — Frontend Architecture

**What this is.** The storefront: a React 19 + TypeScript + Vite single-page app in `src/frontend/` — its own npm project, *not* part of the .NET solution. It talks to the versioned Web API at `/api/v1`.

Related: [backend-architecture](backend-architecture.md) · [erd](erd.md) · [API](API.md) · [PRD](PRD.md)

## 1. The stack

Strict TypeScript (with `erasableSyntaxOnly`, so no TS enums — roles are string-const objects). The state has been deliberately split across libraries so each does one job: **TanStack Query** owns *server* state (fetching, caching, refetching), **TanStack Form** owns *client* form state (validation, submission), **Zustand** with `persist` is the entire *client* store (just the auth session), **Axios** is the HTTP client, and **Zod** is the runtime validator at the API boundary. Styling is Tailwind v4, built on `@base-ui/react` headless primitives, with `sonner` toasts. The dev server proxies `/api` straight to the API, so dev avoids CORS and the self-signed cert entirely.

## 2. The layout

Three rings around the feature folders:

- **`app/`** — the shell: the single route tree, four layout shells, provider wiring, and the role home.
- **`shared/`** — everything features would otherwise duplicate: the axios client, the auth store, the auth/role guards, the UI kit, the error mapper, and theming.
- **`features/<name>/`** — one folder per domain (auth, discovery, carts, menus, orders, my-restaurant, restaurant-applications, restaurant-orders, admin). Each follows the same shape:

```
features/orders/
├── models/schemas.ts   # zod wire + response schemas            (always)
├── service/api.ts      # query keys + endpoint fns + TanStack hooks (always)
├── pages/              # the route components                   (always)
├── routes.tsx          # this feature's RouteObject[]           (always)
├── hooks/              # form / state view-models              (only when needed)
└── index.ts            # the ONLY file other features may import
```

`@/` is an alias for `src/`.

## 3. The app shell — routing and roles

`app/router.tsx` builds one tree from four "shells," each a layout wrapping its pages:

- **Auth** (public, split-screen) — login, register, forgot & reset password.
- **Customer marketplace** — public discovery (`restaurants`, details), plus a nested authenticated block with profile, cart, checkout, orders, and apply.
- **Restaurant owner** — dashboard, my-restaurant, menu, owner orders; wrapped in a Restaurant-role guard.
- **Admin** — dashboard, applications, restaurants, orders, accounts, payments; wrapped in an Administrator-role guard.

**Guards.** `RequireAuth` sends unauthenticated users to `/login?redirectTo=…` (which bounces them back). `RequireRoles` admits any of its roles; while roles are *unknown* it renders nothing rather than wrongly denying — the login response omits roles, and an invisible `ProfileSync` mounted in every shell fetches them. `/` is handled by `RoleHome`, which sends admins, owners, and customers to their own homes (and guests to the public restaurants list).

## 4. Feature slices — the pattern

**`service/api.ts` is the data module.** It holds a query-key registry for the feature, one pure function per endpoint — each of which **zod-parses the response** into the feature's schema before any React code sees it — and the TanStack Query hooks colocated right beside them (e.g. cart mutations write the fresh cart back into the cache on success).

**Forms** use TanStack Form v1 with *no* adapter. A `useXForm` hook owns `useForm` (schema validation on submit + a submission handler); on a server failure it routes the error into the form with `routeApiErrorToForm`: a known backend code wins, a form-level `detail` becomes the form error, otherwise the server's PascalCase field map is matched onto fields (applied via `form.setErrorMap`, not a zod adapter). The same hook pattern powers login/register, checkout, my-restaurant, and the apply form.

**Zod at the boundary** keeps the *wire* schemas (exactly what the API sends) separate from stricter *form* schemas (`z.email()`, min lengths, `.refine` password match); types are `z.infer` of the wire ones.

## 5. Data and auth plumbing

**One error shape.** The axios client normalizes *every* non-2xx (and network failure) into `ApiError { status, code, detail, fieldErrors }`, so the UI never touches an axios error. A request interceptor attaches the Bearer token from the store.

**Single-flight refresh.** On a 401 (that isn't an auth endpoint and hasn't already retried), a module-level promise deduplicates so N concurrent expired-401s cause exactly **one** refresh. The refresh POST deliberately uses a bare `axios.post` (not the shared instance, so the interceptors can't re-enter). On success it swaps the tokens and replays the original request; on failure it clears the session exactly once.

**The store** is Zustand `persist` (key `sofra.auth`) holding the access/refresh tokens and a user snapshot (id, email, roles, phone) — defined *structurally* in `shared` so it never imports a feature schema. The query client retries only 5xx/network (never 4xx) and never retries mutations.

## 6. UI and theming

The kit in `shared/components/ui/` is shadcn-style, built on `@base-ui/react` + `cva` variants + the `cn` class joiner. Tailwind v4 tokens live in `index.css` as CSS variables (light/dark, oklch). The nicest touch: **each shell carries its own accent theme** — terracotta "market" for customers, espresso "kitchen" for owners, indigo "console" for admins — all by swapping the same variables on a wrapper class. A small custom `ThemeProvider` handles light/dark/system with persistence and a `d` shortcut.

## 7. The routes, at a glance

| Who | Paths |
|---|---|
| Public | login, register, forgot/reset password · `restaurants` list + details |
| Customer (auth req.) | `profile`, `cart`, `checkout`, `orders` (+details), `restaurants/apply`, `restaurants/my-applications` |
| Owner (Restaurant) | `restaurant/dashboard`, `restaurants/me`, `menus`, `restaurant/orders` (+details) |
| Admin (Administrator) | `admin/` dashboard, restaurant-applications, restaurants, orders (+details), accounts, payments |
| Root | `/` = role home · `*` → `/` |

## 8. Adding a new slice

1. `models/schemas.ts` — zod wire + response schemas, and a form schema if there's a form.
2. `service/api.ts` — the query-key registry, endpoint functions (parse every response), and the hooks.
3. `pages/` — the route components.
4. `hooks/` — the form view-model, only if needed.
5. `routes.tsx` — the `RouteObject[]`.
6. Wire the routes into `app/router.tsx` under the right shell (plus guards). If another feature needs it, expose it through `index.ts` only.
