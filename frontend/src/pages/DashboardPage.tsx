import { useCallback, useEffect, useState } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
  PieChart,
  Pie,
  Cell,
} from "recharts";
import {
  getOccupancyReport,
  getRevenueReport,
  getStockReport,
  getCashReport,
} from "../api/reports";
import type { OccupancyReportDto, RevenueReportDto, StockReportDto, CashReportDto } from "../types/api";

const CHART_COLORS = ["#0f172a", "#334155", "#64748b", "#94a3b8", "#cbd5e1"];

function formatCurrency(value: number): string {
  return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function formatPercentage(value: number): string {
  return `${value.toLocaleString("pt-BR", { maximumFractionDigits: 1 })}%`;
}

function defaultFromDate(): string {
  const now = new Date();
  const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
  return firstDay.toISOString().slice(0, 10);
}

function defaultToDate(): string {
  return new Date().toISOString().slice(0, 10);
}

export default function DashboardPage() {
  const [branchId, setBranchId] = useState(1);
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(defaultToDate());

  const [occupancy, setOccupancy] = useState<OccupancyReportDto | null>(null);
  const [revenue, setRevenue] = useState<RevenueReportDto | null>(null);
  const [stock, setStock] = useState<StockReportDto | null>(null);
  const [cash, setCash] = useState<CashReportDto | null>(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadReports = useCallback(async () => {
    setLoading(true);
    setError(null);

    const params = { branchId, fromDate, toDate: `${toDate}T23:59:59` };

    try {
      const [occupancyData, revenueData, stockData, cashData] = await Promise.all([
        getOccupancyReport(params),
        getRevenueReport(params),
        getStockReport(branchId),
        getCashReport(params),
      ]);

      setOccupancy(occupancyData);
      setRevenue(revenueData);
      setStock(stockData);
      setCash(cashData);
    } catch {
      setError("Não foi possível carregar os relatórios. Verifique a filial informada e tente novamente.");
    } finally {
      setLoading(false);
    }
  }, [branchId, fromDate, toDate]);

  useEffect(() => {
    loadReports();
  }, [loadReports]);

  const totalCashDifference =
    cash?.reconciliations.reduce((sum, r) => sum + r.difference, 0) ?? 0;

  const revenueByType = revenue
    ? [
        { name: "Rotativo", value: revenue.rotativeRevenue },
        { name: "Convênio", value: revenue.agreementRevenue },
        { name: "Mensalista", value: revenue.monthlyRevenue },
        { name: "Lava-rápido", value: revenue.washServiceRevenue },
      ]
    : [];

  const hourlyData =
    occupancy?.hourlyBreakdown.map((h) => ({
      hour: `${h.hour}h`,
      entradas: h.entryCount,
    })) ?? [];

  return (
    <div className="flex flex-col gap-6">
      <section className="rounded-xl bg-white p-4 shadow-sm">
        <form
          className="flex flex-wrap items-end gap-4"
          onSubmit={(e) => {
            e.preventDefault();
            loadReports();
          }}
        >
          <div>
            <label htmlFor="branchId" className="mb-1 block text-xs font-medium text-slate-600">
              Filial (ID)
            </label>
            <input
              id="branchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className="w-28 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
            />
          </div>
          <div>
            <label htmlFor="fromDate" className="mb-1 block text-xs font-medium text-slate-600">
              De
            </label>
            <input
              id="fromDate"
              type="date"
              value={fromDate}
              onChange={(e) => setFromDate(e.target.value)}
              className="rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
            />
          </div>
          <div>
            <label htmlFor="toDate" className="mb-1 block text-xs font-medium text-slate-600">
              Até
            </label>
            <input
              id="toDate"
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              className="rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
            />
          </div>
          <button
            type="submit"
            className="rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700"
          >
            Atualizar
          </button>
        </form>
      </section>

      {loading && (
        <div className="rounded-xl bg-white p-6 text-center text-sm text-slate-500 shadow-sm">
          Carregando relatórios...
        </div>
      )}

      {error && !loading && (
        <div className="rounded-xl bg-red-50 p-4 text-sm text-red-700 shadow-sm">{error}</div>
      )}

      {!loading && !error && (
        <>
          <section className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <SummaryCard
              label="Receita Total"
              value={revenue ? formatCurrency(revenue.grandTotal) : "-"}
              accent="bg-emerald-50 text-emerald-700"
            />
            <SummaryCard
              label="Ocupação Média"
              value={occupancy ? formatPercentage(occupancy.averageOccupancyPercentage) : "-"}
              accent="bg-blue-50 text-blue-700"
            />
            <SummaryCard
              label="Diferença de Caixa"
              value={formatCurrency(totalCashDifference)}
              accent={
                totalCashDifference < 0
                  ? "bg-red-50 text-red-700"
                  : "bg-slate-50 text-slate-700"
              }
            />
          </section>

          <section className="grid grid-cols-1 gap-4 lg:grid-cols-2">
            <div className="rounded-xl bg-white p-4 shadow-sm">
              <h2 className="mb-3 text-sm font-semibold text-slate-700">
                Receita por Tipo de Cliente
              </h2>
              <ResponsiveContainer width="100%" height={280}>
                <PieChart>
                  <Pie
                    data={revenueByType}
                    dataKey="value"
                    nameKey="name"
                    cx="50%"
                    cy="50%"
                    outerRadius={90}
                    label={(entry) => `${entry.name}`}
                  >
                    {revenueByType.map((entry, index) => (
                      <Cell key={entry.name} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value) => formatCurrency(Number(value))} />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </div>

            <div className="rounded-xl bg-white p-4 shadow-sm">
              <h2 className="mb-3 text-sm font-semibold text-slate-700">
                Ocupação por Hora (entradas)
              </h2>
              <ResponsiveContainer width="100%" height={280}>
                <BarChart data={hourlyData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                  <XAxis dataKey="hour" tick={{ fontSize: 12 }} />
                  <YAxis allowDecimals={false} tick={{ fontSize: 12 }} />
                  <Tooltip />
                  <Bar dataKey="entradas" fill="#334155" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </section>

          <section className="rounded-xl bg-white p-4 shadow-sm">
            <h2 className="mb-3 text-sm font-semibold text-slate-700">
              Produtos Abaixo do Estoque Mínimo
            </h2>
            {stock && stock.belowMinimumProducts.length > 0 ? (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200 text-sm">
                  <thead>
                    <tr className="text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
                      <th className="py-2 pr-4">Produto</th>
                      <th className="py-2 pr-4">SKU</th>
                      <th className="py-2 pr-4">Categoria</th>
                      <th className="py-2 pr-4 text-right">Estoque Atual</th>
                      <th className="py-2 pr-4 text-right">Estoque Mínimo</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100">
                    {stock.belowMinimumProducts.map((product) => (
                      <tr key={product.id}>
                        <td className="py-2 pr-4 font-medium text-slate-800">{product.name}</td>
                        <td className="py-2 pr-4 text-slate-500">{product.sku}</td>
                        <td className="py-2 pr-4 text-slate-500">{product.category}</td>
                        <td className="py-2 pr-4 text-right text-red-600">{product.stock}</td>
                        <td className="py-2 pr-4 text-right text-slate-500">
                          {product.minimumStock}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <p className="text-sm text-slate-500">
                Nenhum produto abaixo do estoque mínimo nesta filial.
              </p>
            )}
          </section>
        </>
      )}
    </div>
  );
}

function SummaryCard({
  label,
  value,
  accent,
}: {
  label: string;
  value: string;
  accent: string;
}) {
  return (
    <div className="rounded-xl bg-white p-5 shadow-sm">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-500">{label}</p>
      <p className={`mt-2 inline-block rounded-md px-2 py-1 text-2xl font-bold ${accent}`}>
        {value}
      </p>
    </div>
  );
}
