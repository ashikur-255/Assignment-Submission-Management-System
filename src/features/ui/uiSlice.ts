import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface UiState { sidebarOpen: boolean; darkMode: boolean; toast: { type: "success" | "error" | "info"; message: string } | null; }
const initialState: UiState = {
  sidebarOpen: false,
  darkMode: localStorage.getItem("eduassign_theme") === "dark",
  toast: null
};

const slice = createSlice({
  name: "ui",
  initialState,
  reducers: {
    toggleSidebar: s => { s.sidebarOpen = !s.sidebarOpen; },
    closeSidebar: s => { s.sidebarOpen = false; },
    toggleTheme: s => {
      s.darkMode = !s.darkMode;
      localStorage.setItem("eduassign_theme", s.darkMode ? "dark" : "light");
    },
    showToast: (s, a: PayloadAction<UiState["toast"]>) => { s.toast = a.payload; },
    clearToast: s => { s.toast = null; }
  }
});
export const { toggleSidebar, closeSidebar, toggleTheme, showToast, clearToast } = slice.actions;
export default slice.reducer;
