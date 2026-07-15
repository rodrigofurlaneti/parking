# Parking System — Checklist de Implementação

**Status:** Ready for Development  
**Stack:** .NET 9 + EF Core + SQL Server + Clean Architecture  
**Padrão:** SyncBar (DDD/CQRS)  
**Data:** 2026-07-15

---

## Fases de Implementação

### ✅ Fase 0 — Modelagem (CONCLUÍDA)

- [x] DDL SQL (48 tabelas)
- [x] Documentação de negócio
- [x] Diagrama ER (Mermaid)

---

## 🔴 Fase 1 — Foundation (2 sprints)

**Objetivo:** Auth + Org

- [ ] Company, Branch
- [ ] AppUser, Role, Permission, RefreshToken, AccessLog
- [ ] JWT auth + BCrypt
- [ ] Repositories + EF Configs
- [ ] Controllers + Tests

---

## 🟠 Fase 2 — Operacional (2 sprints)

**Objetivo:** Employees + Cash + Parking Physical

- [ ] JobTitle, Employee, EmployeeSchedule, EmployeeTimeOff
- [ ] CashRegister, CashSession, CashMovement
- [ ] ParkingSpace, ParkingLotType, ParkingSpaceStatus

---

## 🟡 Fase 3 — MVP (2 sprints)

**Objetivo:** Customer + Vehicle

- [ ] Customer (3 tipos)
- [ ] RotativeCustomerPriceTier, AgreementMerchant, MonthlyCustomerContract
- [ ] Vehicle

---

## 🟢 Fase 4 — CORE (4 sprints)

**Objetivo:** MVP COMPLETO — Parking + Billing

- [ ] ParkingSession (entry/exit/pricing)
- [ ] Sale, SalePayment
- [ ] OpenCashSession, CloseCashSession
- [ ] Tests: rotativo, convênio, mensal

**Success:** Entrada/saída + preço correto + múltiplos pagamentos + reconciliação

---

## 🟡 Fase 5 — Lava Rápido (3 sprints)

**Objetivo:** Service + Products + Costs

- [ ] ServiceCategory, ServiceItem, ServiceProduct
- [ ] ServiceOrder, ServiceOrderItem
- [ ] Stock deduction on completion

---

## 🟡 Fase 6 — Estoque (3 sprints)

**Objetivo:** Ledger-based stock control

- [ ] Supplier, StockItem, StockMovement
- [ ] Purchase, PurchaseItem
- [ ] Stock ledger (NEVER UPDATE CurrentQuantity directly)

---

## ⚪ Fase 7 — Expansão (∞)

**Objetivo:** Reports, scheduling, mobile

- [ ] Occupancy reports
- [ ] Revenue by type
- [ ] Stock alerts
- [ ] Service scheduling
- [ ] Reserve spaces
- [ ] Online payment

---

## Timeline (MVP = 10 sprints)

| Fase | Sprints | Status |
|------|---------|--------|
| 0 | 1 | ✅ Done |
| 1 | 2 | ⏳ Ready |
| 2 | 2 | ⏳ Ready |
| 3 | 2 | ⏳ Ready |
| 4 | 4 | ⏳ Ready |
| 5 | 3 | ⏳ Pending 4 |
| 6 | 3 | ⏳ Ready |
| 7 | ∞ | ⏳ Backlog |

---

**Pronto para começar Fase 1!**
