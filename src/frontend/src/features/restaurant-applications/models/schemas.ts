import { z } from "zod"

/**
 * The restaurant-application feature's contracts: submitting an application
 * for administrator review and tracking its status (FR-11, FR-47, FR-48).
 * Backend enums serialize as strings.
 */
// ─── Wire payloads ───────────────────────────────────────────────────────────

export const applyApplicationRequestSchema = z.object({
  restaurantName: z.string(),
  address: z.string(),
  phone: z.string(),
  description: z.string(),
})

export const applicationResponseSchema = z.object({
  id: z.guid(),
  restaurantName: z.string(),
  address: z.string(),
  phone: z.string(),
  description: z.string(),
  status: z.string(),
  rejectionReason: z.string().nullable(),
  createdAt: z.iso.datetime(),
  reviewedAt: z.iso.datetime().nullable(),
  applicantEmail: z.string(),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type ApplyApplicationRequest = z.infer<
  typeof applyApplicationRequestSchema
>
export type ApplicationResponse = z.infer<typeof applicationResponseSchema>

// ─── Form schema (stricter than the wire) ────────────────────────────────────

export const applyApplicationFormSchema = z.object({
  restaurantName: z.string().min(1, "Restaurant name is required."),
  address: z.string().min(1, "Address is required."),
  phone: z.string().min(1, "Phone is required."),
  description: z.string().min(1, "Description is required."),
})
