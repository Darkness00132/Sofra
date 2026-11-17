import { useEffect } from "react"
import { useQuery } from "@tanstack/react-query"
import { fetchUserProfile } from "@/api/endpoints/auth.api"
import { mapUserProfileDtoToUser } from "@/models/mappers/auth.mappers"
import { queryKeys } from "@/state/query-keys"
import { selectIsAuthenticated, useAuthStore } from "@/state/stores/auth-store"

/**
 * The signed-in user's profile. Disabled while signed out; keeps the auth
 * store's user snapshot fresh (login responses carry no roles or phone).
 */
export function useProfileQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)
  const setUser = useAuthStore((state) => state.setUser)

  const query = useQuery({
    queryKey: queryKeys.auth.profile(),
    queryFn: async () => mapUserProfileDtoToUser(await fetchUserProfile()),
    enabled: authenticated,
    staleTime: 60_000,
  })

  useEffect(() => {
    if (query.data) setUser(query.data)
  }, [query.data, setUser])

  return query
}
