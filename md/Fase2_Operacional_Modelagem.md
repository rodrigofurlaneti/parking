# Fase 2 — Operacional (Employees + Cash + Parking Spaces)

**Status:** 🟢 Ready to Implement  
**Tempo Estimado:** 2 sprints (10 dias)  
**Dependências:** Fase 1 (Auth/Org) ✅ Completo

---

## 📊 Domínios Novos

### 1. EMPLOYEES (Funcionários)
Gerenciar funcionários com turnos, salários, permissões específicas.

**Entities:**
- `Employee` (AggregateRoot)
  - Id, CompanyId, BranchId
  - Name, Email, Phone, CPF
  - HireDate, TerminationDate (soft delete)
  - Role (FK to Role)
  - IsActive, CreatedAt, UpdatedAt

- `EmployeeSchedule` (Entity)
  - EmployeeId (FK)
  - DayOfWeek (Monday-Sunday)
  - StartTime, EndTime
  - IsActive

- `EmployeePayroll` (Entity)
  - EmployeeId (FK)
  - MonthYear
  - BaseSalary, Bonuses, Deductions
  - Status (Draft, Approved, Paid)
  - PaidDate

**Value Objects:**
- `CPF` — Validação 11 dígitos
- `TimeSlot` — StartTime, EndTime com validação

**Repository Interfaces:**
- IEmployeeRepository
- IEmployeeScheduleRepository
- IEmployeePayrollRepository

---

### 2. CASH REGISTER (Caixa)
Gerenciar movimentações de caixa, fechamentos, auditoria.

**Entities:**
- `CashRegister` (AggregateRoot)
  - Id, BranchId, EmployeeId (FK)
  - OpenedAt, ClosedAt
  - OpeningBalance, ClosingBalance
  - Status (Open, Closed)
  - IsActive

- `CashMovement` (Entity)
  - CashRegisterId (FK)
  - Type (Entry, Exit, Adjustment) → CashMovementType enum
  - Amount, Currency (BRL)
  - Description
  - CreatedAt
  - ReferencedDocumentType, ReferencedDocumentId (para rastreabilidade)

- `CashMovementType` (Lookup/Seed)
  - 1 = Entry (Entrada)
  - 2 = Exit (Saída)
  - 3 = Adjustment (Ajuste)

**Value Objects:**
- `Money` — Amount + Currency com validação (>= 0)

**Repository Interfaces:**
- ICashRegisterRepository
- ICashMovementRepository

---

### 3. PARKING SPACES (Vagas)
Gerenciar vagas de estacionamento com tipos e disponibilidade.

**Entities:**
- `ParkingSpace` (AggregateRoot)
  - Id, BranchId (FK)
  - SpaceNumber (e.g., "A-001", "B-015")
  - ParkingSpaceType (Covered, Uncovered, Reserved, Handicap)
  - Status (Available, Occupied, Maintenance)
  - IsActive

- `ParkingSpaceType` (Lookup/Seed)
  - 1 = Covered (Coberta)
  - 2 = Uncovered (Descoberta)
  - 3 = Reserved (Reservada)
  - 4 = Handicap (Deficientes)

- `VehicleEntry` (AggregateRoot)
  - Id, BranchId, ParkingSpaceId (FK)
  - CustomerId (FK)
  - Vehicle (LicensePlate, Model, Color)
  - EntryTime
  - ExitTime (nullable)
  - Status (Parked, Exited)
  - DurationMinutes
  - IsActive

- `VehicleExit` (Entity)
  - VehicleEntryId (FK)
  - ExitTime
  - DurationMinutes
  - TotalAmount
  - ParkingMode (Rotativo, Agreement, Monthly)

**Value Objects:**
- `LicensePlate` — Validação placa brasileira (ABC-1234)
- `DurationMinutes` — Validação minutos > 0

**Repository Interfaces:**
- IParkingSpaceRepository
- IVehicleEntryRepository
- IVehicleExitRepository

---

## 🔄 Fluxos de Negócio

### Employee Workflow
```
1. Create Employee
   ↓
2. Assign to Schedule (Daily: DayOfWeek + TimeSlot)
   ↓
3. Each Month → Generate Payroll
   ↓
4. Manager approves payroll
   ↓
5. Mark as Paid
```

### Cash Register Workflow
```
1. Open Register (OpeningBalance + Employee)
   ↓
2. Throughout day → Record CashMovements (Entry/Exit/Adjustment)
   ↓
3. At end of day → Calculate total (OpeningBalance + Sum of movements)
   ↓
4. Close Register (ClosingBalance)
   ↓
5. Audit: Verify OpeningBalance + Movements = ClosingBalance
```

### Parking Space & Vehicle Entry/Exit Workflow
```
1. Setup ParkingSpaces in Branch (e.g., "A-001", "A-002", etc)
   ↓
2. Vehicle Arrives
   ├─ Create VehicleEntry (CurrentTime, ParkingSpace)
   ├─ Mark ParkingSpace as Occupied
   └─ Store: LicensePlate, Model, Color

3. Vehicle Parked
   └─ Wait for exit

4. Vehicle Exits
   ├─ Create VehicleExit (ExitTime, DurationMinutes)
   ├─ Calculate Bill based on customer type:
   │  ├─ Rotativo: Premium 1st hour + hourly rate
   │  ├─ Agreement: Partner discount
   │  └─ Monthly: Flat fee (no additional charge)
   ├─ Record CashMovement (Entry from parking payment)
   ├─ Mark ParkingSpace as Available
   └─ Status = Exited
```

---

## 📐 Data Model Additions

### New Tables (9 tables)
```
employees
├─ id (BIGINT PK)
├─ company_id (FK)
├─ branch_id (FK)
├─ name (NVARCHAR(255))
├─ email (NVARCHAR(255) Unique per Company)
├─ phone (NVARCHAR(15))
├─ cpf (NVARCHAR(11) Unique)
├─ hire_date (DATETIME2)
├─ termination_date (DATETIME2 nullable)
├─ role_id (FK to roles)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

employee_schedules
├─ id (BIGINT PK)
├─ employee_id (FK)
├─ day_of_week (INT: 0-6)
├─ start_time (TIME)
├─ end_time (TIME)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

employee_payrolls
├─ id (BIGINT PK)
├─ employee_id (FK)
├─ month_year (DATE)
├─ base_salary (DECIMAL 18,2)
├─ bonuses (DECIMAL 18,2 default 0)
├─ deductions (DECIMAL 18,2 default 0)
├─ status (INT: Draft=0, Approved=1, Paid=2)
├─ paid_date (DATETIME2 nullable)
└─ [CreatedAt, UpdatedAt]

cash_registers
├─ id (BIGINT PK)
├─ branch_id (FK)
├─ employee_id (FK - who opened)
├─ opened_at (DATETIME2)
├─ closed_at (DATETIME2 nullable)
├─ opening_balance (DECIMAL 18,2)
├─ closing_balance (DECIMAL 18,2 nullable)
├─ status (INT: Open=0, Closed=1)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

cash_movements
├─ id (BIGINT PK)
├─ cash_register_id (FK)
├─ type (INT: Entry=1, Exit=2, Adjustment=3)
├─ amount (DECIMAL 18,2)
├─ description (NVARCHAR(255))
├─ referenced_doc_type (INT nullable)
├─ referenced_doc_id (BIGINT nullable)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

parking_spaces
├─ id (BIGINT PK)
├─ branch_id (FK)
├─ space_number (NVARCHAR(10) Unique per Branch)
├─ type (INT: Covered=1, Uncovered=2, Reserved=3, Handicap=4)
├─ status (INT: Available=0, Occupied=1, Maintenance=2)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

vehicle_entries
├─ id (BIGINT PK)
├─ branch_id (FK)
├─ parking_space_id (FK)
├─ customer_id (FK)
├─ license_plate (NVARCHAR(10))
├─ vehicle_model (NVARCHAR(100))
├─ vehicle_color (NVARCHAR(50))
├─ entry_time (DATETIME2)
├─ exit_time (DATETIME2 nullable)
├─ status (INT: Parked=0, Exited=1)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

vehicle_exits
├─ id (BIGINT PK)
├─ vehicle_entry_id (FK)
├─ exit_time (DATETIME2)
├─ duration_minutes (INT)
├─ total_amount (DECIMAL 18,2)
├─ parking_mode (INT: Rotativo=1, Agreement=2, Monthly=3)
├─ is_active (BIT)
└─ [CreatedAt, UpdatedAt]

parking_space_occupancy (View or calculated field)
├─ branch_id
├─ total_spaces (COUNT)
├─ available_spaces (COUNT where status=Available)
├─ occupied_spaces (COUNT where status=Occupied)
├─ occupancy_rate (occupied/total)
```

---

## 🎯 Features por Sprint

### Sprint 1 (Dias 1-5): Employees + Cash Register
1. **Domain:** Employee, EmployeeSchedule, EmployeePayroll entities + VOs
2. **Domain:** CashRegister, CashMovement entities + Money VO
3. **Infrastructure:** EF configs + Repositories (Employee, CashRegister)
4. **Application:** Commands (CreateEmployee, OpenCashRegister, RecordCashMovement)
5. **Application:** Queries (GetEmployeeSchedule, GetCashRegisterBalance)
6. **API:** EmployeeController, CashRegisterController

### Sprint 2 (Dias 6-10): Parking Spaces + Vehicle Entry/Exit
1. **Domain:** ParkingSpace, VehicleEntry, VehicleExit entities + VOs
2. **Infrastructure:** EF configs + Repositories (ParkingSpace, VehicleEntry)
3. **Application:** Commands (CreateParkingSpace, RegisterVehicleEntry, RegisterVehicleExit)
4. **Application:** Queries (GetParkingSpaceOccupancy, GetVehicleEntry)
5. **API:** ParkingSpaceController, VehicleEntryController
6. **Tests & Integration**

---

## 📋 Success Criteria (End of Fase 2)

✅ **Functional**
- POST /api/employees → create employee
- GET /api/employees/{id} → get employee with schedule
- POST /api/cash-registers → open register
- POST /api/cash-movements → record movement
- GET /api/cash-registers/{id}/balance → total balance
- POST /api/parking-spaces → create space
- GET /api/parking-spaces?branchId=X → list spaces with occupancy
- POST /api/vehicle-entries → vehicle arrived
- POST /api/vehicle-exits → vehicle left + calculate bill

✅ **Business Logic**
- Employee scheduling validates no time overlaps
- Cash register balance = OpeningBalance + Sum(Movements)
- Parking space occupancy updated in real-time
- Vehicle exit calculates bill based on customer type (Rotativo/Agreement/Monthly)

✅ **Architecture**
- All new domains follow DDD patterns
- Handlers are `internal sealed`
- Tests passing (unit + integration)

---

## 📚 Reference

**Previous:** Fase 1 (Auth/Org) ✅  
**Current:** Fase 2 (Employees/Cash/Spaces) 🟢  
**Next:** Fase 3 (Car Wash Services)

