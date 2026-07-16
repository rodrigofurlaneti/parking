import apiClient from "./client";
import type {
  OccupancyReportDto,
  RevenueReportDto,
  StockReportDto,
  EmployeeReportDto,
  CashReportDto,
} from "../types/api";

export interface DateRangeParams {
  branchId: number;
  fromDate: string;
  toDate: string;
}

export async function getOccupancyReport(params: DateRangeParams): Promise<OccupancyReportDto> {
  const response = await apiClient.get<OccupancyReportDto>("/api/reports/occupancy", { params });
  return response.data;
}

export async function getRevenueReport(params: DateRangeParams): Promise<RevenueReportDto> {
  const response = await apiClient.get<RevenueReportDto>("/api/reports/revenue", { params });
  return response.data;
}

export async function getStockReport(branchId: number): Promise<StockReportDto> {
  const response = await apiClient.get<StockReportDto>("/api/reports/stock", { params: { branchId } });
  return response.data;
}

export async function getEmployeeReport(params: DateRangeParams): Promise<EmployeeReportDto> {
  const response = await apiClient.get<EmployeeReportDto>("/api/reports/employees", { params });
  return response.data;
}

export async function getCashReport(params: DateRangeParams): Promise<CashReportDto> {
  const response = await apiClient.get<CashReportDto>("/api/reports/cash", { params });
  return response.data;
}
