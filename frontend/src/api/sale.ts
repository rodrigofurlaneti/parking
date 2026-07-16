import apiClient from "./client";
import type { PaymentInput, SaleDto } from "../types/api";

export interface RegisterSaleRequest {
  branchId: number;
  vehicleExitId: number;
  cashRegisterId: number;
  payments: PaymentInput[];
}

export async function registerSale(payload: RegisterSaleRequest): Promise<SaleDto> {
  const response = await apiClient.post<SaleDto>("/api/sale", payload);
  return response.data;
}

export async function getSaleById(id: number): Promise<SaleDto> {
  const response = await apiClient.get<SaleDto>(`/api/sale/${id}`);
  return response.data;
}
