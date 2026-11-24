import { useRef, useState } from "react"
import { Link } from "react-router-dom"
import { Store } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { coverImage } from "@/shared/lib/food-image"
import { FormTextField } from "@/shared/components/form-text-field"
import { Button, buttonVariants } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { Badge } from "@/shared/components/ui/badge"
import { Alert, AlertDescription } from "@/shared/components/ui/alert"
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@/shared/components/ui/tabs"
import {
  useMyRestaurantQuery,
  useRemoveRestaurantImageMutation,
  useSetAcceptingOrdersMutation,
  useUploadRestaurantImageMutation,
} from "../service/api"
import { useMyRestaurantForm } from "../hooks/use-my-restaurant-form"

export function MyRestaurantPage() {
  const restaurant = useMyRestaurantQuery()
  const { form, formError } = useMyRestaurantForm()
  const setAcceptingOrders = useSetAcceptingOrdersMutation()
  const uploadImage = useUploadRestaurantImageMutation()
  const removeImage = useRemoveRestaurantImageMutation()
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [selectedFile, setSelectedFile] = useState<File | null>(null)

  if (restaurant.isLoading) {
    return (
      <main className="p-4">
        <PageLoader />
      </main>
    )
  }

  if (restaurant.isError || !restaurant.data) {
    return (
      <main className="mx-auto flex min-h-svh max-w-md items-center justify-center p-4">
        <Card className="w-full">
          <CardHeader>
            <CardTitle>No restaurant yet</CardTitle>
            <CardDescription>
              Your storefront appears once your application is approved (BR-01).
            </CardDescription>
          </CardHeader>
          <CardFooter>
            <Link to="/restaurants/apply" className={buttonVariants({})}>
              Apply now
            </Link>
          </CardFooter>
        </Card>
      </main>
    )
  }

  const data = restaurant.data
  const busy =
    setAcceptingOrders.isPending ||
    uploadImage.isPending ||
    removeImage.isPending

  return (
    <div className="flex flex-col gap-6">
      <SurfaceHeader
        icon={Store}
        kicker="Restaurant"
        title={data.name}
        description={`Storefront is ${data.status}. Update the details customers see.`}
        image={coverImage(data.imageUrl, data.name)}
      />
      <div className="flex items-center justify-between">
        <Badge variant="secondary">{data.status}</Badge>
        <Link to="/menus" className="text-sm hover:underline">
          Manage menu
        </Link>
      </div>

      <Tabs defaultValue="info">
        <TabsList>
          <TabsTrigger value="info">Information</TabsTrigger>
          <TabsTrigger value="picture">Picture</TabsTrigger>
          <TabsTrigger value="orders">Order acceptance</TabsTrigger>
        </TabsList>

        <TabsContent value="info" className="mt-4">
          <Card>
            <CardHeader>
              <CardTitle>Restaurant information</CardTitle>
              <CardDescription>Update your storefront details.</CardDescription>
            </CardHeader>
            <CardContent>
              <form
                onSubmit={(event) => {
                  event.preventDefault()
                  form.handleSubmit()
                }}
                className="flex flex-col gap-4"
              >
                {formError ? (
                  <Alert variant="destructive">
                    <AlertDescription>{formError}</AlertDescription>
                  </Alert>
                ) : null}
                <form.Field name="name">
                  {(field) => (
                    <FormTextField
                      id={field.name}
                      label="Name"
                      value={field.state.value}
                      errors={field.state.meta.errors}
                      onBlur={field.handleBlur}
                      onChange={(value) => field.handleChange(value)}
                    />
                  )}
                </form.Field>
                <form.Field name="address">
                  {(field) => (
                    <FormTextField
                      id={field.name}
                      label="Address"
                      value={field.state.value}
                      errors={field.state.meta.errors}
                      onBlur={field.handleBlur}
                      onChange={(value) => field.handleChange(value)}
                    />
                  )}
                </form.Field>
                <form.Field name="phone">
                  {(field) => (
                    <FormTextField
                      id={field.name}
                      label="Phone"
                      value={field.state.value}
                      errors={field.state.meta.errors}
                      onBlur={field.handleBlur}
                      onChange={(value) => field.handleChange(value)}
                    />
                  )}
                </form.Field>
                <form.Field name="description">
                  {(field) => (
                    <FormTextField
                      id={field.name}
                      label="Description"
                      value={field.state.value}
                      errors={field.state.meta.errors}
                      onBlur={field.handleBlur}
                      onChange={(value) => field.handleChange(value)}
                    />
                  )}
                </form.Field>
                <form.Subscribe selector={(state) => state.isSubmitting}>
                  {(submitting) => (
                    <Button type="submit" disabled={submitting || busy}>
                      {submitting ? "Saving…" : "Save changes"}
                    </Button>
                  )}
                </form.Subscribe>
              </form>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="picture" className="mt-4">
          <Card>
            <CardHeader>
              <CardTitle>Storefront picture</CardTitle>
              <CardDescription>
                .jpg, .jpeg, .png or .webp — up to 5 MB.
              </CardDescription>
            </CardHeader>
            <CardContent className="flex flex-col gap-4">
              {data.imageUrl ? (
                <img
                  src={data.imageUrl}
                  alt={`${data.name} picture`}
                  className="max-h-64 w-full rounded-md object-cover"
                />
              ) : (
                <div className="flex h-40 items-center justify-center rounded-md border border-dashed text-sm text-muted-foreground">
                  No picture yet
                </div>
              )}
              <div className="flex flex-wrap items-center gap-2">
                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".jpg,.jpeg,.png,.webp"
                  className="text-sm"
                  onChange={(event) =>
                    setSelectedFile(event.target.files?.[0] ?? null)
                  }
                />
                <Button
                  variant="outline"
                  disabled={busy || !selectedFile}
                  onClick={() => {
                    if (selectedFile) uploadImage.mutate({ image: selectedFile })
                  }}
                >
                  {uploadImage.isPending ? "Uploading…" : "Upload"}
                </Button>
                {data.imageUrl ? (
                  <Button
                    variant="outline"
                    disabled={busy}
                    onClick={() => removeImage.mutate()}
                  >
                    Remove
                  </Button>
                ) : null}
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="orders" className="mt-4">
          <Card>
            <CardHeader>
              <CardTitle>Order acceptance</CardTitle>
              <CardDescription>
                Currently{" "}
                {data.acceptingOrders ? (
                  <span className="text-green-600 font-medium">
                    accepting orders
                  </span>
                ) : (
                  <span className="text-destructive font-medium">
                    not accepting orders
                  </span>
                )}
                .
              </CardDescription>
            </CardHeader>
            <CardFooter>
              <Button
                variant={data.acceptingOrders ? "destructive" : "default"}
                disabled={busy}
                onClick={() =>
                  setAcceptingOrders.mutate({
                    acceptingOrders: !data.acceptingOrders,
                  })
                }
              >
                {data.acceptingOrders
                  ? "Stop accepting orders"
                  : "Start accepting orders"}
              </Button>
            </CardFooter>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  )
}
