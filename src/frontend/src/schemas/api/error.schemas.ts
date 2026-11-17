import { z } from "zod"

/**
 * Exact shape of every Sofra API failure: an ASP.NET Core ProblemDetails plus
 * the backend's extension members, serialized camelCase.
 *
 * `errors` is present only on validation failures (400). Its keys are the
 * backend's PascalCase CLR property names ("Email", "Password"), so lookups
 * from camelCase form fields must be case-insensitive (see lib/form-errors).
 */
export const errorEnvelopeSchema = z.object({
  status: z.number().int(),
  title: z.string(),
  code: z.string(),
  detail: z.string(),
  instance: z.string(),
  traceId: z.string(),
  errors: z.record(z.string(), z.array(z.string())).optional(),
})
