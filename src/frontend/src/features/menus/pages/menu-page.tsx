import { useRef, useState } from "react"
import { UtensilsCrossed } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { Input } from "@/shared/components/ui/input"
import {
  useAddMenuItemMutation,
  useAddMenuTypeMutation,
  useMyMenuQuery,
  useRemoveMenuItemImageMutation,
  useRemoveMenuItemMutation,
  useRemoveMenuTypeMutation,
  useSetMenuItemAvailabilityMutation,
  useUploadMenuItemImageMutation,
} from "../service/api"

/**
 * The owner's menu management (FR-13…FR-16): menu types under platform
 * categories, food items with prices, availability toggles and pictures.
 * Every mutation invalidates the menu query.
 */
export function MenuPage() {
  const menu = useMyMenuQuery()
  const [newTypeName, setNewTypeName] = useState("")

  const addMenuType = useAddMenuTypeMutation()
  const removeMenuType = useRemoveMenuTypeMutation()
  const removeMenuItem = useRemoveMenuItemMutation()
  const setAvailability = useSetMenuItemAvailabilityMutation()
  const addMenuItem = useAddMenuItemMutation()

  if (menu.isLoading) {
    return (
      <main className="p-4">
        <PageLoader />
      </main>
    )
  }

  if (menu.isError || !menu.data) {
    return (
      <main className="mx-auto flex min-h-svh max-w-md items-center justify-center p-4">
        <Card className="w-full">
          <CardHeader>
            <CardTitle>No restaurant yet</CardTitle>
            <CardDescription>
              Your menu appears once your application is approved (BR-01).
            </CardDescription>
          </CardHeader>
        </Card>
      </main>
    )
  }

  const toggleAvailability = (foodItemId: string, available: boolean) =>
    setAvailability.mutate({ foodItemId, request: { available } })

  return (
    <div className="mx-auto flex w-full max-w-3xl flex-col gap-4">
      <SurfaceHeader
        icon={UtensilsCrossed}
        kicker="Restaurant"
        title="Menu"
        description="Group dishes, set prices, and show what is available tonight."
      />

      <Card>
        <CardHeader>
          <CardTitle>Add a menu section</CardTitle>
          <CardDescription>
            A grouping under a platform category (e.g. "Beef Burgers" under
            "Burger").
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-2">
          <Input
            placeholder="Menu type name"
            value={newTypeName}
            onChange={(event) => setNewTypeName(event.target.value)}
          />
          <Button
            disabled={newTypeName.trim() === "" || addMenuType.isPending}
            onClick={() =>
              addMenuType.mutate(
                {
                  foodCategoryId: menu.data.menuTypes[0]?.foodCategoryId ?? "",
                  name: newTypeName.trim(),
                  description: null,
                },
                { onSuccess: () => setNewTypeName("") }
              )
            }
          >
            {addMenuType.isPending ? "Adding…" : "Add menu type"}
          </Button>
          <p className="text-xs text-muted-foreground">
            New types land under the same platform category as your first type
            until a category picker is added.
          </p>
        </CardContent>
      </Card>

      {menu.data.menuTypes.length === 0 ? (
        <p className="text-muted-foreground">
          No menu types yet — add your first one above.
        </p>
      ) : (
        menu.data.menuTypes.map((type) => (
          <Card key={type.id}>
            <CardHeader className="flex flex-row items-center justify-between">
              <div>
                <CardTitle>{type.name}</CardTitle>
                {type.description ? (
                  <CardDescription>{type.description}</CardDescription>
                ) : null}
              </div>
              <Button
                variant="outline"
                disabled={removeMenuType.isPending}
                onClick={() => removeMenuType.mutate(type.id)}
              >
                Remove type
              </Button>
            </CardHeader>
            <CardContent className="flex flex-col gap-3">
              {type.items.map((item) => (
                <div
                  key={item.id}
                  className="flex flex-col gap-2 border-b pb-3 last:border-b-0"
                >
                  <div className="flex items-center justify-between gap-4">
                    <div>
                      <p className="font-medium">
                        {item.name} · {item.price}
                      </p>
                      <p className="text-sm text-muted-foreground">
                        {item.description}
                      </p>
                      <p className="text-sm">
                        {item.isAvailable ? (
                          <span className="text-green-600">
                            {item.availability}
                          </span>
                        ) : (
                          <span className="text-destructive">
                            {item.availability}
                          </span>
                        )}
                      </p>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        disabled={setAvailability.isPending}
                        onClick={() =>
                          toggleAvailability(item.id, !item.isAvailable)
                        }
                      >
                        Toggle availability
                      </Button>
                      <Button
                        variant="outline"
                        disabled={removeMenuItem.isPending}
                        onClick={() => removeMenuItem.mutate(item.id)}
                      >
                        Remove
                      </Button>
                    </div>
                  </div>
                  <MenuItemImageControls foodItemId={item.id} imageUrl={item.imageUrl} />
                </div>
              ))}

              <AddItemInlineForm menuTypeId={type.id} />
            </CardContent>
          </Card>
        ))
      )}
    </div>
  )

  /** Inline add-item form for one menu type. */
  function AddItemInlineForm({ menuTypeId }: { menuTypeId: string }) {
    const [name, setName] = useState("")
    const [description, setDescription] = useState("")
    const [price, setPrice] = useState("")

    return (
      <div className="flex flex-col gap-2 rounded-md border p-3">
        <Input
          placeholder="Item name"
          value={name}
          onChange={(event) => setName(event.target.value)}
        />
        <Input
          placeholder="Description"
          value={description}
          onChange={(event) => setDescription(event.target.value)}
        />
        <Input
          placeholder="Price"
          inputMode="decimal"
          value={price}
          onChange={(event) => setPrice(event.target.value)}
        />
        <Button
          disabled={
            name.trim() === "" ||
            description.trim() === "" ||
            price.trim() === "" ||
            addMenuItem.isPending
          }
          onClick={() =>
            addMenuItem.mutate(
              {
                menuTypeId,
                name: name.trim(),
                description: description.trim(),
                price: Number(price),
                availability: "Available",
              },
              {
                onSuccess: () => {
                  setName("")
                  setDescription("")
                  setPrice("")
                },
              }
            )
          }
        >
          {addMenuItem.isPending ? "Adding…" : "Add item"}
        </Button>
      </div>
    )
  }
}

/** Picture upload / remove controls for one food item. */
function MenuItemImageControls({
  foodItemId,
  imageUrl,
}: {
  foodItemId: string
  imageUrl: string | null
}) {
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const uploadImage = useUploadMenuItemImageMutation()
  const removeImage = useRemoveMenuItemImageMutation()
  const busy = uploadImage.isPending || removeImage.isPending

  return (
    <div className="flex items-center gap-2">
      <input
        ref={fileInputRef}
        type="file"
        accept=".jpg,.jpeg,.png,.webp"
        className="text-sm"
        onChange={(event) => setSelectedFile(event.target.files?.[0] ?? null)}
      />
      <Button
        variant="outline"
        disabled={busy || !selectedFile}
        onClick={() => {
          if (selectedFile) uploadImage.mutate({ foodItemId, image: selectedFile })
        }}
      >
        {uploadImage.isPending ? "Uploading…" : "Upload picture"}
      </Button>
      {imageUrl ? (
        <Button
          variant="outline"
          disabled={busy}
          onClick={() => removeImage.mutate(foodItemId)}
        >
          Remove picture
        </Button>
      ) : null}
    </div>
  )
}
