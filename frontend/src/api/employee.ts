import apiClient from "./client";
import type { EmployeeDto } from "../types/api";

export interface CreateEmployeeRequest {
  companyId: number;
  branchId: number;
  name: string;
  email: string;
  phone: string;
  cpf: string;
  roleId: number;
}

export async function createEmployee(payload: CreateEmployeeRequest): Promise<EmployeeDto> {
  const response = await apiClient.post<EmployeeDto>("/api/employee", payload);
  return response.data;
}

export async function getAllEmployeesByBranch(branchId: number): Promise<EmployeeDto[]> {
  const response = await apiClient.get<EmployeeDto[]>("/api/employee", {
    params: { branchId },
  });
  return response.data;
}

export async function getEmployeeById(employeeId: number): Promise<EmployeeDto> {
  const response = await apiClient.get<EmployeeDto>(`/api/employee/${employeeId}`);
  return response.data;
}

export interface UpdateEmployeeRequest {
  name: string;
  email: string;
  phone: string;
  roleId: number;
}

export async function updateEmployee(
  employeeId: number,
  payload: UpdateEmployeeRequest,
): Promise<EmployeeDto> {
  const response = await apiClient.put<EmployeeDto>(`/api/employee/${employeeId}`, payload);
  return response.data;
}

export async function terminateEmployee(employeeId: number): Promise<void> {
  await apiClient.post(`/api/employee/${employeeId}/terminate`);
}
