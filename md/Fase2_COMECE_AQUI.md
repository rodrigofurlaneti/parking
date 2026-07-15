# 🚀 Fase 2 — Operacional (Employees + Cash + Parking Spaces)

**Status:** ✅ Ready to Build  
**Tempo Estimado:** 10 dias (2 sprints)  
**Dependência:** Fase 1 ✅ Completo

---

## 📋 O Que Você Vai Implementar

### Sprint 1 (Dias 1-5): Employees + Cash Register
- 15 novos arquivos no Domain (Employees, CashRegister entities + VOs)
- 10 EF Configurations + 5 Repositories
- 10 Commands + 5 Queries
- 2 Controllers (Employee, CashRegister)
- Tests

### Sprint 2 (Dias 6-10): Parking Spaces + Vehicle Entry/Exit
- 12 novos arquivos no Domain (ParkingSpace, VehicleEntry entities + VOs)
- 6 EF Configurations + 3 Repositories
- 8 Commands + 4 Queries
- 2 Controllers (ParkingSpace, VehicleEntry)
- Tests + Integration

---

## 📂 Arquivos Que Você Vai Criar

### Domain (27 novos arquivos)

**Employees (9 arquivos):**
```
Parking.Domain/Entities/
├── Employee.cs (AggregateRoot)
├── EmployeeSchedule.cs (Entity)
└── EmployeePayroll.cs (Entity)

Parking.Domain/ValueObjects/
├── CPF.cs (validação 11 dígitos)
└── TimeSlot.cs (StartTime, EndTime)

Parking.Domain/Repositories/
├── IEmployeeRepository.cs
├── IEmployeeScheduleRepository.cs
└── IEmployeePayrollRepository.cs
```

**CashRegister (6 arquivos):**
```
Parking.Domain/Entities/
├── CashRegister.cs (AggregateRoot)
└── CashMovement.cs (Entity)

Parking.Domain/ValueObjects/
└── Money.cs (Amount + Currency)

Parking.Domain/Repositories/
├── ICashRegisterRepository.cs
└── ICashMovementRepository.cs
```

**ParkingSpaces (12 arquivos):**
```
Parking.Domain/Entities/
├── ParkingSpace.cs (AggregateRoot)
├── VehicleEntry.cs (AggregateRoot)
└── VehicleExit.cs (Entity)

Parking.Domain/ValueObjects/
├── LicensePlate.cs (validação placa)
└── DurationMinutes.cs (validação minutos)

Parking.Domain/Repositories/
├── IParkingSpaceRepository.cs
├── IVehicleEntryRepository.cs
└── IVehicleExitRepository.cs
```

### Infrastructure (24 novos arquivos)

**Employees (9 arquivos):**
```
Parking.Infrastructure/Persistence/Configurations/
├── EmployeeConfiguration.cs
├── EmployeeScheduleConfiguration.cs
└── EmployeePayrollConfiguration.cs

Parking.Infrastructure/Persistence/Repositories/
├── EmployeeRepository.cs
├── EmployeeScheduleRepository.cs
└── EmployeePayrollRepository.cs
```

**CashRegister (6 arquivos):**
```
Parking.Infrastructure/Persistence/Configurations/
├── CashRegisterConfiguration.cs
└── CashMovementConfiguration.cs

Parking.Infrastructure/Persistence/Repositories/
├── CashRegisterRepository.cs
└── CashMovementRepository.cs
```

**ParkingSpaces (9 arquivos):**
```
Parking.Infrastructure/Persistence/Configurations/
├── ParkingSpaceConfiguration.cs
├── VehicleEntryConfiguration.cs
└── VehicleExitConfiguration.cs

Parking.Infrastructure/Persistence/Repositories/
├── ParkingSpaceRepository.cs
├── VehicleEntryRepository.cs
└── VehicleExitRepository.cs
```

### Application (30 novos arquivos)

**Employees (10 arquivos):**
- CreateEmployeeCommand + Handler + Validator
- AssignScheduleCommand + Handler + Validator
- GeneratePayrollCommand + Handler + Validator
- GetEmployeeScheduleQuery + Handler
- GetPayrollQuery + Handler

**CashRegister (8 arquivos):**
- OpenCashRegisterCommand + Handler + Validator
- RecordCashMovementCommand + Handler + Validator
- CloseCashRegisterCommand + Handler + Validator
- GetCashRegisterBalanceQuery + Handler

**ParkingSpaces (12 arquivos):**
- CreateParkingSpaceCommand + Handler + Validator
- RegisterVehicleEntryCommand + Handler + Validator
- RegisterVehicleExitCommand + Handler + Validator
- GetParkingSpaceOccupancyQuery + Handler
- GetVehicleEntryQuery + Handler

### API (6 novos arquivos)

```
Parking.API/Controllers/
├── EmployeeController.cs
├── CashRegisterController.cs
├── ParkingSpaceController.cs
└── VehicleEntryController.cs
```

**New DTOs (8 arquivos):**
```
Parking.Application/Features/Common/DTOs/
├── EmployeeDto.cs
├── CashRegisterDto.cs
├── CashMovementDto.cs
├── ParkingSpaceDto.cs
├── VehicleEntryDto.cs
└── VehicleExitDto.cs
```

### Tests (10 novos arquivos)

```
Parking.Tests/Handlers/
├── CreateEmployeeCommandHandlerTests.cs
├── OpenCashRegisterCommandHandlerTests.cs
├── RegisterVehicleEntryCommandHandlerTests.cs
└── GetParkingSpaceOccupancyQueryHandlerTests.cs

Parking.Specs/Features/
├── Employee.feature
├── CashRegister.feature
└── ParkingSpace.feature
```

---

## 📊 Total: ~100+ novos arquivos

---

## 🔄 Ordem de Implementação

### Dia 1-2: Domain — Employees
```
1. Employee.cs (AggregateRoot)
   - Id, Name, Email, Phone, CPF, HireDate, TerminationDate
   - Methods: Create(), TerminateEmployee()

2. EmployeeSchedule.cs (Entity)
   - EmployeeId, DayOfWeek, StartTime, EndTime

3. EmployeePayroll.cs (Entity)
   - EmployeeId, MonthYear, BaseSalary, Bonuses, Deductions, Status

4. ValueObjects: CPF.cs, TimeSlot.cs

5. Repositories: IEmployeeRepository, IEmployeeScheduleRepository, IEmployeePayrollRepository

Test: dotnet build Parking.Domain
```

### Dia 3: Domain — CashRegister
```
1. CashRegister.cs (AggregateRoot)
   - Id, BranchId, EmployeeId, OpenedAt, ClosedAt, OpeningBalance, ClosingBalance, Status

2. CashMovement.cs (Entity)
   - CashRegisterId, Type (Entry/Exit/Adjustment), Amount

3. ValueObject: Money.cs (validação Amount >= 0)

4. Repositories: ICashRegisterRepository, ICashMovementRepository
```

### Dia 4: Domain — ParkingSpaces
```
1. ParkingSpace.cs (AggregateRoot)
   - Id, BranchId, SpaceNumber, Type, Status

2. VehicleEntry.cs (AggregateRoot)
   - Id, ParkingSpaceId, CustomerId, LicensePlate, EntryTime, ExitTime, Status

3. VehicleExit.cs (Entity)
   - VehicleEntryId, ExitTime, DurationMinutes, TotalAmount

4. ValueObjects: LicensePlate.cs, DurationMinutes.cs

5. Repositories: IParkingSpaceRepository, IVehicleEntryRepository, IVehicleExitRepository

Test: dotnet build Parking.Domain
```

### Dia 5: Infrastructure — All Configurations + Repositories
```
1. Add all 9 EF Configurations (Employee, CashRegister, ParkingSpace)
2. Implement all 8 Repositories
3. Update AppDbContext with new DbSets
4. Update DependencyInjection.cs to register new repos

Compile: dotnet build Parking.Infrastructure

Create migration:
dotnet ef migrations add Fase2_Operacional -p Parking.Infrastructure -s Parking.API
```

### Dia 6-7: Application — Commands + Queries
```
Sprint 1 Commands (6):
- CreateEmployeeCommand + Handler + Validator
- AssignScheduleCommand + Handler + Validator
- GeneratePayrollCommand + Handler + Validator
- OpenCashRegisterCommand + Handler + Validator
- RecordCashMovementCommand + Handler + Validator
- CloseCashRegisterCommand + Handler + Validator

Sprint 2 Commands (3):
- CreateParkingSpaceCommand + Handler + Validator
- RegisterVehicleEntryCommand + Handler + Validator
- RegisterVehicleExitCommand + Handler + Validator

Queries (6):
- GetEmployeeScheduleQuery + Handler
- GetPayrollQuery + Handler
- GetCashRegisterBalanceQuery + Handler
- GetParkingSpaceOccupancyQuery + Handler
- GetVehicleEntryQuery + Handler
- GetParkingSpaceDetailsQuery + Handler

DTOs (8):
- EmployeeDto, CashRegisterDto, CashMovementDto
- ParkingSpaceDto, VehicleEntryDto, VehicleExitDto

Compile: dotnet build Parking.Application
```

### Dia 8: API — Controllers
```
1. EmployeeController.cs
   - POST /api/employees (CreateEmployee)
   - GET /api/employees/{id}
   - POST /api/employees/{id}/schedule (AssignSchedule)
   - POST /api/employees/{id}/payroll (GeneratePayroll)

2. CashRegisterController.cs
   - POST /api/cash-registers (OpenRegister)
   - POST /api/cash-registers/{id}/movements (RecordMovement)
   - POST /api/cash-registers/{id}/close (CloseRegister)
   - GET /api/cash-registers/{id}/balance (GetBalance)

3. ParkingSpaceController.cs
   - POST /api/parking-spaces (Create)
   - GET /api/parking-spaces?branchId=X (List with occupancy)
   - GET /api/parking-spaces/{id}

4. VehicleEntryController.cs
   - POST /api/vehicle-entries (RegisterEntry)
   - POST /api/vehicle-exits (RegisterExit + calculate bill)
   - GET /api/vehicle-entries/{id}

Compile: dotnet build Parking.API
```

### Dia 9-10: Tests + Validation
```
1. Unit Tests (4 handlers)
2. BDD Features (3 features)
3. Integration tests
4. Run: dotnet test
5. Validate endpoints with Postman/curl
```

---

## 🧪 Test Endpoints (After Implementation)

```bash
# Create Employee
POST /api/employees
{
  "name": "João Silva",
  "email": "joao@parking.com",
  "phone": "11987654321",
  "cpf": "12345678901",
  "companyId": 1,
  "branchId": 1
}

# Open Cash Register
POST /api/cash-registers
{
  "branchId": 1,
  "employeeId": 1,
  "openingBalance": 1000.00
}

# Record Cash Movement
POST /api/cash-registers/1/movements
{
  "type": 1,  # Entry
  "amount": 50.00,
  "description": "Parking payment"
}

# Create Parking Space
POST /api/parking-spaces
{
  "branchId": 1,
  "spaceNumber": "A-001",
  "type": 1  # Covered
}

# Register Vehicle Entry
POST /api/vehicle-entries
{
  "parkingSpaceId": 1,
  "customerId": 1,
  "licensePlate": "ABC-1234",
  "vehicleModel": "Toyota Corolla",
  "vehicleColor": "White"
}

# Register Vehicle Exit (calculates bill)
POST /api/vehicle-exits
{
  "vehicleEntryId": 1,
  "parkingMode": 1  # Rotativo
}

# Get Parking Occupancy
GET /api/parking-spaces?branchId=1
# Returns: total_spaces, available, occupied, occupancy_rate
```

---

## ✅ Success Checklist (End of Fase 2)

- [ ] `dotnet build` compila sem erros
- [ ] `dotnet ef database update` cria novas tabelas
- [ ] POST /api/employees funciona
- [ ] POST /api/cash-registers + POST /movements funciona
- [ ] GET cash balance calcula corretamente
- [ ] POST /api/vehicle-entries + /exits funciona
- [ ] Vehicle exit calcula bill baseado em customer type
- [ ] Parking occupancy retorna dados corretos
- [ ] Todos os testes passam
- [ ] Architecture rules validados

---

## 🎯 Próximas Fases

**Fase 3 — Serviços:** Car Wash (Lava Rápido) + Products + Scheduling  
**Fase 4 — MVP:** Complete billing + reports  
**Fase 5+:** Mobile app, advanced features

---

**Ready? Vamos começar!** 🚀

Use este arquivo como checklist e vá marcando cada seção conforme implementa.

