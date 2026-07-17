// Tipos espelhando os DTOs reais do backend (.NET 9)
// Fonte: Parking.Application/Features/Auth/Login/LoginCommand.cs

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  userId: number;
  userName: string;
}

// Fonte: Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommand.cs
export interface RefreshTokenRequest {
  refreshToken: string;
}

// Fonte: Parking.Application/Features/Reports/GetOccupancyReport/OccupancyReportDto.cs
export interface HourlyOccupancyDto {
  hour: number;
  entryCount: number;
}

export interface OccupancyReportDto {
  branchId: number;
  fromDate: string;
  toDate: string;
  totalSpaces: number;
  totalEntries: number;
  averageOccupancyPercentage: number;
  peakHour: number;
  hourlyBreakdown: HourlyOccupancyDto[];
}

// Fonte: Parking.Application/Features/Reports/GetRevenueReport/RevenueReportDto.cs
export interface RevenueReportDto {
  branchId: number;
  fromDate: string;
  toDate: string;
  rotativeRevenue: number;
  agreementRevenue: number;
  monthlyRevenue: number;
  parkingTotalRevenue: number;
  washServiceRevenue: number;
  grandTotal: number;
}

// Fonte: Parking.Application/Features/Common/DTOs/ProductDto.cs
export interface ProductDto {
  id: number;
  branchId: number;
  name: string;
  sku: string;
  category: string;
  cost: number;
  sellingPrice: number;
  stock: number;
  minimumStock: number;
  supplierId: number | null;
  isActive: boolean;
}

// Fonte: Parking.Application/Features/Reports/GetStockReport/StockReportDto.cs
export interface ProductMovementCountDto {
  productId: number;
  productName: string;
  movementCount: number;
}

export interface StockReportDto {
  branchId: number;
  totalStockValue: number;
  belowMinimumProducts: ProductDto[];
  topMovedProducts: ProductMovementCountDto[];
}

// Fonte: Parking.Application/Features/Reports/GetEmployeeReport/EmployeeReportDto.cs
export interface EmployeeProductivityDto {
  employeeId: number;
  employeeName: string;
  hoursWorked: number;
  washSessionsCompleted: number;
}

export interface EmployeeReportDto {
  branchId: number;
  fromDate: string;
  toDate: string;
  employees: EmployeeProductivityDto[];
}

// Fonte: Parking.Application/Features/Reports/GetCashReport/CashReportDto.cs
export interface CashRegisterReconciliationDto {
  cashRegisterId: number;
  employeeId: number;
  openedAt: string;
  closedAt: string | null;
  expectedAmount: number;
  closingBalance: number;
  difference: number;
}

export interface OperatorSummaryDto {
  employeeId: number;
  registersOperated: number;
  totalDifference: number;
}

export interface CashReportDto {
  branchId: number;
  fromDate: string;
  toDate: string;
  reconciliations: CashRegisterReconciliationDto[];
  operatorSummary: OperatorSummaryDto[];
}

// ===== Pátio / Vagas =====
// Fonte: Parking.Application/Features/ParkingSpace/*
export interface ParkingSpaceDto {
  id: number;
  branchId: number;
  spaceNumber: string;
  type: number;
  status: number;
  isActive: boolean;
}

export interface ParkingSpaceDetailsDto extends ParkingSpaceDto {
  statusDescription: string;
}

export interface ParkingSpaceOccupancyDto {
  branchId: number;
  totalSpaces: number;
  occupiedSpaces: number;
  availableSpaces: number;
  occupancyRate: number;
}

// ===== Entrada/Saída de veículos =====
// Fonte: Parking.Application/Features/VehicleEntry/*
export interface VehicleEntryDto {
  id: number;
  branchId: number;
  parkingSpaceId: number;
  customerId: number;
  licensePlate: string;
  vehicleModel: string;
  vehicleColor: string;
  entryTime: string;
  exitTime: string | null;
  status: number;
  isActive: boolean;
}

// Fonte: Parking.Application/Features/VehicleEntry/RegisterVehicleEntryByPlate/*
export interface VehicleEntryByPlateResultDto {
  id: number;
  branchId: number;
  parkingSpaceId: number;
  customerId: number;
  customerName: string;
  customerType: number;
  isNewCustomer: boolean;
  licensePlate: string;
  vehicleModel: string;
  vehicleColor: string;
  entryTime: string;
  exitTime: string | null;
  status: number;
  isActive: boolean;
}

export interface VehicleExitDto {
  id: number;
  vehicleEntryId: number;
  exitTime: string;
  durationMinutes: number;
  totalAmount: number;
  parkingMode: number;
  isActive: boolean;
}

// ===== Caixa =====
// Fonte: Parking.Application/Features/CashRegister/*
export interface CashRegisterDto {
  id: number;
  branchId: number;
  employeeId: number;
  openedAt: string;
  closedAt: string | null;
  openingBalance: number;
  closingBalance: number;
  status: number;
  isActive: boolean;
}

export interface CashMovementDto {
  id: number;
  cashRegisterId: number;
  type: number;
  amount: number;
  description: string;
  referencedDocumentType: number | null;
  referencedDocumentId: number | null;
  isActive: boolean;
}

// ===== Vendas =====
// Fonte: Parking.Application/Features/Sale/RegisterSale/RegisterSaleCommand.cs
export interface PaymentInput {
  paymentMethod: number;
  amount: number;
}

export interface SalePaymentDto {
  id: number;
  paymentMethod: number;
  amount: number;
}

export interface SaleDto {
  id: number;
  branchId: number;
  vehicleExitId: number;
  saleNumber: number;
  totalAmount: number;
  saleDate: string;
  isActive: boolean;
  payments: SalePaymentDto[];
}

// ===== Funcionários =====
// Fonte: Parking.Application/Features/Employee/*
export interface EmployeeDto {
  id: number;
  companyId: number;
  branchId: number;
  name: string;
  email: string;
  phone: string;
  cpf: string;
  hireDate: string;
  terminationDate: string | null;
  roleId: number;
  isActive: boolean;
}

// ===== Clientes / Veículos / Contratos =====
// Fonte: Parking.Application/Features/Customer, Vehicle, AgreementCustomerContract, MonthlyCustomerContract, AgreementMerchant
export interface CustomerDto {
  id: number;
  branchId: number;
  customerType: number;
  name: string;
  document: string;
  phone: string | null;
  email: string | null;
  isActive: boolean;
}

export interface VehicleDto {
  id: number;
  customerId: number;
  licensePlate: string;
  model: string | null;
  color: string | null;
  isActive: boolean;
}

// Fonte: Parking.Application/Features/VehicleModel/*
// Catalogo compartilhado de modelos de veiculo (usado no autocomplete de entrada).
export interface VehicleModelDto {
  id: number;
  name: string;
  isActive: boolean;
}

export interface AgreementCustomerContractDto {
  id: number;
  customerId: number;
  agreementMerchantId: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface MonthlyCustomerContractDto {
  id: number;
  customerId: number;
  branchId: number;
  monthlyFee: number;
  maxVehicles: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface AgreementMerchantDto {
  id: number;
  branchId: number;
  companyName: string;
  discountPercentage: number;
  isActive: boolean;
}

// Fonte: Parking.Application/Features/Common/DTOs/TariffDto.cs
export interface TariffDto {
  id: number;
  branchId: number;
  firstHourRate: number;
  additionalHourRate: number;
  dailyMaxRate: number | null;
  isActive: boolean;
}

// Fonte: Parking.Application/Features/Common/DTOs/BranchDto.cs
export interface BranchDto {
  id: number;
  companyId: number;
  name: string;
  totalSpaces: number;
  isActive: boolean;
}

// ===== Lava Rápido =====
// Fonte: Parking.Application/Features/ServiceCategory, ServiceItem, WashSchedule
export interface ServiceCategoryDto {
  id: number;
  branchId: number;
  name: string;
  description: string | null;
  isActive: boolean;
}

export interface ServiceItemDto {
  id: number;
  serviceCategoryId: number;
  name: string;
  description: string | null;
  durationMinutes: number;
  baseCost: number;
  isActive: boolean;
}

export interface WashScheduleDto {
  id: number;
  branchId: number;
  vehicleEntryId: number;
  scheduledTime: string;
  actualStartTime: string | null;
  actualEndTime: string | null;
  employeeId: number;
  status: number;
  isActive: boolean;
}

// ===== Estoque =====
// Fonte: Parking.Application/Features/Supplier, Purchase, Product
export interface SupplierDto {
  id: number;
  branchId: number;
  name: string;
  document: string;
  phone: string | null;
  email: string | null;
  isActive: boolean;
}

export interface PurchaseItemInput {
  productId: number;
  quantityOrdered: number;
  unitCost: number;
}

export interface PurchaseItemDto {
  id: number;
  purchaseId: number;
  productId: number;
  quantityOrdered: number;
  quantityReceived: number;
  unitCost: number;
  isFullyReceived: boolean;
}

export interface PurchaseDto {
  id: number;
  branchId: number;
  supplierId: number;
  purchaseNumber: number;
  purchaseDate: string;
  status: number;
  isActive: boolean;
  items: PurchaseItemDto[];
}

export interface ReceivePurchaseItemInput {
  purchaseItemId: number;
  quantityReceived: number;
}

export interface StockMovementDto {
  id: number;
  productId: number;
  movementType: number;
  quantity: number;
  unitCost: number;
  reason: string;
  referencedDocumentType: string | null;
  referencedDocumentId: number | null;
  movementDate: string;
}
