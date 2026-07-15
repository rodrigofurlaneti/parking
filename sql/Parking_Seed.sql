-- ============================================================================
-- PARKING SYSTEM DATABASE — Seed Data (Idempotent)
-- ============================================================================
-- Inserts default lookups, roles, permissions, and example data
-- Safe to run multiple times (MERGE or IF NOT EXISTS)
-- ============================================================================

USE ParkingDb;

-- ============================================================================
-- 1. SEED ROLES & PERMISSIONS (Already in DDL, just ensure they exist)
-- ============================================================================

-- If not already seeded by DDL script
IF NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'Admin')
BEGIN
    SET IDENTITY_INSERT Role ON;
    INSERT INTO Role (Id, Name, Description, IsActive) VALUES
        (1, 'Admin', 'System Administrator', 1),
        (2, 'Manager', 'Branch Manager', 1),
        (3, 'Operator', 'Parking Operator / Cashier', 1),
        (4, 'ServiceTech', 'Lava Rápido Technician', 1),
        (5, 'Viewer', 'Read-only Access', 1);
    SET IDENTITY_INSERT Role OFF;
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE Name = 'parking.entry.register')
BEGIN
    SET IDENTITY_INSERT Permission ON;
    INSERT INTO Permission (Id, Name, Description, IsActive) VALUES
        (1, 'parking.entry.register', 'Register parking entry', 1),
        (2, 'parking.exit.register', 'Register parking exit', 1),
        (3, 'sale.create', 'Create sales', 1),
        (4, 'cash.session.open', 'Open cash session', 1),
        (5, 'cash.session.close', 'Close cash session', 1),
        (6, 'employee.manage', 'Manage employees', 1),
        (7, 'customer.manage', 'Manage customers', 1),
        (8, 'report.view', 'View reports', 1),
        (9, 'system.admin', 'System administration', 1);
    SET IDENTITY_INSERT Permission OFF;
END

-- ============================================================================
-- 2. SEED ROLE PERMISSIONS (Admin = all permissions)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = 1)
BEGIN
    INSERT INTO RolePermission (RoleId, PermissionId, CreatedAt)
    SELECT 1, Id, SYSDATETIME()
    FROM Permission
    WHERE IsActive = 1;
END

-- Manager: most permissions except system.admin
IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = 2 AND PermissionId IN (SELECT Id FROM Permission WHERE Name = 'parking.entry.register'))
BEGIN
    INSERT INTO RolePermission (RoleId, PermissionId, CreatedAt)
    SELECT 2, Id, SYSDATETIME()
    FROM Permission
    WHERE IsActive = 1 AND Name != 'system.admin';
END

-- Operator: entry, exit, sale, cash
IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = 3 AND PermissionId IN (SELECT Id FROM Permission WHERE Name = 'parking.entry.register'))
BEGIN
    INSERT INTO RolePermission (RoleId, PermissionId, CreatedAt)
    SELECT 3, Id, SYSDATETIME()
    FROM Permission
    WHERE IsActive = 1 AND Name IN ('parking.entry.register', 'parking.exit.register', 'sale.create', 'cash.session.open', 'cash.session.close');
END

-- ============================================================================
-- 3. SEED COMPANY & BRANCH (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM Company WHERE Cnpj = '12345678000100')
BEGIN
    SET IDENTITY_INSERT Company ON;
    INSERT INTO Company (Id, Name, Cnpj, LegalName, IsActive, CreatedAt) VALUES
        (1, 'Parking Plus', '12345678000100', 'Parking Plus Estacionamentos Ltda', 1, SYSDATETIME());
    SET IDENTITY_INSERT Company OFF;
END

IF NOT EXISTS (SELECT 1 FROM Branch WHERE Name = 'Centro' AND CompanyId = 1)
BEGIN
    SET IDENTITY_INSERT Branch ON;
    INSERT INTO Branch (Id, CompanyId, Name, Address, PhoneNumber, TotalSpaces, IsActive, CreatedAt) VALUES
        (1, 1, 'Centro', 'Rua Principal, 100 - São Paulo, SP', '11-3000-0001', 50, 1, SYSDATETIME()),
        (2, 1, 'Bela Vista', 'Av. Paulista, 1000 - São Paulo, SP', '11-3000-0002', 75, 1, SYSDATETIME());
    SET IDENTITY_INSERT Branch OFF;
END

-- ============================================================================
-- 4. SEED EXAMPLE USER (Admin)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM AppUser WHERE UserName = 'admin')
BEGIN
    SET IDENTITY_INSERT AppUser ON;
    -- Password: Admin@123 (hashed with BCrypt workFactor=12)
    -- In production, use: BCrypt.Net.BCrypt.HashPassword("Admin@123", workFactor: 12)
    INSERT INTO AppUser (Id, UserName, Email, PasswordHash, FullName, PhoneNumber, IsActive, CreatedAt) VALUES
        (1, 'admin', 'admin@parkingplus.com', '$2a$12$..HASH_HERE..', 'System Admin', '11-99999-9999', 1, SYSDATETIME());
    SET IDENTITY_INSERT AppUser OFF;

    -- Assign Admin role
    INSERT INTO UserRole (UserId, RoleId, AssignedAt)
    VALUES (1, 1, SYSDATETIME());
END

-- ============================================================================
-- 5. SEED JOB TITLES (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM JobTitle WHERE CompanyId = 1 AND Name = 'Operador de Estacionamento')
BEGIN
    SET IDENTITY_INSERT JobTitle ON;
    INSERT INTO JobTitle (Id, CompanyId, Name, Description, BaseSalary, IsActive, CreatedAt) VALUES
        (1, 1, 'Operador de Estacionamento', 'Responsável por entrada/saída de veículos', 1500.00, 1, SYSDATETIME()),
        (2, 1, 'Gerente de Turno', 'Gerencia operações em turno específico', 2500.00, 1, SYSDATETIME()),
        (3, 1, 'Técnico Lava Rápido', 'Executa serviços de lavagem', 1800.00, 1, SYSDATETIME());
    SET IDENTITY_INSERT JobTitle OFF;
END

-- ============================================================================
-- 6. SEED EMPLOYEES (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM Employee WHERE Cpf = '12345678901')
BEGIN
    SET IDENTITY_INSERT Employee ON;
    INSERT INTO Employee (Id, BranchId, JobTitleId, FullName, Cpf, Email, PhoneNumber, HireDate, IsActive, CreatedAt) VALUES
        (1, 1, 2, 'João Silva', '12345678901', 'joao@parkingplus.com', '11-98765-4321', CONVERT(DATE, '2024-01-15'), 1, SYSDATETIME()),
        (2, 1, 1, 'Maria Santos', '12345678902', 'maria@parkingplus.com', '11-98765-4322', CONVERT(DATE, '2024-02-01'), 1, SYSDATETIME()),
        (3, 2, 1, 'Pedro Costa', '12345678903', 'pedro@parkingplus.com', '11-98765-4323', CONVERT(DATE, '2024-03-10'), 1, SYSDATETIME());
    SET IDENTITY_INSERT Employee OFF;
END

-- ============================================================================
-- 7. SEED EMPLOYEE SCHEDULES (Example: João - Seg-Qua 08:00-16:00)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM EmployeeSchedule WHERE EmployeeId = 1 AND DayOfWeek = 1)
BEGIN
    SET IDENTITY_INSERT EmployeeSchedule ON;
    INSERT INTO EmployeeSchedule (Id, EmployeeId, DayOfWeek, StartTime, EndTime, IsWorkDay, CreatedAt) VALUES
        (1, 1, 1, '08:00:00', '16:00:00', 1, SYSDATETIME()),  -- Segunda
        (2, 1, 2, '08:00:00', '16:00:00', 1, SYSDATETIME()),  -- Terça
        (3, 1, 3, '08:00:00', '16:00:00', 1, SYSDATETIME()),  -- Quarta
        (4, 1, 4, NULL, NULL, 0, SYSDATETIME()),              -- Quinta (folga)
        (5, 1, 5, NULL, NULL, 0, SYSDATETIME()),              -- Sexta (folga)
        (6, 1, 6, NULL, NULL, 0, SYSDATETIME()),              -- Sábado (folga)
        (7, 1, 0, NULL, NULL, 0, SYSDATETIME());              -- Domingo (folga)
    SET IDENTITY_INSERT EmployeeSchedule OFF;
END

-- ============================================================================
-- 8. SEED CUSTOMERS (3 Types Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM Customer WHERE Document = '98765432109')
BEGIN
    SET IDENTITY_INSERT Customer ON;
    INSERT INTO Customer (Id, CompanyId, Name, Email, PhoneNumber, Document, CustomerType, IsActive, CreatedAt) VALUES
        (1, 1, 'João da Silva (Rotativo)', 'joao.silva@email.com', '11-98765-1111', '98765432109', 'Rotative', 1, SYSDATETIME()),
        (2, 1, 'Padaria do Centro (Convênio)', 'contato@padariacentro.com', '11-3000-2222', '12345678000200', 'Agreement', 1, SYSDATETIME()),
        (3, 1, 'Empresa Tech Ltda (Mensal)', 'parking@techltda.com', '11-3000-3333', '12345678000300', 'Monthly', 1, SYSDATETIME());
    SET IDENTITY_INSERT Customer OFF;
END

-- ============================================================================
-- 9. SEED VEHICLES (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM Vehicle WHERE LicensePlate = 'ABC-1234')
BEGIN
    SET IDENTITY_INSERT Vehicle ON;
    INSERT INTO Vehicle (Id, CustomerId, LicensePlate, Brand, Model, Year, Color, VehicleType, IsActive, CreatedAt) VALUES
        (1, 1, 'ABC-1234', 'Fiat', 'Palio', 2020, 'Preto', 'Car', 1, SYSDATETIME()),
        (2, 2, 'XYZ-5678', 'Ford', 'Van', 2019, 'Branco', 'Truck', 1, SYSDATETIME()),
        (3, 3, 'DEF-9012', 'Toyota', 'Corolla', 2021, 'Prata', 'Car', 1, SYSDATETIME()),
        (4, 3, 'GHI-3456', 'Honda', 'Civic', 2022, 'Azul', 'Car', 1, SYSDATETIME());
    SET IDENTITY_INSERT Vehicle OFF;
END

-- ============================================================================
-- 10. SEED PARKING SPACES (Example: 50 vagas no Centro)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM ParkingSpace WHERE BranchId = 1 AND SpaceNumber = 'A-01')
BEGIN
    DECLARE @i INT = 1;
    WHILE @i <= 50
    BEGIN
        DECLARE @level INT = CEILING(CAST(@i AS FLOAT) / 25);
        DECLARE @number INT = ((@i - 1) % 25) + 1;
        DECLARE @spaceNumber NVARCHAR(20) = CHAR(64 + @level) + '-' + FORMAT(@number, '00');
        DECLARE @lotTypeId BIGINT = CASE WHEN @i <= 10 THEN 1 ELSE CASE WHEN @i <= 30 THEN 2 ELSE 3 END END;
        DECLARE @isHandicapped BIT = CASE WHEN @i IN (1, 2, 26) THEN 1 ELSE 0 END;

        INSERT INTO ParkingSpace (BranchId, LotTypeId, SpaceNumber, Level, IsHandicapped, IsActive, CreatedAt)
        VALUES (1, @lotTypeId, @spaceNumber, @level, @isHandicapped, 1, SYSDATETIME());

        SET @i = @i + 1;
    END
END

-- ============================================================================
-- 11. SEED ROTATIVE PRICE TIERS (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM RotativeCustomerPriceTier WHERE CompanyId = 1 AND HourNumber = 1)
BEGIN
    SET IDENTITY_INSERT RotativeCustomerPriceTier ON;
    INSERT INTO RotativeCustomerPriceTier (Id, CompanyId, HourNumber, PricePerHour, IsActive, CreatedAt) VALUES
        (1, 1, 1, 15.00, 1, SYSDATETIME()),   -- 1ª hora: mais cara
        (2, 1, 2, 10.00, 1, SYSDATETIME()),   -- 2ª hora: desconto
        (3, 1, 3, 10.00, 1, SYSDATETIME()),   -- 3ª hora: preço normal
        (4, 1, 4, 10.00, 1, SYSDATETIME()),   -- 4ª+ horas
        (5, 1, 5, 10.00, 1, SYSDATETIME());
    SET IDENTITY_INSERT RotativeCustomerPriceTier OFF;
END

-- ============================================================================
-- 12. SEED CASH REGISTERS (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM CashRegister WHERE BranchId = 1 AND TerminalNumber = 'PDV-001')
BEGIN
    SET IDENTITY_INSERT CashRegister ON;
    INSERT INTO CashRegister (Id, BranchId, TerminalNumber, IsActive, CreatedAt) VALUES
        (1, 1, 'PDV-001', 1, SYSDATETIME()),
        (2, 1, 'PDV-002', 1, SYSDATETIME()),
        (3, 2, 'PDV-003', 1, SYSDATETIME());
    SET IDENTITY_INSERT CashRegister OFF;
END

-- ============================================================================
-- 13. SEED SERVICE CATEGORIES & ITEMS (Example: Lava Rápido)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM ServiceCategory WHERE CompanyId = 1 AND Name = 'Lavagem Básica')
BEGIN
    SET IDENTITY_INSERT ServiceCategory ON;
    INSERT INTO ServiceCategory (Id, CompanyId, Name, Description, EstimatedMinutes, IsActive, CreatedAt) VALUES
        (1, 1, 'Lavagem Básica', 'Lavagem externa simples', 20, 1, SYSDATETIME()),
        (2, 1, 'Lavagem Premium', 'Lavagem completa com cera', 40, 1, SYSDATETIME()),
        (3, 1, 'Polimento', 'Polimento e proteção UV', 60, 1, SYSDATETIME());
    SET IDENTITY_INSERT ServiceCategory OFF;

    SET IDENTITY_INSERT ServiceItem ON;
    INSERT INTO ServiceItem (Id, CategoryId, Name, Description, BasePrice, VariableCost, FixedCost, IsActive, CreatedAt) VALUES
        (1, 1, 'Lavagem Externa', 'Jato de água + secagem', 30.00, 5.00, 10.00, 1, SYSDATETIME()),
        (2, 1, 'Aspiração Interna', 'Aspiração completa interior', 20.00, 2.00, 5.00, 1, SYSDATETIME()),
        (3, 2, 'Lavagem Completa', 'Externa + interna + cera', 60.00, 8.00, 15.00, 1, SYSDATETIME()),
        (4, 3, 'Polimento Orbital', 'Polimento profissional', 80.00, 10.00, 20.00, 1, SYSDATETIME());
    SET IDENTITY_INSERT ServiceItem OFF;
END

-- ============================================================================
-- 14. SEED SERVICE PRODUCTS (Example: Products for Lava Rápido)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM ServiceProduct WHERE CompanyId = 1 AND Name = 'Sabão Automotivo')
BEGIN
    SET IDENTITY_INSERT ServiceProduct ON;
    INSERT INTO ServiceProduct (Id, CompanyId, Name, UnitOfMeasure, CostPerUnit, IsActive, CreatedAt) VALUES
        (1, 1, 'Sabão Automotivo', 'L', 15.00, 1, SYSDATETIME()),
        (2, 1, 'Cera Protetora', 'L', 45.00, 1, SYSDATETIME()),
        (3, 1, 'Polidor Premium', 'L', 60.00, 1, SYSDATETIME()),
        (4, 1, 'Secante Rápido', 'L', 25.00, 1, SYSDATETIME());
    SET IDENTITY_INSERT ServiceProduct OFF;
end

-- ============================================================================
-- 15. SEED SUPPLIER (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM Supplier WHERE CompanyId = 1 AND Name = 'ChemiCar Ltda')
BEGIN
    SET IDENTITY_INSERT Supplier ON;
    INSERT INTO Supplier (Id, CompanyId, Name, ContactEmail, ContactPhone, Document, IsActive, CreatedAt) VALUES
        (1, 1, 'ChemiCar Ltda', 'vendas@chemicar.com', '11-3000-5555', '98765432000100', 1, SYSDATETIME()),
        (2, 1, 'Produtos de Limpeza XYZ', 'contato@limpezaxyz.com', '11-3000-6666', '98765432000200', 1, SYSDATETIME());
    SET IDENTITY_INSERT Supplier OFF;
END

-- ============================================================================
-- 16. AGREEMENT MERCHANT (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM AgreementMerchant WHERE CompanyId = 1 AND Name = 'Padaria do Centro')
BEGIN
    SET IDENTITY_INSERT AgreementMerchant ON;
    INSERT INTO AgreementMerchant (Id, CompanyId, BranchId, Name, Document, ContactEmail, ContactPhone, IsActive, CreatedAt) VALUES
        (1, 1, 1, 'Padaria do Centro', '12345678000200', 'gerente@padariacentro.com', '11-3000-2222', 1, SYSDATETIME()),
        (2, 1, 1, 'Banco XYZ Agência', '12345678000300', 'gerente@bancoagencia.com', '11-3000-7777', 1, SYSDATETIME());
    SET IDENTITY_INSERT AgreementMerchant OFF;
END

-- ============================================================================
-- 17. AGREEMENT & MONTHLY CONTRACTS (Example)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM AgreementCustomerContract WHERE CustomerId = 2)
BEGIN
    SET IDENTITY_INSERT AgreementCustomerContract ON;
    INSERT INTO AgreementCustomerContract (Id, CustomerId, MerchantId, DiscountPercent, MaxDiscountPerDay, ValidFrom, ValidUntil, IsActive, CreatedAt) VALUES
        (1, 2, 1, 30.00, 100.00, CONVERT(DATE, '2026-01-01'), NULL, 1, SYSDATETIME());
    SET IDENTITY_INSERT AgreementCustomerContract OFF;

    SET IDENTITY_INSERT MonthlyCustomerContract ON;
    INSERT INTO MonthlyCustomerContract (Id, CustomerId, BranchId, MonthlyFee, MaxVehicles, ValidFrom, ValidUntil, AutoRenewal, IsActive, CreatedAt) VALUES
        (1, 3, 1, 500.00, 2, CONVERT(DATE, '2026-01-01'), NULL, 1, 1, SYSDATETIME());
    SET IDENTITY_INSERT MonthlyCustomerContract OFF;
END

-- ============================================================================
-- END OF SEED
-- ============================================================================

PRINT 'Parking Database seed completed successfully.';
