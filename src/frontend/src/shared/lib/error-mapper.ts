import { isApiError, type ApiError } from "@/shared/api/client"

/**
 * Backend envelope codes that are really field-level problems, mapped to the
 * form field they invalidate. One declarative registry instead of if/switch
 * chains inside every form hook — a new backend code is one line here.
 */
const errorCodeFieldMap: Record<string, string | undefined> = {
  EMAIL_ALREADY_REGISTERED: "email",
  INVALID_RESET_CODE: "otp",
  PASSWORD_INVALID: "newPassword",
}

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
function serverFieldErrors(
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
function isFormLevelError(error: ApiError): boolean {
  return !error.fieldErrors || Object.keys(error.fieldErrors).length === 0
}

/** Narrow an unknown submission failure to its API form (or null for foreign errors). */
export function asApiError(error: unknown): ApiError | null {
  return isApiError(error) ? error : null
}

/**
 * Route one API failure into form UX: either a form-level message or a
 * field→message map (ready for form.setErrorMap). Known error codes win
 * first, then the validation error map, otherwise the detail goes form-level.
 */
export function routeApiErrorToForm(
  error: ApiError,
  fieldNames: readonly string[]
): { formError: string | null; fieldErrors: Record<string, string> } {
  const mappedField = errorCodeFieldMap[error.code]
  if (mappedField)
    return { formError: null, fieldErrors: { [mappedField]: error.detail } }
  if (isFormLevelError(error))
    return { formError: error.detail, fieldErrors: {} }
  return { formError: null, fieldErrors: serverFieldErrors(error, fieldNames) }
}
