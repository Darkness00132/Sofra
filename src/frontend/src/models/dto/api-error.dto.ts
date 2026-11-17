import type { z } from "zod"
import type { errorEnvelopeSchema } from "@/schemas/api/error.schemas"

/**
 * Wire shape of every API failure — the camelCase ProblemDetails envelope
 * (documenting the contract here keeps it greppable next to the other DTOs).
 */
export type ApiErrorEnvelopeDto = z.infer<typeof errorEnvelopeSchema>
