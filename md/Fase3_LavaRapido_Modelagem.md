# Fase 3 — Lava Rápido (Car Wash Services)

**Status:** 🟢 Ready to Implement  
**Tempo Estimado:** 2 sprints (10 dias)  
**Dependências:** Fase 1 + Fase 2 ✅ Completo

---

## 📊 Novo Domínio: Lava Rápido

Serviço de limpeza de veículos integrado ao estacionamento. Sistema de agendamento, produtos/serviços, funcionários dedicados, e custos operacionais.

---

## 🏢 Estrutura Lava Rápido

### 1. SERVICE CATALOG (Catálogo de Serviços)

**Entities:**
- `ServiceCategory` (AggregateRoot)
  - Id, BranchId, Name (e.g., "Lavagem Rápida", "Lavagem Completa")
  - Description, IsActive

- `ServiceItem` (Entity)
  - ServiceCategoryId (FK)
  - Name, Description, Duration (minutos)
  - BaseCost (valor base), IsActive

- `Product` (AggregateRoot)
  - BranchId, Name, SKU
  - Category, Cost, SellingPrice
  - Stock (quantidade), Supplier info
  - IsActive

**Value Objects:**
- `Duration` — minutos com validação (>0)
- `Cost` — custo com validação (>=0)

**Repository Interfaces:**
- IServiceCategoryRepository
- IServiceItemRepository
- IProductRepository

---

### 2. SCHEDULING (Agendamentos)

**Entities:**
- `WashSchedule` (AggregateRoot)
  - Id, BranchId, VehicleEntryId (FK)
  - ScheduledTime, ActualStartTime, ActualEndTime
  - Status (Scheduled, InProgress, Completed, Cancelled)
  - Employee assigned (EmployeeId FK)
  - IsActive

- `WashSession` (Entity)
  - WashScheduleId (FK)
  - ServiceItems (List of ServiceItem)
  - Products used (List of Product with quantity)
  - TotalCost, TotalDuration
  - StartTime, EndTime, Status
  - Notes

**Value Objects:**
- `ScheduleSlot` — Time slot validation

**Repository Interfaces:**
- IWashScheduleRepository
- IWashSessionRepository

---

### 3. WORKFORCE (Equipe Lava Rápido)

**Entities:**
- `WashEmployee` (Entity extending Employee)
  - Base employee + specializations
  - Certifications, TrainingLevel
  - AssignedToServiceCategories (Many-to-many)
  - IsActive

- `WashEmployeeSchedule` (Entity)
  - WashEmployeeId (FK)
  - AvailableSlots (Monday-Sunday, timeframe)
  - Capacity (max wash sessions per day)

**Repository Interfaces:**
- IWashEmployeeRepository
- IWashEmployeeScheduleRepository

---

### 4. COST & REVENUE TRACKING

**Entities:**
- `WashServiceRevenue` (Entity)
  - WashScheduleId (FK)
  - ServiceItemId (FK)
  - Quantity, UnitPrice, TotalPrice
  - Commission (if employee commission)
  - Date, Status

- `WashProductConsumption` (Entity)
  - WashScheduleId (FK)
  - ProductId (FK)
  - QuantityUsed, UnitCost, TotalCost
  - Date

- `WashOperationalCost` (AggregateRoot)
  - BranchId, MonthYear
  - Labor costs (employee salaries)
  - Material costs (products)
  - Equipment depreciation
  - Utilities (water, electricity)
  - Total costs, Gross revenue, Net profit

**Repository Interfaces:**
- IWashServiceRevenueRepository
- IWashProductConsumptionRepository
- IWashOperationalCostRepository

---

## 🔄 Fluxos de Negócio

### Workflow 1: Criar Serviço de Lavagem

```
1. Veículo estacionado (VehicleEntry criado)
   ↓
2. Customer pede lavagem
   ↓
3. System sugere ServiceItems baseado em:
   - Vehicle type (car, SUV, truck)
   - Customer preference (quick wash, full wash)
   - Time available (duração vs tempo livre no estacionamento)
   ↓
4. Create WashSchedule
   - SelectServiceItems
   - SelectProducts needed
   - ScheduleTime (ASAP ou específico)
   - AssignEmployee
   ↓
5. WashSession inicia
   - Employee executa serviço
   - Tracks start/end time
   - Records products used
   ↓
6. WashSession completa
   - Calculate costs
   - Record revenue
   - Update product stock
   - Add to VehicleExit bill
   ↓
7. Payment collected (integrado com CashRegister)
```

### Workflow 2: Employee Assignment & Capacity

```
1. Manager views available WashEmployees
   - Filtered by: Availability, Certifications, Capacity
   ↓
2. Assign employee to WashSchedule
   - Check if available in ScheduledTime
   - Check if capacity not exceeded (e.g., max 5 washes/day)
   ↓
3. Employee starts WashSession
   - Clock in (actualStartTime)
   - Complete work
   - Clock out (actualEndTime)
   ↓
4. Revenue calculated
   - Service: $50
   - Products: $10 (cost) or $15 (selling if charged separately)
   - Employee commission: 20% of service = $10
   - Net: $50 - $10 (products) - $10 (commission) = $30 profit
```

### Workflow 3: Product & Cost Management

```
Daily flow:
1. Morning: Check product stock
   ↓
2. Throughout day: Log products used per wash
   - Each WashProductConsumption records quantity & cost
   ↓
3. Weekly: Purchase new products from suppliers
   ↓
4. Monthly: Generate WashOperationalCost report
   - Sum of all labor (employee salary)
   - Sum of all product consumption
   - Fixed costs (utilities, depreciation)
   - Revenue from all washes
   - Profit margin calculation
```

---

## 📐 Data Model Additions (11 new tables)

```
service_categories
├─ id (BIGINT PK)
├─ branch_id (FK)
├─ name (NVARCHAR(255))
├─ description (NVARCHAR(500))
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

service_items
├─ id (BIGINT PK)
├─ service_category_id (FK)
├─ name (NVARCHAR(255))
├─ description (NVARCHAR(500))
├─ duration_minutes (INT)
├─ base_cost (DECIMAL 18,2)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

products
├─ id (BIGINT PK)
├─ branch_id (FK)
├─ name (NVARCHAR(255))
├─ sku (NVARCHAR(50) Unique per branch)
├─ category (NVARCHAR(100))
├─ cost (DECIMAL 18,2)
├─ selling_price (DECIMAL 18,2)
├─ stock (DECIMAL 18,3)
├─ supplier_id (FK nullable)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

wash_schedules
├─ id (BIGINT PK)
├─ branch_id (FK)
├─ vehicle_entry_id (FK)
├─ scheduled_time (DATETIME2)
├─ actual_start_time (DATETIME2 nullable)
├─ actual_end_time (DATETIME2 nullable)
├─ employee_id (FK)
├─ status (INT: Scheduled=0, InProgress=1, Completed=2, Cancelled=3)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

wash_sessions
├─ id (BIGINT PK)
├─ wash_schedule_id (FK)
├─ total_cost (DECIMAL 18,2)
├─ total_duration_minutes (INT)
├─ start_time (DATETIME2)
├─ end_time (DATETIME2 nullable)
├─ status (INT)
├─ notes (NVARCHAR(500))
└─ [CreatedAt, UpdatedAt]

wash_session_items
├─ id (BIGINT PK)
├─ wash_session_id (FK)
├─ service_item_id (FK)
├─ unit_price (DECIMAL 18,2)
├─ quantity (INT default 1)
└─ [CreatedAt, UpdatedAt]

wash_session_products
├─ id (BIGINT PK)
├─ wash_session_id (FK)
├─ product_id (FK)
├─ quantity_used (DECIMAL 18,3)
├─ unit_cost (DECIMAL 18,2)
└─ [CreatedAt, UpdatedAt]

wash_employees
├─ id (BIGINT PK)
├─ employee_id (FK unique)
├─ specializations (NVARCHAR(500) nullable)
├─ certifications (NVARCHAR(500) nullable)
├─ training_level (INT)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

wash_service_revenue
├─ id (BIGINT PK)
├─ wash_schedule_id (FK)
├─ service_item_id (FK)
├─ quantity (INT default 1)
├─ unit_price (DECIMAL 18,2)
├─ total_price (DECIMAL 18,2)
├─ commission (DECIMAL 18,2)
├─ date (DATE)
└─ [CreatedAt, UpdatedAt]

wash_operational_costs
├─ id (BIGINT PK)
├─ branch_id (FK)
├─ month_year (DATE)
├─ labor_cost (DECIMAL 18,2)
├─ material_cost (DECIMAL 18,2)
├─ equipment_cost (DECIMAL 18,2)
├─ utilities_cost (DECIMAL 18,2)
├─ total_cost (DECIMAL 18,2)
├─ total_revenue (DECIMAL 18,2)
└─ [CreatedAt, UpdatedAt]
```

---

## 📋 Features por Sprint

### Sprint 1 (Dias 1-5): Service Catalog + Basic Scheduling
1. **Domain:** ServiceCategory, ServiceItem, Product entities
2. **Domain:** WashSchedule, WashSession entities
3. **Infrastructure:** EF configs + Repositories
4. **Application:** Commands (CreateServiceCategory, CreateServiceItem, CreateProduct, CreateWashSchedule)
5. **Application:** Queries (GetServiceItems, GetWashSchedule, GetAvailableSlots)
6. **API:** ServiceController, WashScheduleController

### Sprint 2 (Dias 6-10): Workforce + Cost Tracking
1. **Domain:** WashEmployee, WashOperationalCost entities
2. **Infrastructure:** EF configs + Repositories
3. **Application:** Commands (AssignWashEmployee, RecordProductConsumption, GenerateMonthlyReport)
4. **Application:** Queries (GetEmployeeAvailability, GetMonthlyMetrics)
5. **API:** WashEmployeeController, ReportsController
6. **Tests & Integration**

---

## 🎯 Success Criteria (End of Fase 3)

✅ **Functional**
- POST /api/service-categories → create category
- POST /api/service-items → create service
- POST /api/products → add product to inventory
- POST /api/wash-schedules → schedule wash
- POST /api/wash-sessions → start/complete wash
- GET /api/wash-schedules?branchId=X → list with employee availability
- GET /api/products/stock → inventory levels
- POST /api/operational-costs → generate monthly report
- GET /api/operational-costs/monthly?monthYear=X → profit/loss

✅ **Business Logic**
- WashSchedule respects employee capacity (max N per day)
- WashSession logs time and products used
- Product stock decreases with consumption
- Revenue tracked by service + employee commission
- Monthly report calculates gross revenue - costs = net profit

✅ **Integration**
- Wash cost added to VehicleExit bill
- Revenue integrated with CashRegister
- Employee earnings tracked for payroll

---

## 📚 Dependencies

- Employees entity (Fase 2)
- Product inventory concept
- Revenue tracking
- Time-based scheduling

---

**Previous:** Fase 1 (Auth) + Fase 2 (Ops) ✅  
**Current:** Fase 3 (Lava Rápido) 🟢  
**Next:** Fase 4 (MVP Reports & Billing)

