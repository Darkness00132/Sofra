import { Input } from "@/shared/components/ui/input"
import { Label } from "@/shared/components/ui/label"
import { fieldErrorMessages } from "@/shared/lib/error-mapper"

interface FormTextFieldProps {
  id: string
  label: string
  value: string
  errors: ReadonlyArray<unknown>
  type?: "text" | "email" | "password"
  autoComplete?: string
  inputMode?: "text" | "numeric"
  onBlur: () => void
  onChange: (value: string) => void
}

/**
 * Dumb text input with label and error list — everything arrives via props,
 * including the TanStack field state the parent wires up.
 */
export function FormTextField({
  id,
  label,
  value,
  errors,
  type = "text",
  autoComplete,
  inputMode,
  onBlur,
  onChange,
}: FormTextFieldProps) {
  const messages = fieldErrorMessages(errors)

  return (
    <div className="flex flex-col gap-2">
      <Label htmlFor={id}>{label}</Label>
      <Input
        id={id}
        name={id}
        type={type}
        autoComplete={autoComplete}
        inputMode={inputMode}
        value={value}
        onBlur={onBlur}
        onChange={(event) => onChange(event.target.value)}
        aria-invalid={messages.length > 0}
      />
      {messages.map((message) => (
        <p key={message} className="text-sm text-destructive">
          {message}
        </p>
      ))}
    </div>
  )
}
