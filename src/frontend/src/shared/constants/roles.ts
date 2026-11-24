/**
 * Role names shared between guards, the nav and role filters. Kept as a
 * string-literal list (no TS enums — erasableSyntaxOnly).
 */
export const Roles = {
  Customer: "Customer",
  Restaurant: "Restaurant",
  Administrator: "Administrator",
} as const

export const allRoles: readonly string[] = [
  Roles.Customer,
  Roles.Restaurant,
  Roles.Administrator,
]
