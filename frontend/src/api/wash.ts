import apiClient from "./client";
import type { ServiceCategoryDto, ServiceItemDto, WashScheduleDto } from "../types/api";

// NOTA: o backend expõe estas rotas em "api/services" (não "api/service"), confira ServiceController.cs
// ===== Categorias de serviço =====
export interface CreateServiceCategoryRequest {
  branchId: number;
  name: string;
  description?: string;
}

export async function createServiceCategory(
  payload: CreateServiceCategoryRequest,
): Promise<ServiceCategoryDto> {
  const response = await apiClient.post<ServiceCategoryDto>(
    "/api/services/categories",
    payload,
  );
  return response.data;
}

// ===== Itens de serviço =====
export interface CreateServiceItemRequest {
  serviceCategoryId: number;
  name: string;
  description?: string;
  durationMinutes: number;
  baseCost: number;
}

export async function createServiceItem(
  payload: CreateServiceItemRequest,
): Promise<ServiceItemDto> {
  const response = await apiClient.post<ServiceItemDto>("/api/services/items", payload);
  return response.data;
}

export async function getServiceItems(categoryId: number): Promise<ServiceItemDto[]> {
  const response = await apiClient.get<ServiceItemDto[]>("/api/services/items", {
    params: { categoryId },
  });
  return response.data;
}

// ===== Agendamentos =====
export interface CreateWashScheduleRequest {
  branchId: number;
  vehicleEntryId: number;
  scheduledTime: string;
  employeeId: number;
}

export async function createWashSchedule(
  payload: CreateWashScheduleRequest,
): Promise<WashScheduleDto> {
  const response = await apiClient.post<WashScheduleDto>("/api/wash-schedules", payload);
  return response.data;
}

export async function getWashSchedule(id: number): Promise<WashScheduleDto> {
  const response = await apiClient.get<WashScheduleDto>(`/api/wash-schedules/${id}`);
  return response.data;
}

export async function assignWashEmployee(
  id: number,
  employeeId: number,
): Promise<WashScheduleDto> {
  const response = await apiClient.post<WashScheduleDto>(
    `/api/wash-schedules/${id}/assign-employee`,
    { employeeId },
  );
  return response.data;
}
