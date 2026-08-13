import axios from "axios";
import { getAuth, clearAuth } from "./storage";

const API_URL =
  import.meta.env.VITE_API_URL ||
  "https://localhost:7085/api";

export const api = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    const auth = getAuth();

    if (auth?.accessToken) {
      config.headers.Authorization =
        `Bearer ${auth.accessToken}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);

export function apiError(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data;

    if (
      data &&
      typeof data.Message === "string"
    ) {
      return data.Message;
    }

    if (
      data &&
      typeof data.message === "string"
    ) {
      return data.message;
    }

    if (
      data &&
      data.errors &&
      typeof data.errors === "object"
    ) {
      const messages: string[] = [];

      for (const value of Object.values(
        data.errors
      )) {
        if (Array.isArray(value)) {
          messages.push(...value.map(String));
        } else {
          messages.push(String(value));
        }
      }

      if (messages.length) {
        return messages.join(" ");
      }
    }

    if (
      typeof data === "string" &&
      data.trim()
    ) {
      return data;
    }

    switch (error.response?.status) {
      case 400:
        return "Invalid request.";

      case 401:
        return "Invalid email or password.";

      case 403:
        return "You are not authorized.";

      case 404:
        return "The requested resource was not found.";

      case 409:
        return "This record already exists.";

      case 500:
        return "A server error occurred.";

      default:
        return (
          error.message ||
          "An unexpected error occurred."
        );
    }
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "An unexpected error occurred.";
}