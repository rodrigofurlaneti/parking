# Parking System — Modelagem de Negócio e Banco de Dados

**Projeto:** Parking Management System  
**Stack:** .NET 9 + EF Core + SQL Server  
**Padrão:** Clean Architecture / DDD / CQRS (baseado em SyncBar)  
**Data:** 2026-07-15

---

## 1. Visão Geral do Negócio

O sistema gerencia **estacionamentos de diferentes tipos** com três modelos de cliente:

1. **Rotativo** — Paga por hora; primeira hora mais cara
2. **Convênio** — Desconto variável com comerciantes parceiros
3. **Mensal** — Taxa fixa/mês

Além de estacionamento, oferece **Lava Rápido** como serviço adicional com:
- Controle de lavagens/serviços
- Produtos (sabão, cera, etc)
- Funcionários e escala de folgas
- Custos fixos e variáveis
- Faturamento integrado

**Objetivo:** Um padrão harmônico que funcione para **qualquer estacionamento**, independente de porte ou localização.

---

## 2. Módulos e Tabelas (48 tabelas)

### 2.1 Organizacional (2)
- **Company** — razão social, CNPJ
- **Branch** — endereço, telefone, total de vagas por filial

### 2.2 Autenticação (6)
- **Role** — papéis (Admin, Operador, Gerente)
- **Permission** — permissões granulares
- **RolePermission** — relacionamento N:N
- **AppUser** — usuários do sistema
- **UserRole** — N:N (user ↔ role)
- **RefreshToken** — tokens JWT
- **AccessLog** — auditoria de acesso

### 2.3 Funcionários (3)
- **JobTitle** — cargo (Operador, Gerente Turno, Faxineiro)
- **Employee** — dados PF + vínculo com JobTitle
- **EmployeeSchedule** — escala de trabalho (seg-dom, horários)
- **EmployeeTimeOff** — férias, licenças (com aprovação)

### 2.4 Clientes (5)
- **Customer** — agregado raiz; 3 tipos (Rotativo, Agreement, Monthly)
- **RotativeCustomerPriceTier** — tabela de preços por hora
- **AgreementMerchant** — comerciantes parceiros
- **AgreementCustomerContract** — vínculo + desconto
- **MonthlyCustomerContract** — plano mensal + máximo de veículos

### 2.5 Veículos (1)
- **Vehicle** — placa, marca, modelo, cor, tipo

### 2.6 Estacionamento (3)
- **ParkingLotType** — tipos de vaga (Standard, Covered, Premium)
- **ParkingSpace** — vaga individual + nível + acessibilidade
- **ParkingSpaceStatus** — lookup (Available, Occupied, Reserved, Blocked)

### 2.7 Sessões de Estacionamento (1)
- **ParkingSession** — **AGREGADO RAIZ** — entrada/saída, cálculo de preço

### 2.8 Lava Rápido (5)
- **ServiceCategory**, **ServiceItem**, **ServiceProduct**
- **ServiceItemProduct**, **ServiceOrder**

### 2.9 Caixa (4)
- **CashRegister**, **CashSessionStatus**, **CashSession**, **CashMovement**

### 2.10 Faturamento (3)
- **PaymentMethod**, **Sale**, **SalePayment**

### 2.11 Estoque (5)
- **Supplier**, **StockItem**, **StockMovementType**, **StockMovement**, **Purchase**

---

## 3. Fluxos de Negócio

### 3.1 Rotativo
```
Entrada → Customer + Vehicle
      → ParkingSession criada
Saída → Calcular TotalHours
     → BaseAmount via RotativeCustomerPriceTier
     → Sale criada
     → CashMovement registrado
     → ParkingSession.IsCharged = 1
```

### 3.2 Convênio
```
Entrada → Validar AgreementCustomerContract ativo
      → ParkingSession com CustomerType = 'Agreement'
Saída → BaseAmount - DescountAmount (%)
     → TotalAmount = desconto aplicado
     → Sale com DiscountAmount > 0
```

### 3.3 Mensal
```
Entrada → Validar MonthlyCustomerContract ativo
      → MaxVehicles check
      → ParkingSession com BaseAmount = 0
Saída → TotalAmount = 0 (não fatura por sessão)
Job mensal → Sale com MonthlyFee
```

---

## 4. Invariantes de Negócio

**Domain Level:**
- Customer.CustomerType ∈ {'Rotative', 'Agreement', 'Monthly'}
- ParkingSession.TotalHours = (ExitTime - EntryTime) / 3600
- Sale.TotalAmount = SubtotalAmount - DiscountAmount + ServiceFeeAmount
- CashSession: uma única sessão Open por CashRegister
- StockItem.CurrentQuantity nunca UPDATE direto — sempre via StockMovement

**Application Level:**
- Validar desconto ≤ MaxDiscountPerDay
- Validar CashSession aberta antes de Sale
- Validar MonthlyCustomerContract válido antes de estacionar
- Não permitir mais produto que StockItem.CurrentQuantity

---

## 5. Padrão SQL

- **IDs:** BIGINT IDENTITY(1,1)
- **Datas:** DATETIME2
- **Valores:** DECIMAL(18,2)
- **Quantidades:** DECIMAL(18,3)
- **Soft delete:** UPDATE IsActive = 0
- **Índices únicos filtrados:** WHERE IsActive = 1

---

## 6. Ordem de Implementação (MVP)

| Fase | Objetivo | Sprints |
|------|----------|---------|
| 1 | Foundation: Auth + Org | 2 |
| 2 | Operacional: Employees + Cash | 2 |
| 3 | MVP: Customer + Vehicle | 2 |
| 4 | **CORE:** Parking + Billing | 4 |
| **Total MVP** | **Entrada/Saída 3 tipos + Faturamento** | **10** |

---

**Versão:** 1.0 — Ready for Architecture Review
