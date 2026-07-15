# 🚀 Fase 3 — Lava Rápido (Car Wash Services)

**Status:** ✅ Ready to Build  
**Tempo Estimado:** 10 dias (2 sprints)  
**Dependência:** Fase 1 + Fase 2 ✅ Completo

---

## 📊 O Que Você Vai Implementar

### Sprint 1 (Dias 1-5): Service Catalog + Basic Scheduling
- Service categories (e.g., "Lavagem Rápida", "Lavagem Completa")
- Service items (e.g., "Exterior", "Interior", "Wax")
- Products/inventory management
- Basic wash scheduling

### Sprint 2 (Dias 6-10): Workforce + Cost Tracking
- Assign wash employees
- Track product consumption
- Calculate monthly operational costs
- Revenue reporting by service/employee

---

## 📂 Arquivos Que Você Vai Criar

### Domain (18 novos arquivos)

**Service Catalog (6 arquivos):**
```
Parking.Domain/Entities/
├── ServiceCategory.cs (AggregateRoot)
├── ServiceItem.cs (Entity)
└── Product.cs (AggregateRoot)

Parking.Domain/ValueObjects/
├── Duration.cs (minutos)
└── Cost.cs (custo)

Parking.Domain/Repositories/
├── IServiceCategoryRepository.cs
├── IServiceItemRepository.cs
└── IProductRepository.cs
```

**Scheduling & Workforce (6 arquivos):**
```
Parking.Domain/Entities/
├── WashSchedule.cs (AggregateRoot)
├── WashSession.cs (Entity)
└── WashEmployee.cs (Entity extending Employee)

Parking.Domain/Repositories/
├── IWashScheduleRepository.cs
├── IWashSessionRepository.cs
└── IWashEmployeeRepository.cs
```

**Cost Tracking (6 arquivos):**
```
Parking.Domain/Entities/
├── WashOperationalCost.cs (AggregateRoot)
├── WashServiceRevenue.cs (Entity)
└── WashProductConsumption.cs (Entity)

Parking.Domain/Repositories/
├── IWashOperationalCostRepository.cs
├── IWashServiceRevenueRepository.cs
└── IWashProductConsumptionRepository.cs
```

### Infrastructure (18 novos arquivos)

**EF Configurations (9 arquivos):**
- ServiceCategoryConfiguration.cs
- ServiceItemConfiguration.cs
- ProductConfiguration.cs
- WashScheduleConfiguration.cs
- WashSessionConfiguration.cs
- WashEmployeeConfiguration.cs
- WashOperationalCostConfiguration.cs
- WashServiceRevenueConfiguration.cs
- WashProductConsumptionConfiguration.cs

**Repositories (9 arquivos):**
- ServiceCategoryRepository.cs
- ServiceItemRepository.cs
- ProductRepository.cs
- WashScheduleRepository.cs
- WashSessionRepository.cs
- WashEmployeeRepository.cs
- WashOperationalCostRepository.cs
- WashServiceRevenueRepository.cs
- WashProductConsumptionRepository.cs

### Application (35 novos arquivos)

**DTOs (4 arquivos):**
- ServiceCategoryDto.cs
- WashScheduleDto.cs
- ProductDto.cs
- WashOperationalCostDto.cs

**Commands (12 arquivos - 4 commands + handlers + validators):**
- CreateServiceCategoryCommand + Handler + Validator
- CreateProductCommand + Handler + Validator
- CreateWashScheduleCommand + Handler + Validator
- AssignWashEmployeeCommand + Handler + Validator
- RecordProductConsumptionCommand + Handler + Validator
- GenerateMonthlyReportCommand + Handler + Validator

**Queries (12 arquivos - 4 queries + handlers):**
- GetServiceItemsQuery + Handler
- GetWashScheduleQuery + Handler
- GetProductStockQuery + Handler
- GetMonthlyMetricsQuery + Handler

### API (4 novos arquivos)

```
Parking.API/Controllers/
├── ServiceController.cs
├── WashScheduleController.cs
├── ProductController.cs
└── WashReportsController.cs
```

### Tests (9 novos arquivos)

```
Parking.Tests/Handlers/
├── CreateServiceCategoryCommandHandlerTests.cs
├── CreateWashScheduleCommandHandlerTests.cs
├── GenerateMonthlyReportCommandHandlerTests.cs
└── GetMonthlyMetricsQueryHandlerTests.cs

Parking.Specs/Features/
├── WashService.feature
└── ProductInventory.feature
```

---

## 📊 Total: ~95 novos arquivos

---

## 🔄 Ordem de Implementação

### Dia 1: Domain — Service Catalog
```
1. ServiceCategory.cs (AggregateRoot)
   - Name, Description, IsActive

2. ServiceItem.cs (Entity)
   - ServiceCategoryId, Name, Duration, BaseCost

3. Product.cs (AggregateRoot)
   - Name, SKU, Category, Cost, SellingPrice, Stock

4. ValueObjects: Duration.cs, Cost.cs

5. Repositories: IServiceCategoryRepository, IServiceItemRepository, IProductRepository

Compile: dotnet build Parking.Domain
```

### Dia 2: Domain — Scheduling + Workforce
```
1. WashSchedule.cs (AggregateRoot)
   - VehicleEntryId, ScheduledTime, EmployeeId, Status

2. WashSession.cs (Entity)
   - WashScheduleId, ServiceItems list, TotalCost, TotalDuration

3. WashEmployee.cs (Entity)
   - EmployeeId, Specializations, TrainingLevel

4. Repositories: IWashScheduleRepository, IWashSessionRepository, IWashEmployeeRepository
```

### Dia 3: Domain — Cost Tracking
```
1. WashOperationalCost.cs (AggregateRoot)
   - BranchId, MonthYear, LaborCost, MaterialCost, TotalRevenue

2. WashServiceRevenue.cs (Entity)
   - WashScheduleId, ServiceItemId, Revenue

3. WashProductConsumption.cs (Entity)
   - WashScheduleId, ProductId, QuantityUsed, Cost

4. Repositories: IWashOperationalCostRepository, IWashServiceRevenueRepository, IWashProductConsumptionRepository
```

### Dia 4: Infrastructure — All EF Configs + Repositories
```
1. Add 9 EF Configurations
2. Implement 9 Repositories
3. Update AppDbContext with 9 new DbSets
4. Update DependencyInjection.cs

Compile: dotnet build Parking.Infrastructure

Create migration:
dotnet ef migrations add Fase3_LavaRapido -p Parking.Infrastructure -s Parking.API
```

### Dia 5-6: Application — Commands + Queries
```
Commands (6):
- CreateServiceCategoryCommand + Handler + Validator
- CreateProductCommand + Handler + Validator
- CreateWashScheduleCommand + Handler + Validator
- AssignWashEmployeeCommand + Handler + Validator
- RecordProductConsumptionCommand + Handler + Validator
- GenerateMonthlyReportCommand + Handler + Validator

Queries (4):
- GetServiceItemsQuery + Handler
- GetWashScheduleQuery + Handler
- GetProductStockQuery + Handler
- GetMonthlyMetricsQuery + Handler

Compile: dotnet build Parking.Application
```

### Dia 7-8: API — Controllers + Tests
```
Controllers:
- ServiceController.cs
- WashScheduleController.cs
- ProductController.cs
- WashReportsController.cs

Compile: dotnet build Parking.API
```

### Dia 9-10: Tests + Validation
```
1. Unit Tests (4 handlers)
2. BDD Features (2 features)
3. dotnet test
4. Validate endpoints
```

---

## 🧪 Test Endpoints (After Implementation)

```bash
# Create Service Category
POST /api/services/categories
{
  "branchId": 1,
  "name": "Lavagem Rápida",
  "description": "Lavagem externa rápida"
}

# Create Service Item
POST /api/services/items
{
  "serviceCategoryId": 1,
  "name": "Exterior",
  "durationMinutes": 15,
  "baseCost": 30.00
}

# Add Product (inventory)
POST /api/products
{
  "branchId": 1,
  "name": "Shampoo Automotivo",
  "sku": "SHMP-001",
  "category": "Chemicals",
  "cost": 15.00,
  "sellingPrice": 20.00,
  "stock": 100.00
}

# Create Wash Schedule
POST /api/wash-schedules
{
  "branchId": 1,
  "vehicleEntryId": 1,
  "scheduledTime": "2026-07-15T14:30:00Z",
  "serviceItemIds": [1],
  "employeeId": 1
}

# Record Product Consumption
POST /api/wash-sessions/1/products
{
  "productId": 1,
  "quantityUsed": 0.250
}

# Get Monthly Metrics
GET /api/wash-reports/monthly?branchId=1&monthYear=2026-07

# Response: {
#   "totalServices": 45,
#   "totalRevenue": 2250.00,
#   "totalLaborCost": 450.00,
#   "totalMaterialCost": 300.00,
#   "netProfit": 1500.00,
#   "profitMargin": 66.7%
# }
```

---

## ✅ Success Checklist (End of Fase 3)

- [ ] All 18 Domain files created
- [ ] All 18 Infrastructure files created
- [ ] All 35 Application files created
- [ ] All 4 API controllers created
- [ ] Migration created and applied
- [ ] POST /api/services/categories works
- [ ] POST /api/services/items works
- [ ] POST /api/products works
- [ ] POST /api/wash-schedules works
- [ ] GET /api/wash-reports/monthly works
- [ ] Product stock updates correctly
- [ ] Revenue tracked per service
- [ ] Monthly report calculates profit
- [ ] All tests pass

---

## 🎯 Próximas Fases

**Fase 4 — MVP:** Complete billing + integrated reports  
**Fase 5+:** Mobile app, advanced analytics

---

**Ready? Let's build Fase 3!** 🚀

