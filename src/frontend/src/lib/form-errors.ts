import { isApiError, type ApiError } from "@/api/client"

/** Flatten TanStack Form's field error list (schema issues + string errors) into messages. */
export function fieldErrorMessages(errors: ReadonlyArray<unknown>): string[] {
  return errors
    .map((error) => {
      if (typeof error === "string") return error
      if (typeof error === "object" && error !== null && "message" in error) {
        const message = (error as { message?: unknown }).message
        if (typeof message === "string") return message
      }
      return undefined
    })
    .filter((message): message is string => Boolean(message))
}

/**
 * Map the backend's validation error map onto camelCase form fields. Keys
 * arrive PascalCase ("Email", "Password"), so matching is case-insensitive;
 * only the first message per field is kept (ready for form.setErrorMap).
 */
export function serverFieldErrors(
  error: ApiError,
  fieldNames: readonly string[]
): Record<string, string> {
  if (!error.fieldErrors) return {}
  const fields: Record<string, string> = {}
  for (const fieldName of fieldNames) {
    const target = fieldName.toLowerCase()
    const key = Object.keys(error.fieldErrors).find(
      (key) => key.toLowerCase() === target
    )
    const messages = key === undefined ? undefined : error.fieldErrors[key]
    if (messages?.length) fields[fieldName] = messages[0]
  }
  return fields
}

/** True when an API failure carries no field-level messages (form-level failure). */
export function isFormLevelError(error: ApiError): boolean {
  return !error.fieldErrors || Object.keys(error.fieldErrors).length === 0
}

/** Narrow an unknown submission failure to its API form (or null for foreign errors). */
export function asApiError(error: unknown): ApiError | null {
  return isApiError(error) ? error : null
}
