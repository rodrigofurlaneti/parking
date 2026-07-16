import apiClient from "./client";
import type {
  CustomerDto,
  VehicleDto,
  AgreementCustomerContractDto,
  MonthlyCustomerContractDto,
  AgreementMerchantDto,
} from "../types/api";

// ===== Clientes =====
export interface CreateCustomerRequest {
  branchId: number;
  customerType: number;
  name: string;
  document: string;
  phone?: string;
  email?: string;
}

export async function createCustomer(payload: CreateCustomerRequest): Promise<CustomerDto> {
  const response = await apiClient.post<CustomerDto>("/api/customer", payload);
  return response.data;
}

export async function getCustomerByDocument(document: string): Promise<CustomerDto> {
  const response = await apiClient.get<CustomerDto>(`/api/customer/document/${document}`);
  return response.data;
}

export async function getAllCustomersByBranch(branchId: number): Promise<CustomerDto[]> {
  const response = await apiClient.get<CustomerDto[]>("/api/customer", {
    params: { branchId },
  });
  return response.data;
}

// ===== Veículos =====
export interface CreateVehicleRequest {
  customerId: number;
  licensePlate: string;
  model?: string;
  color?: string;
}

export async function createVehicle(payload: CreateVehicleRequest): Promise<VehicleDto> {
  const response = await apiClient.post<VehicleDto>("/api/vehicle", payload);
  return response.data;
}

// ===== Contratos =====
export interface CreateAgreementContractRequest {
  customerId: number;
  agreementMerchantId: number;
  startDate: string;
  endDate: string;
}

export async function createAgreementContract(
  payload: CreateAgreementContractRequest,
): Promise<AgreementCustomerContractDto> {
  const response = await apiClient.post<AgreementCustomerContractDto>(
    "/api/contract/agreement",
    payload,
  );
  return response.data;
}

export interface CreateMonthlyContractRequest {
  customerId: number;
  branchId: number;
  monthlyFee: number;
  maxVehicles: number;
  startDate: string;
  endDate: string;
}

export async function createMonthlyContract(
  payload: CreateMonthlyContractRequest,
): Promise<MonthlyCustomerContractDto> {
  const response = await apiClient.post<MonthlyCustomerContractDto>(
    "/api/contract/monthly",
    payload,
  );
  return response.data;
}

// ===== Empresa conveniada =====
export interface CreateAgreementMerchantRequest {
  branchId: number;
  companyName: string;
  discountPercentage: number;
}

export async function createAgreementMerchant(
  payload: CreateAgreementMerchantRequest,
): Promise<AgreementMerchantDto> {
  const response = await apiClient.post<AgreementMerchantDto>(
    "/api/agreement-merchant",
    payload,
  );
  return response.data;
}
