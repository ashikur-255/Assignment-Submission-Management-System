export const formatDate = (value?: string | null) =>
  value ? new Intl.DateTimeFormat("en-GB", { dateStyle: "medium" }).format(new Date(value)) : "—";

export const formatDateTime = (value?: string | null) =>
  value ? new Intl.DateTimeFormat("en-GB", { dateStyle: "medium", timeStyle: "short" }).format(new Date(value)) : "—";

export const isOverdue = (deadline: string) => new Date(deadline).getTime() < Date.now();

export const initials = (first = "", last = "") =>
  `${first[0] ?? ""}${last[0] ?? ""}`.toUpperCase();

export const fileUrl = (path?: string | null) => {
  if (!path) return "";
  if (/^https?:\/\//i.test(path)) return path;
  const base =
  import.meta.env.VITE_FILE_BASE_URL ||
  "https://localhost:7085";
  return `${base}${path.startsWith("/") ? "" : "/"}${path}`;
};
