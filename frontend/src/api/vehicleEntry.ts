import apiClient from "./client";
import type {
  ParkingSpaceDto,
  ParkingSpaceDetailsDto,
  ParkingSpaceOccupancyDto,
  VehicleEntryDto,
  VehicleExitDto,
  CashRegisterDto,
  CashMovementDto,
} from "../types/api";

// ===== Vagas =====
export interface CreateParkingSpaceRequest {
  branchId: number;
  spaceNumber: string;
  type: number;
}

export async function createParkingSpace(
  payload: CreateParkingSpaceRequest,
): Promise<ParkingSpaceDto> {
  const response = await apiClient.post<ParkingSpaceDto>("/api/parking-space", payload);
  return response.data;
}

export async function getParkingSpaceDetails(
  parkingSpaceId: number,
): Promise<ParkingSpaceDetailsDto> {
  const response = await apiClient.get<ParkingSpaceDetailsDto>(
    `/api/parking-space/${parkingSpaceId}`,
  );
  return response.data;
}

export async function getParkingSpaceOccupancy(
  branchId: number,
): Promise<ParkingSpaceOccupancyDto> {
  const response = await apiClient.get<ParkingSpaceOccupancyDto>(
    `/api/parking-space/branch/${branchId}/occupancy`,
  );
  return response.data;
}

export async function getAllParkingSpacesByBranch(
  branchId: number,
): Promise<ParkingSpaceDto[]> {
  const response = await apiClient.get<ParkingSpaceDto[]>(
    `/api/parking-space/branch/${branchId}`,
  );
  return response.data;
}

// ===== Entrada/Saída =====
export interface RegisterVehicleEntryRequest {
  branchId: number;
  parkingSpaceId: number;
  customerId: number;
  licensePlate: string;
  vehicleModel: string;
  vehicleColor: string;
}

export async function registerVehicleEntry(
  payload: RegisterVehicleEntryRequest,
): Promise<VehicleEntryDto> {
  const response = await apiClient.post<VehicleEntryDto>("/api/vehicle-entry/entry", payload);
  return response.data;
}

export interface RegisterVehicleExitRequest {
  vehicleEntryId: number;
}

export async function registerVehicleExit(
  payload: RegisterVehicleExitRequest,
): Promise<VehicleExitDto> {
  const response = await apiClient.post<VehicleExitDto>("/api/vehicle-entry/exit", payload);
  return response.data;
}

export async function getVehicleEntry(vehicleEntryId: number): Promise<VehicleEntryDto> {
  const response = await apiClient.get<VehicleEntryDto>(`/api/vehicle-entry/${vehicleEntryId}`);
  return response.data;
}

export async function getOpenVehicleEntriesByBranch(
  branchId: number,
): Promise<VehicleEntryDto[]> {
  const response = await apiClient.get<VehicleEntryDto[]>("/api/vehicle-entry/open", {
    params: { branchId },
  });
  return response.data;
}

// ===== Caixa =====
export interface OpenCashRegisterRequest {
  branchId: number;
  employeeId: number;
  openingBalance: number;
}

export async function openCashRegister(
  payload: OpenCashRegisterRequest,
): Promise<CashRegisterDto> {
  const response = await apiClient.post<CashRegisterDto>("/api/cash-register/open", payload);
  return response.data;
}

export interface RecordCashMovementRequest {
  type: number;
  amount: number;
  description: string;
}

export async function recordCashMovement(
  cashRegisterId: number,
  payload: RecordCashMovementRequest,
): Promise<CashMovementDto> {
  const response = await apiClient.post<CashMovementDto>(
    `/api/cash-register/${cashRegisterId}/movements`,
    payload,
  );
  return response.data;
}

export async function closeCashRegister(
  cashRegisterId: number,
  closingBalance: number,
): Promise<CashRegisterDto> {
  const response = await apiClient.post<CashRegisterDto>(
    `/api/cash-register/${cashRegisterId}/close`,
    { closingBalance },
  );
  return response.data;
}

export async function getCashRegisterBalance(cashRegisterId: number): Promise<number> {
  const response = await apiClient.get<number>(`/api/cash-register/${cashRegisterId}/balance`);
  return response.data;
}

export async function getOpenCashRegisterByBranch(
  branchId: number,
): Promise<CashRegisterDto | null> {
  const response = await apiClient.get<CashRegisterDto | null>("/api/cash-register/open", {
    params: { branchId },
  });
  return response.data;
}
