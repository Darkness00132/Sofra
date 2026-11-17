import { resolve } from "node:path"
import tailwindcss from "@tailwindcss/vite"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": resolve(import.meta.dirname, "./src"),
    },
  },
  server: {
    proxy: {
      // The API has no CORS policy; dev traffic rides the dev server origin.
      // `secure: false` accepts the ASP.NET Core self-signed dev certificate.
      "/api": {
        target: "https://localhost:7265",
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
