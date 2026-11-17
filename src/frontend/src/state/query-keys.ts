/**
 * Central query-key registry — one place to keep cache reads and
 * invalidations consistent across queries and mutations.
 */
export const queryKeys = {
  auth: {
    profile: () => ["auth", "profile"] as const,
  },
} as const
