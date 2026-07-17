import apiClient from "./client";
import type { VehicleModelDto } from "../types/api";

// Catalogo compartilhado de modelos de veiculo. Usado pelo autocomplete de "modelo" nas
// telas de entrada de veiculo, para padronizar o que os funcionarios digitam.
export async function searchVehicleModels(query: string): Promise<VehicleModelDto[]> {
  if (!query.trim()) return [];
  const response = await apiClient.get<VehicleModelDto[]>("/api/vehicle-model/search", {
    params: { q: query.trim() },
  });
  return response.data;
}

export async function getAllVehicleModels(): Promise<VehicleModelDto[]> {
  const response = await apiClient.get<VehicleModelDto[]>("/api/vehicle-model");
  return response.data;
}

export interface CreateVehicleModelRequest {
  name: string;
}

// Get-or-create no backend: se o nome ja existir (ignorando maiusculas/minusculas), devolve
// o modelo existente em vez de duplicar.
export async function createVehicleModel(
  payload: CreateVehicleModelRequest,
): Promise<VehicleModelDto> {
  const response = await apiClient.post<VehicleModelDto>("/api/vehicle-model", payload);
  return response.data;
}
