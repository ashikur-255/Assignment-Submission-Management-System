import {
  createAsyncThunk,
  createSlice,
  type PayloadAction,
} from "@reduxjs/toolkit";

import { api, apiError } from "../../lib/api";
import { clearAuth, getAuth, saveAuth } from "../../lib/storage";

import type {
  ApiResponse,
  AuthResponse,
  User,
} from "../../types";

interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  loading: boolean;
  error: string | null;
  initialized: boolean;
}

const stored = getAuth();

const initialState: AuthState = {
  user: stored?.user ?? null,
  accessToken: stored?.accessToken ?? null,
  refreshToken: stored?.refreshToken ?? null,
  loading: false,
  error: null,
  initialized: !!stored,
};

// ============================================================
// LOGIN
// ============================================================

export const login = createAsyncThunk<
  AuthResponse,
  {
    email: string;
    password: string;
  },
  {
    rejectValue: string;
  }
>(
  "auth/login",
  async (body, thunkAPI) => {
    try {
      const email = body.email.trim();
      const password = body.password;

      // Frontend validation
      if (!email) {
        return thunkAPI.rejectWithValue("Email is required.");
      }

      if (!password) {
        return thunkAPI.rejectWithValue("Password is required.");
      }

      // Basic email validation
      const emailRegex =
        /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

      if (!emailRegex.test(email)) {
        return thunkAPI.rejectWithValue(
          "Please enter a valid email address."
        );
      }

      const payload = {
        email,
        password,
      };

      console.log("LOGIN REQUEST:", payload);

      const response = await api.post<
        ApiResponse<AuthResponse>
      >(
        "/auth/login",
        payload
      );

      console.log("LOGIN RESPONSE:", response.data);

      if (!response.data.success || !response.data.data) {
        return thunkAPI.rejectWithValue(
          response.data.message || "Login failed."
        );
      }

      saveAuth(response.data.data);

      return response.data.data;
    } catch (error) {
      console.error("LOGIN ERROR:", error);

      return thunkAPI.rejectWithValue(
        apiError(error)
      );
    }
  }
);

// ============================================================
// REGISTER
// ============================================================

interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  password: string;
}

export const register = createAsyncThunk<
  User,
  RegisterRequest,
  {
    rejectValue: string;
  }
>(
  "auth/register",
  async (body, thunkAPI) => {
    try {
      const firstName = body.firstName.trim();
      const lastName = body.lastName.trim();
      const email = body.email.trim().toLowerCase();
      const phone = body.phone?.trim() || null;
      const password = body.password;

      if (!firstName) {
        return thunkAPI.rejectWithValue(
          "First name is required."
        );
      }

      if (!lastName) {
        return thunkAPI.rejectWithValue(
          "Last name is required."
        );
      }

      if (!email) {
        return thunkAPI.rejectWithValue(
          "Email is required."
        );
      }

      const emailRegex =
        /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

      if (!emailRegex.test(email)) {
        return thunkAPI.rejectWithValue(
          "Please enter a valid email address."
        );
      }

      if (password.length < 8) {
        return thunkAPI.rejectWithValue(
          "Password must be at least 8 characters."
        );
      }

      const payload = {
        firstName,
        lastName,
        email,
        phone,
        password,
      };

      console.log("========== REGISTER REQUEST ==========");
      console.log(payload);

      const response =
        await api.post<ApiResponse<User>>(
          "/auth/register",
          payload
        );

      console.log(
        "========== REGISTER RESPONSE =========="
      );
      console.log(response.data);

      if (
        !response.data.success ||
        !response.data.data
      ) {
        return thunkAPI.rejectWithValue(
          response.data.message ||
            "Registration failed."
        );
      }

      return response.data.data;

    } catch (error: any) {
      console.error(
        "========== REGISTER ERROR =========="
      );

      console.error("STATUS:", error.response?.status);
      console.error(
        "DATA:",
        error.response?.data
      );
      console.error(
        "HEADERS:",
        error.response?.headers
      );

      return thunkAPI.rejectWithValue(
        apiError(error)
      );
    }
  }
);

// ============================================================
// CURRENT USER
// ============================================================

export const fetchMe = createAsyncThunk<
  User,
  void,
  {
    rejectValue: string;
  }
>(
  "auth/fetchMe",
  async (_, thunkAPI) => {
    try {
      const { data } =
        await api.get<ApiResponse<User>>(
          "/auth/me"
        );

      if (!data.success || !data.data) {
        return thunkAPI.rejectWithValue(
          data.message || "Unable to load user."
        );
      }

      return data.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(
        apiError(error)
      );
    }
  }
);

// ============================================================
// LOGOUT
// ============================================================

export const logout = createAsyncThunk<
  void,
  void,
  {
    rejectValue: string;
  }
>(
  "auth/logout",
  async (_, thunkAPI) => {
    try {
      const auth = getAuth();

      if (auth?.refreshToken) {
        await api.post(
          "/auth/logout",
          {
            refreshToken: auth.refreshToken,
          }
        );
      }

      clearAuth();
    } catch (error) {
      clearAuth();

      return thunkAPI.rejectWithValue(
        apiError(error)
      );
    }
  }
);

// ============================================================
// SLICE
// ============================================================

const slice = createSlice({
  name: "auth",

  initialState,

  reducers: {
    setCredentials(
      state,
      action: PayloadAction<AuthResponse>
    ) {
      state.user = action.payload.user;
      state.accessToken =
        action.payload.accessToken;
      state.refreshToken =
        action.payload.refreshToken;

      state.error = null;
      state.initialized = true;
    },

    clearCredentials(state) {
      state.user = null;
      state.accessToken = null;
      state.refreshToken = null;
      state.initialized = true;
      state.error = null;

      clearAuth();
    },
  },

  extraReducers: (builder) => {
    builder

      // LOGIN
      .addCase(
        login.pending,
        (state) => {
          state.loading = true;
          state.error = null;
        }
      )

      .addCase(
        login.fulfilled,
        (state, action) => {
          state.loading = false;

          state.user =
            action.payload.user;

          state.accessToken =
            action.payload.accessToken;

          state.refreshToken =
            action.payload.refreshToken;

          state.initialized = true;
          state.error = null;
        }
      )

      .addCase(
        login.rejected,
        (state, action) => {
          state.loading = false;

          state.error =
            action.payload ??
            "Login failed.";
        }
      )

      // FETCH ME
      .addCase(
        fetchMe.fulfilled,
        (state, action) => {
          state.user =
            action.payload;

          state.initialized = true;
        }
      )

      .addCase(
        fetchMe.rejected,
        (state) => {
          state.initialized = true;
        }
      )

      // LOGOUT
      .addCase(
        logout.fulfilled,
        (state) => {
          state.user = null;
          state.accessToken = null;
          state.refreshToken = null;
          state.error = null;
          state.initialized = true;
        }
      )

      // REGISTER
      .addCase(
        register.pending,
        (state) => {
          state.loading = true;
          state.error = null;
        }
      )

      .addCase(
        register.fulfilled,
        (state) => {
          state.loading = false;
          state.error = null;
        }
      )

      .addCase(
        register.rejected,
        (state, action) => {
          state.loading = false;

          state.error =
            action.payload ??
            "Registration failed.";
        }
      );
  },
});

export const {
  setCredentials,
  clearCredentials,
} = slice.actions;

export default slice.reducer;