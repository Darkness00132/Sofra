import { useState } from "react"
import { Users } from "lucide-react"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { Skeleton } from "@/shared/components/ui/skeleton"
import { Alert, AlertDescription } from "@/shared/components/ui/alert"
import { Badge } from "@/shared/components/ui/badge"
import { Button } from "@/shared/components/ui/button"
import { Input } from "@/shared/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table"
import { useAccountsQuery, useSetAccountLockMutation } from "../service/api"

export function AdminAccountsPage() {
  const [role, setRole] = useState<string | null>(null)
  const [search, setSearch] = useState("")
  const accounts = useAccountsQuery(role, search)
  const setLock = useSetAccountLockMutation()

  return (
    <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
      <SurfaceHeader
        icon={Users}
        kicker="Administrator"
        title="Accounts"
        description="Customers, restaurant owners, and the locks that keep the platform safe."
      />

      <div className="flex flex-col gap-3 sm:flex-row">
        <Select
          value={role ?? "all"}
          onValueChange={(value) => setRole(value === "all" ? null : value)}
        >
          <SelectTrigger className="w-full sm:w-48">
            <SelectValue placeholder="All roles" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All roles</SelectItem>
            <SelectItem value="Customer">Customer</SelectItem>
            <SelectItem value="Restaurant">Restaurant</SelectItem>
            <SelectItem value="Administrator">Administrator</SelectItem>
          </SelectContent>
        </Select>
        <Input
          className="sm:max-w-sm"
          placeholder="Search by email…"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
      </div>

      {accounts.isLoading ? (
        <div className="flex flex-col gap-2">
          {Array.from({ length: 5 }).map((_, i) => (
            <Skeleton key={i} className="h-12 w-full" />
          ))}
        </div>
      ) : accounts.isError ? (
        <Alert variant="destructive">
          <AlertDescription>Could not load accounts.</AlertDescription>
        </Alert>
      ) : (accounts.data?.length ?? 0) === 0 ? (
        <p className="text-sm text-muted-foreground">No accounts in this view.</p>
      ) : (
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Email</TableHead>
                <TableHead>Roles</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {accounts.data!.map((account) => (
                <TableRow key={account.id}>
                  <TableCell className="font-medium">{account.email}</TableCell>
                  <TableCell>
                    {account.roles.length
                      ? account.roles.join(", ")
                      : "no roles"}
                  </TableCell>
                  <TableCell>
                    {account.locked ? (
                      <Badge variant="destructive">Locked</Badge>
                    ) : (
                      <Badge variant="secondary">Active</Badge>
                    )}
                  </TableCell>
                  <TableCell className="text-right">
                    {account.locked ? (
                      <Button
                        variant="outline"
                        size="sm"
                        disabled={setLock.isPending}
                        onClick={() =>
                          setLock.mutate({ accountId: account.id, locked: false })
                        }
                      >
                        Unlock
                      </Button>
                    ) : (
                      <Button
                        variant="destructive"
                        size="sm"
                        disabled={setLock.isPending}
                        onClick={() =>
                          setLock.mutate({ accountId: account.id, locked: true })
                        }
                      >
                        Lock
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}
    </div>
  )
}
