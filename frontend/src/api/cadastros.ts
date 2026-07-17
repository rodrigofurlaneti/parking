import apiClient from "./client";
import type { AgreementMerchantDto, TariffDto, BranchDto } from "../types/api";

// ===== Convênio (AgreementMerchant) =====
export interface CreateAgreementMerchantRequest {
  branchId: number;
  companyName: string;
  discountPercentage: number;
}

export async function createAgreementMerchant(
  payload: CreateAgreementMerchantRequest,
): Promise<AgreementMerchantDto> {
  const response = await apiClient.post<AgreementMerchantDto>("/api/agreement-merchant", payload);
  return response.data;
}

export async function getAllAgreementMerchantsByBranch(
  branchId: number,
): Promise<AgreementMerchantDto[]> {
  const response = await apiClient.get<AgreementMerchantDto[]>("/api/agreement-merchant", {
    params: { branchId },
  });
  return response.data;
}

export interface UpdateAgreementMerchantRequest {
  companyName: string;
  discountPercentage: number;
}

export async function updateAgreementMerchant(
  agreementMerchantId: number,
  payload: UpdateAgreementMerchantRequest,
): Promise<AgreementMerchantDto> {
  const response = await apiClient.put<AgreementMerchantDto>(
    `/api/agreement-merchant/${agreementMerchantId}`,
    payload,
  );
  return response.data;
}

export async function deactivateAgreementMerchant(agreementMerchantId: number): Promise<void> {
  await apiClient.post(`/api/agreement-merchant/${agreementMerchantId}/deactivate`);
}

// ===== Tarifa =====
export interface CreateTariffRequest {
  branchId: number;
  firstHourRate: number;
  additionalHourRate: number;
  dailyMaxRate?: number | null;
}

export async function createTariff(payload: CreateTariffRequest): Promise<TariffDto> {
  const response = await apiClient.post<TariffDto>("/api/tariff", payload);
  return response.data;
}

export async function getAllTariffsByBranch(branchId: number): Promise<TariffDto[]> {
  const response = await apiClient.get<TariffDto[]>("/api/tariff", {
    params: { branchId },
  });
  return response.data;
}

export interface UpdateTariffRequest {
  firstHourRate: number;
  additionalHourRate: number;
  dailyMaxRate?: number | null;
}

export async function updateTariff(
  tariffId: number,
  payload: UpdateTariffRequest,
): Promise<TariffDto> {
  const response = await apiClient.put<TariffDto>(`/api/tariff/${tariffId}`, payload);
  return response.data;
}

export async function deactivateTariff(tariffId: number): Promise<void> {
  await apiClient.post(`/api/tariff/${tariffId}/deactivate`);
}

// ===== Filial =====
export interface CreateBranchRequest {
  companyId: number;
  name: string;
  address: string;
  totalSpaces: number;
}

export async function createBranch(payload: CreateBranchRequest): Promise<BranchDto> {
  const response = await apiClient.post<BranchDto>("/api/branch", payload);
  return response.data;
}

export async function getAllBranchesByCompany(companyId: number): Promise<BranchDto[]> {
  const response = await apiClient.get<BranchDto[]>("/api/branch", {
    params: { companyId },
  });
  return response.data;
}

export interface UpdateBranchRequest {
  name: string;
  address: string;
  phoneNumber?: string;
  totalSpaces: number;
}

export async function updateBranch(
  branchId: number,
  payload: UpdateBranchRequest,
): Promise<BranchDto> {
  const response = await apiClient.put<BranchDto>(`/api/branch/${branchId}`, payload);
  return response.data;
}

export async function deactivateBranch(branchId: number): Promise<void> {
  await apiClient.post(`/api/branch/${branchId}/deactivate`);
}
