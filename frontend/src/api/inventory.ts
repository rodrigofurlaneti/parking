import apiClient from "./client";
import type {
  SupplierDto,
  ProductDto,
  PurchaseDto,
  PurchaseItemInput,
  ReceivePurchaseItemInput,
  StockMovementDto,
} from "../types/api";

// ===== Fornecedores =====
export interface CreateSupplierRequest {
  branchId: number;
  name: string;
  document: string;
  phone?: string;
  email?: string;
}

export async function createSupplier(payload: CreateSupplierRequest): Promise<SupplierDto> {
  const response = await apiClient.post<SupplierDto>("/api/supplier", payload);
  return response.data;
}

export async function getAllSuppliersByBranch(branchId: number): Promise<SupplierDto[]> {
  const response = await apiClient.get<SupplierDto[]>("/api/supplier", {
    params: { branchId },
  });
  return response.data;
}

// ===== Produtos =====
export interface CreateProductRequest {
  branchId: number;
  name: string;
  sku: string;
  category: string;
  cost: number;
  sellingPrice: number;
  stock: number;
  supplierId?: number | null;
  minimumStock?: number;
}

export async function createProduct(payload: CreateProductRequest): Promise<ProductDto> {
  const response = await apiClient.post<ProductDto>("/api/products", payload);
  return response.data;
}

export async function getProductStock(branchId: number): Promise<ProductDto[]> {
  const response = await apiClient.get<ProductDto[]>("/api/products/stock", {
    params: { branchId },
  });
  return response.data;
}

export async function adjustStock(
  productId: number,
  adjustment: number,
  reason: string,
): Promise<ProductDto> {
  const response = await apiClient.post<ProductDto>(`/api/products/${productId}/adjust-stock`, {
    adjustment,
    reason,
  });
  return response.data;
}

export async function getStockLedger(
  productId: number,
  fromDate?: string,
  toDate?: string,
): Promise<StockMovementDto[]> {
  const response = await apiClient.get<StockMovementDto[]>(
    `/api/products/${productId}/stock-ledger`,
    { params: { fromDate, toDate } },
  );
  return response.data;
}

export async function getBelowMinimum(branchId: number): Promise<ProductDto[]> {
  const response = await apiClient.get<ProductDto[]>("/api/products/below-minimum", {
    params: { branchId },
  });
  return response.data;
}

// ===== Compras =====
export interface CreatePurchaseRequest {
  branchId: number;
  supplierId: number;
  items: PurchaseItemInput[];
}

export async function createPurchase(payload: CreatePurchaseRequest): Promise<PurchaseDto> {
  const response = await apiClient.post<PurchaseDto>("/api/purchase", payload);
  return response.data;
}

export async function receivePurchaseItems(
  purchaseId: number,
  items: ReceivePurchaseItemInput[],
): Promise<PurchaseDto> {
  const response = await apiClient.post<PurchaseDto>(`/api/purchase/${purchaseId}/receive`, {
    items,
  });
  return response.data;
}
