import apiClient from "./client";
import type { LoginRequest, LoginResponse, RefreshTokenRequest } from "../types/api";

export async function login(userName: string, password: string): Promise<LoginResponse> {
  const payload: LoginRequest = { userName, password };
  const response = await apiClient.post<LoginResponse>("/api/auth/login", payload);
  return response.data;
}

// Fonte: Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommand.cs
// Request: { refreshToken: string } -> Response: LoginResponse (novo accessToken/refreshToken)
export async function refreshToken(refreshTokenValue: string): Promise<LoginResponse> {
  const payload: RefreshTokenRequest = { refreshToken: refreshTokenValue };
  const response = await apiClient.post<LoginResponse>("/api/auth/refresh-token", payload);
  return response.data;
}
