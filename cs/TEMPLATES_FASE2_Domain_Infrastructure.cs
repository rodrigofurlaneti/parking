/*
 * TEMPLATES_FASE2_Domain_Infrastructure.cs
 *
 * Copy-paste ready code for Fase 2 (Employees + Cash + ParkingSpaces)
 *
 * Structure:
 * 1. Domain Entities (9 classes)
 * 2. Value Objects (4 classes)
 * 3. Repository Interfaces (8 interfaces)
 * 4. EF Core Configurations (9 classes)
 * 5. Repository Implementations (8 classes)
 */

// ═══════════════════════════════════════════════════════════════════════════
// DOMAIN LAYER — EMPLOYEES
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Domain.Entities;

using Parking.Domain.Common;
using Parking.Domain.ValueObjects;

public sealed class Employee : AggregateRoot
{
    public long CompanyId { get; private set; }
    public long BranchId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string CPF { get; private set; } = null!;
    public DateTime HireDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public long RoleId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Employee() { }

    private Employee(
        long companyId,
        long branchId,
        string name,
        string email,
        string phone,
        string cpf,
        long roleId)
    {
        CompanyId = companyId;
        BranchId = branchId;
        Name = name;
        Email = email;
        Phone = phone;
        CPF = cpf;
        HireDate = DateTime.UtcNow;
        RoleId = roleId;
    }

    public static Result<Employee> Create(
        long companyId,
        long branchId,
        string name,
        string email,
        string phone,
        string cpf,
        long roleId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Employee>(new Error("Employee.InvalidName", "Name required."));

        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
            return Result.Failure<Employee>(new Error("Employee.InvalidCPF", "Valid CPF required."));

        return Result.Success(new Employee(companyId, branchId, name, email, phone, cpf, roleId));
    }

    public void Terminate()
    {
        TerminationDate = DateTime.UtcNow;
        IsActive = false;
    }
}

public sealed class EmployeeSchedule : Entity
{
    public long EmployeeId { get; private set; }
    public int DayOfWeek { get; private set; } // 0-6 (Monday-Sunday)
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public bool IsActive { get; private set; } = true;

    private EmployeeSchedule() { }

    private EmployeeSchedule(long employeeId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime)
    {
        EmployeeId = employeeId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<EmployeeSchedule> Create(
        long employeeId,
        int dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime)
    {
        if (dayOfWeek < 0 || dayOfWeek > 6)
            return Result.Failure<EmployeeSchedule>(
                new Error("EmployeeSchedule.InvalidDay", "Day of week must be 0-6."));

        if (startTime >= endTime)
            return Result.Failure<EmployeeSchedule>(
                new Error("EmployeeSchedule.InvalidTime", "Start time must be before end time."));

        return Result.Success(new EmployeeSchedule(employeeId, dayOfWeek, startTime, endTime));
    }
}

public sealed class EmployeePayroll : Entity
{
    public long EmployeeId { get; private set; }
    public DateTime MonthYear { get; private set; }
    public decimal BaseSalary { get; private set; }
    public decimal Bonuses { get; private set; } = 0m;
    public decimal Deductions { get; private set; } = 0m;
    public int Status { get; private set; } = 0; // 0=Draft, 1=Approved, 2=Paid
    public DateTime? PaidDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private EmployeePayroll() { }

    private EmployeePayroll(long employeeId, DateTime monthYear, decimal baseSalary)
    {
        EmployeeId = employeeId;
        MonthYear = monthYear;
        BaseSalary = baseSalary;
    }

    public static Result<EmployeePayroll> Create(long employeeId, DateTime monthYear, decimal baseSalary)
    {
        if (baseSalary <= 0)
            return Result.Failure<EmployeePayroll>(
                new Error("EmployeePayroll.InvalidSalary", "Salary must be greater than 0."));

        return Result.Success(new EmployeePayroll(employeeId, monthYear, baseSalary));
    }

    public decimal GetTotalAmount() => BaseSalary + Bonuses - Deductions;

    public void Approve() => Status = 1;
    public void MarkAsPaid() => Status = 2;
}

// ═══════════════════════════════════════════════════════════════════════════
// DOMAIN LAYER — CASH REGISTER
// ═══════════════════════════════════════════════════════════════════════════

public sealed class CashRegister : AggregateRoot
{
    public long BranchId { get; private set; }
    public long EmployeeId { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public decimal OpeningBalance { get; private set; }
    public decimal ClosingBalance { get; private set; }
    public int Status { get; private set; } = 0; // 0=Open, 1=Closed
    public bool IsActive { get; private set; } = true;

    private CashRegister() { }

    private CashRegister(long branchId, long employeeId, decimal openingBalance)
    {
        BranchId = branchId;
        EmployeeId = employeeId;
        OpeningBalance = openingBalance;
        OpenedAt = DateTime.UtcNow;
    }

    public static Result<CashRegister> Create(long branchId, long employeeId, decimal openingBalance)
    {
        if (openingBalance < 0)
            return Result.Failure<CashRegister>(
                new Error("CashRegister.InvalidBalance", "Opening balance cannot be negative."));

        return Result.Success(new CashRegister(branchId, employeeId, openingBalance));
    }

    public void Close(decimal closingBalance)
    {
        ClosingBalance = closingBalance;
        ClosedAt = DateTime.UtcNow;
        Status = 1;
    }
}

public sealed class CashMovement : Entity
{
    public long CashRegisterId { get; private set; }
    public int Type { get; private set; } // 1=Entry, 2=Exit, 3=Adjustment
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = null!;
    public int? ReferencedDocumentType { get; private set; }
    public long? ReferencedDocumentId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private CashMovement() { }

    private CashMovement(long cashRegisterId, int type, decimal amount, string description)
    {
        CashRegisterId = cashRegisterId;
        Type = type;
        Amount = amount;
        Description = description;
    }

    public static Result<CashMovement> Create(
        long cashRegisterId,
        int type,
        decimal amount,
        string description)
    {
        if (amount <= 0)
            return Result.Failure<CashMovement>(
                new Error("CashMovement.InvalidAmount", "Amount must be greater than 0."));

        if (type < 1 || type > 3)
            return Result.Failure<CashMovement>(
                new Error("CashMovement.InvalidType", "Type must be 1 (Entry), 2 (Exit), or 3 (Adjustment)."));

        return Result.Success(new CashMovement(cashRegisterId, type, amount, description));
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// DOMAIN LAYER — PARKING SPACES
// ═══════════════════════════════════════════════════════════════════════════

public sealed class ParkingSpace : AggregateRoot
{
    public long BranchId { get; private set; }
    public string SpaceNumber { get; private set; } = null!;
    public int Type { get; private set; } // 1=Covered, 2=Uncovered, 3=Reserved, 4=Handicap
    public int Status { get; private set; } = 0; // 0=Available, 1=Occupied, 2=Maintenance
    public bool IsActive { get; private set; } = true;

    private ParkingSpace() { }

    private ParkingSpace(long branchId, string spaceNumber, int type)
    {
        BranchId = branchId;
        SpaceNumber = spaceNumber;
        Type = type;
    }

    public static Result<ParkingSpace> Create(long branchId, string spaceNumber, int type)
    {
        if (string.IsNullOrWhiteSpace(spaceNumber))
            return Result.Failure<ParkingSpace>(
                new Error("ParkingSpace.InvalidNumber", "Space number required."));

        if (type < 1 || type > 4)
            return Result.Failure<ParkingSpace>(
                new Error("ParkingSpace.InvalidType", "Type must be 1-4."));

        return Result.Success(new ParkingSpace(branchId, spaceNumber, type));
    }

    public void MarkAsOccupied() => Status = 1;
    public void MarkAsAvailable() => Status = 0;
    public void MarkAsMaintenance() => Status = 2;
}

public sealed class VehicleEntry : AggregateRoot
{
    public long BranchId { get; private set; }
    public long ParkingSpaceId { get; private set; }
    public long CustomerId { get; private set; }
    public string LicensePlate { get; private set; } = null!;
    public string VehicleModel { get; private set; } = null!;
    public string VehicleColor { get; private set; } = null!;
    public DateTime EntryTime { get; private set; }
    public DateTime? ExitTime { get; private set; }
    public int Status { get; private set; } = 0; // 0=Parked, 1=Exited
    public bool IsActive { get; private set; } = true;

    private VehicleEntry() { }

    private VehicleEntry(
        long branchId,
        long parkingSpaceId,
        long customerId,
        string licensePlate,
        string vehicleModel,
        string vehicleColor)
    {
        BranchId = branchId;
        ParkingSpaceId = parkingSpaceId;
        CustomerId = customerId;
        LicensePlate = licensePlate;
        VehicleModel = vehicleModel;
        VehicleColor = vehicleColor;
        EntryTime = DateTime.UtcNow;
    }

    public static Result<VehicleEntry> Create(
        long branchId,
        long parkingSpaceId,
        long customerId,
        string licensePlate,
        string vehicleModel,
        string vehicleColor)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return Result.Failure<VehicleEntry>(
                new Error("VehicleEntry.InvalidPlate", "License plate required."));

        return Result.Success(new VehicleEntry(branchId, parkingSpaceId, customerId, licensePlate, vehicleModel, vehicleColor));
    }

    public void MarkAsExited()
    {
        ExitTime = DateTime.UtcNow;
        Status = 1;
    }

    public int GetDurationMinutes()
    {
        var exitTime = ExitTime ?? DateTime.UtcNow;
        return (int)(exitTime - EntryTime).TotalMinutes;
    }
}

public sealed class VehicleExit : Entity
{
    public long VehicleEntryId { get; private set; }
    public DateTime ExitTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal TotalAmount { get; private set; }
    public int ParkingMode { get; private set; } // 1=Rotativo, 2=Agreement, 3=Monthly
    public bool IsActive { get; private set; } = true;

    private VehicleExit() { }

    private VehicleExit(
        long vehicleEntryId,
        int durationMinutes,
        decimal totalAmount,
        int parkingMode)
    {
        VehicleEntryId = vehicleEntryId;
        ExitTime = DateTime.UtcNow;
        DurationMinutes = durationMinutes;
        TotalAmount = totalAmount;
        ParkingMode = parkingMode;
    }

    public static Result<VehicleExit> Create(
        long vehicleEntryId,
        int durationMinutes,
        decimal totalAmount,
        int parkingMode)
    {
        if (durationMinutes <= 0)
            return Result.Failure<VehicleExit>(
                new Error("VehicleExit.InvalidDuration", "Duration must be greater than 0."));

        if (totalAmount < 0)
            return Result.Failure<VehicleExit>(
                new Error("VehicleExit.InvalidAmount", "Amount cannot be negative."));

        return Result.Success(new VehicleExit(vehicleEntryId, durationMinutes, totalAmount, parkingMode));
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// DOMAIN LAYER — VALUE OBJECTS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Parking.Domain.Common;

public sealed class CPF : ValueObject
{
    public string Value { get; }

    private static readonly Regex CPFRegex = new(@"^\d{11}$", RegexOptions.Compiled);

    private CPF(string value) => Value = value;

    public static Result<CPF> Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || !CPFRegex.IsMatch(cpf))
            return Result.Failure<CPF>(new Error("CPF.Invalid", "CPF must be 11 digits."));

        return Result.Success(new CPF(cpf.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            return Result.Failure<Money>(new Error("Money.InvalidAmount", "Amount cannot be negative."));

        return Result.Success(new Money(amount, currency));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}

public sealed class LicensePlate : ValueObject
{
    public string Value { get; }

    private static readonly Regex PlateRegex = new(@"^[A-Z]{3}-\d{4}$", RegexOptions.Compiled);

    private LicensePlate(string value) => Value = value;

    public static Result<LicensePlate> Create(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate) || !PlateRegex.IsMatch(plate))
            return Result.Failure<LicensePlate>(
                new Error("LicensePlate.Invalid", "License plate must be in format ABC-1234."));

        return Result.Success(new LicensePlate(plate.Trim().ToUpperInvariant()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}

public sealed class DurationMinutes : ValueObject
{
    public int Value { get; }

    private DurationMinutes(int value) => Value = value;

    public static Result<DurationMinutes> Create(int minutes)
    {
        if (minutes <= 0)
            return Result.Failure<DurationMinutes>(
                new Error("DurationMinutes.Invalid", "Duration must be greater than 0."));

        return Result.Success(new DurationMinutes(minutes));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}

// ═══════════════════════════════════════════════════════════════════════════
// DOMAIN LAYER — REPOSITORY INTERFACES
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Employee?> GetByCPFAsync(string cpf, CancellationToken ct = default);
    Task<List<Employee>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(Employee entity, CancellationToken ct = default);
    Task UpdateAsync(Employee entity, CancellationToken ct = default);
}

public interface IEmployeeScheduleRepository
{
    Task<EmployeeSchedule?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<EmployeeSchedule>> GetByEmployeeAsync(long employeeId, CancellationToken ct = default);
    Task AddAsync(EmployeeSchedule entity, CancellationToken ct = default);
    Task UpdateAsync(EmployeeSchedule entity, CancellationToken ct = default);
}

public interface IEmployeePayrollRepository
{
    Task<EmployeePayroll?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<EmployeePayroll>> GetByEmployeeAsync(long employeeId, CancellationToken ct = default);
    Task AddAsync(EmployeePayroll entity, CancellationToken ct = default);
    Task UpdateAsync(EmployeePayroll entity, CancellationToken ct = default);
}

public interface ICashRegisterRepository
{
    Task<CashRegister?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<CashRegister?> GetOpenByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(CashRegister entity, CancellationToken ct = default);
    Task UpdateAsync(CashRegister entity, CancellationToken ct = default);
}

public interface ICashMovementRepository
{
    Task<CashMovement?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<CashMovement>> GetByCashRegisterAsync(long cashRegisterId, CancellationToken ct = default);
    Task<decimal> GetTotalByRegisterAsync(long cashRegisterId, CancellationToken ct = default);
    Task AddAsync(CashMovement entity, CancellationToken ct = default);
}

public interface IParkingSpaceRepository
{
    Task<ParkingSpace?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ParkingSpace?> GetByNumberAsync(long branchId, string spaceNumber, CancellationToken ct = default);
    Task<List<ParkingSpace>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task<int> GetOccupiedCountAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(ParkingSpace entity, CancellationToken ct = default);
    Task UpdateAsync(ParkingSpace entity, CancellationToken ct = default);
}

public interface IVehicleEntryRepository
{
    Task<VehicleEntry?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<VehicleEntry?> GetByParkingSpaceAsync(long parkingSpaceId, CancellationToken ct = default);
    Task<List<VehicleEntry>> GetParkedByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(VehicleEntry entity, CancellationToken ct = default);
    Task UpdateAsync(VehicleEntry entity, CancellationToken ct = default);
}

public interface IVehicleExitRepository
{
    Task<VehicleExit?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<VehicleExit>> GetByEntryAsync(long vehicleEntryId, CancellationToken ct = default);
    Task AddAsync(VehicleExit entity, CancellationToken ct = default);
}

// ═══════════════════════════════════════════════════════════════════════════
// NOTES FOR IMPLEMENTATION
// ═══════════════════════════════════════════════════════════════════════════

/*
NEXT STEPS:

1. Copy all classes above to appropriate Parking.Domain files
   - Entities → Parking.Domain/Entities/
   - ValueObjects → Parking.Domain/ValueObjects/
   - Repository Interfaces → Parking.Domain/Repositories/

2. Create EF Core Configurations (9 files):
   - EmployeeConfiguration.cs
   - EmployeeScheduleConfiguration.cs
   - EmployeePayrollConfiguration.cs
   - CashRegisterConfiguration.cs
   - CashMovementConfiguration.cs
   - ParkingSpaceConfiguration.cs
   - VehicleEntryConfiguration.cs
   - VehicleExitConfiguration.cs

3. Implement Repositories (8 files):
   - EmployeeRepository.cs
   - EmployeeScheduleRepository.cs
   - EmployeePayrollRepository.cs
   - CashRegisterRepository.cs
   - CashMovementRepository.cs
   - ParkingSpaceRepository.cs
   - VehicleEntryRepository.cs
   - VehicleExitRepository.cs

4. Update Infrastructure/DependencyInjection.cs:
   - Register all 8 new repositories

5. Create migration:
   dotnet ef migrations add Fase2_Operacional -p Parking.Infrastructure -s Parking.API

6. Create Application layer (Commands, Queries, Handlers)
   See: TEMPLATES_FASE2_Application.cs (coming next)
*/
