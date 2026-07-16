import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import axios from "axios";
import { registerSale } from "../api/sale";
import { getOpenCashRegisterByBranch } from "../api/vehicleEntry";
import type { CashRegisterDto, PaymentInput, SaleDto } from "../types/api";

function extractErrorMessage(err: unknown, fallback: string): string {
  if (axios.isAxiosError(err)) {
    const apiMessage = (err.response?.data as { error?: string } | undefined)?.error;
    return apiMessage ?? fallback;
  }
  return fallback;
}

const inputClass =
  "w-full rounded-md border border-slate-300 px-3 py-1.5 text-sm shadow-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500";
const labelClass = "mb-1 block text-xs font-medium text-slate-600";
const buttonClass =
  "rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:opacity-60";

const PAYMENT_METHODS = [
  { value: 1, label: "1 - Dinheiro" },
  { value: 2, label: "2 - Débito" },
  { value: 3, label: "3 - Crédito" },
  { value: 4, label: "4 - Pix" },
  { value: 5, label: "5 - Convênio" },
];

function emptyPayment(): PaymentInput {
  return { paymentMethod: 1, amount: 0 };
}

export default function SalesPage() {
  const [branchId, setBranchId] = useState(1);
  const [vehicleExitId, setVehicleExitId] = useState(1);
  const [cashRegisterId, setCashRegisterId] = useState<number | "">("");
  const [payments, setPayments] = useState<PaymentInput[]>([emptyPayment()]);

  const [openRegister, setOpenRegister] = useState<CashRegisterDto | null>(null);
  const [registerLoading, setRegisterLoading] = useState(false);
  const [registerError, setRegisterError] = useState<string | null>(null);

  const [result, setResult] = useState<SaleDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setRegisterLoading(true);
    setRegisterError(null);
    getOpenCashRegisterByBranch(branchId)
      .then((register) => {
        if (cancelled) return;
        setOpenRegister(register);
        setCashRegisterId(register ? register.id : "");
      })
      .catch((err) => {
        if (cancelled) return;
        setRegisterError(extractErrorMessage(err, "Não foi possível verificar o caixa aberto."));
        setOpenRegister(null);
        setCashRegisterId("");
      })
      .finally(() => {
        if (!cancelled) setRegisterLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId]);

  function updatePayment(index: number, patch: Partial<PaymentInput>) {
    setPayments((prev) => prev.map((p, i) => (i === index ? { ...p, ...patch } : p)));
  }

  function addPayment() {
    setPayments((prev) => [...prev, emptyPayment()]);
  }

  function removePayment(index: number) {
    setPayments((prev) => prev.filter((_, i) => i !== index));
  }

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (cashRegisterId === "") {
      setError("Nenhum caixa aberto para esta filial. Abra um caixa antes de registrar a venda.");
      return;
    }
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const sale = await registerSale({
        branchId,
        vehicleExitId,
        cashRegisterId,
        payments,
      });
      setResult(sale);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível registrar a venda (verifique se os valores dos pagamentos batem com o total)."));
    } finally {
      setLoading(false);
    }
  }

  const totalPayments = payments.reduce((sum, p) => sum + (Number(p.amount) || 0), 0);

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-bold text-slate-900">Vendas</h1>

      <section className="rounded-xl bg-white p-5 shadow-sm">
        <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
            <div>
              <label className={labelClass} htmlFor="saleBranchId">
                Filial (ID)
              </label>
              <input
                id="saleBranchId"
                type="number"
                min={1}
                value={branchId}
                onChange={(e) => setBranchId(Number(e.target.value))}
                className={inputClass}
              />
            </div>
            <div>
              <label className={labelClass} htmlFor="saleVehicleExitId">
                Saída do Veículo (ID)
              </label>
              <input
                id="saleVehicleExitId"
                type="number"
                min={1}
                value={vehicleExitId}
                onChange={(e) => setVehicleExitId(Number(e.target.value))}
                className={inputClass}
              />
            </div>
            <div>
              <label className={labelClass} htmlFor="saleCashRegisterId">
                Caixa Aberto
              </label>
              <input
                id="saleCashRegisterId"
                readOnly
                value={
                  registerLoading
                    ? "Verificando..."
                    : openRegister
                      ? `#${openRegister.id} (aberto em ${new Date(openRegister.openedAt).toLocaleString("pt-BR")})`
                      : "Nenhum caixa aberto"
                }
                className={`${inputClass} bg-slate-50 text-slate-500`}
              />
              {registerError && <p className="mt-1 text-xs text-red-600">{registerError}</p>}
              {!registerLoading && !openRegister && (
                <p className="mt-1 text-xs text-amber-700">
                  Abra um caixa para esta filial na tela de Operação de Pátio.
                </p>
              )}
            </div>
          </div>

          <div>
            <div className="mb-2 flex items-center justify-between">
              <h2 className="text-sm font-semibold text-slate-700">Pagamentos</h2>
              <span className="text-xs text-slate-500">
                Total informado:{" "}
                {totalPayments.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}
              </span>
            </div>
            <div className="flex flex-col gap-2">
              {payments.map((payment, index) => (
                <div key={index} className="flex items-end gap-2">
                  <div className="flex-1">
                    <label className={labelClass} htmlFor={`payMethod-${index}`}>
                      Forma de Pagamento
                    </label>
                    <select
                      id={`payMethod-${index}`}
                      value={payment.paymentMethod}
                      onChange={(e) =>
                        updatePayment(index, { paymentMethod: Number(e.target.value) })
                      }
                      className={inputClass}
                    >
                      {PAYMENT_METHODS.map((m) => (
                        <option key={m.value} value={m.value}>
                          {m.label}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="flex-1">
                    <label className={labelClass} htmlFor={`payAmount-${index}`}>
                      Valor (R$)
                    </label>
                    <input
                      id={`payAmount-${index}`}
                      type="number"
                      min={0}
                      step="0.01"
                      value={payment.amount}
                      onChange={(e) => updatePayment(index, { amount: Number(e.target.value) })}
                      className={inputClass}
                    />
                  </div>
                  <button
                    type="button"
                    onClick={() => removePayment(index)}
                    disabled={payments.length === 1}
                    className="rounded-md border border-slate-300 px-3 py-1.5 text-sm text-slate-600 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
                  >
                    Remover
                  </button>
                </div>
              ))}
            </div>
            <button
              type="button"
              onClick={addPayment}
              className="mt-2 rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50"
            >
              + Adicionar pagamento
            </button>
          </div>

          {error && (
            <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
          )}

          {result && (
            <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
              Venda #{result.saleNumber} registrada. Total:{" "}
              {result.totalAmount.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}
            </div>
          )}

          <button type="submit" disabled={loading} className={`${buttonClass} self-start`}>
            {loading ? "Registrando..." : "Registrar Venda"}
          </button>
        </form>
      </section>
    </div>
  );
}
