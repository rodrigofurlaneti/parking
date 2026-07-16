import { useState } from "react";
import type { FormEvent } from "react";
import axios from "axios";
import {
  createServiceCategory,
  createServiceItem,
  getServiceItems,
  createWashSchedule,
  assignWashEmployee,
} from "../api/wash";
import type { ServiceCategoryDto, ServiceItemDto, WashScheduleDto } from "../types/api";

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
const sectionClass = "rounded-xl bg-white p-5 shadow-sm";
const buttonClass =
  "rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:opacity-60 self-start";

export default function WashPage() {
  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-bold text-slate-900">Lava Rápido</h1>
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <CreateCategoryForm />
        <CreateItemForm />
      </div>
      <ServiceItemsList />
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <CreateScheduleForm />
        <AssignEmployeeForm />
      </div>
    </div>
  );
}

function CreateCategoryForm() {
  const [branchId, setBranchId] = useState(1);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const [result, setResult] = useState<ServiceCategoryDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const category = await createServiceCategory({
        branchId,
        name,
        description: description || undefined,
      });
      setResult(category);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar a categoria."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Categoria de Serviço</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="catBranchId">
            Filial (ID)
          </label>
          <input
            id="catBranchId"
            type="number"
            min={1}
            value={branchId}
            onChange={(e) => setBranchId(Number(e.target.value))}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="catName">
            Nome
          </label>
          <input
            id="catName"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="catDescription">
            Descrição
          </label>
          <input
            id="catDescription"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className={inputClass}
          />
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Categoria criada com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Categoria"}
        </button>
      </form>
    </section>
  );
}

function CreateItemForm() {
  const [serviceCategoryId, setServiceCategoryId] = useState(1);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [durationMinutes, setDurationMinutes] = useState(30);
  const [baseCost, setBaseCost] = useState(0);

  const [result, setResult] = useState<ServiceItemDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const item = await createServiceItem({
        serviceCategoryId,
        name,
        description: description || undefined,
        durationMinutes,
        baseCost,
      });
      setResult(item);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o item de serviço."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Item de Serviço</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="itemCategoryId">
            Categoria (ID)
          </label>
          <input
            id="itemCategoryId"
            type="number"
            min={1}
            value={serviceCategoryId}
            onChange={(e) => setServiceCategoryId(Number(e.target.value))}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="itemName">
            Nome
          </label>
          <input
            id="itemName"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="itemDescription">
            Descrição
          </label>
          <input
            id="itemDescription"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className={inputClass}
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="itemDuration">
              Duração (min)
            </label>
            <input
              id="itemDuration"
              type="number"
              min={1}
              value={durationMinutes}
              onChange={(e) => setDurationMinutes(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="itemCost">
              Preço Base (R$)
            </label>
            <input
              id="itemCost"
              type="number"
              min={0}
              step="0.01"
              value={baseCost}
              onChange={(e) => setBaseCost(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Item criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Item"}
        </button>
      </form>
    </section>
  );
}

function ServiceItemsList() {
  const [categoryId, setCategoryId] = useState(1);
  const [items, setItems] = useState<ServiceItemDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const data = await getServiceItems(categoryId);
      setItems(data);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível carregar os itens de serviço."));
      setItems([]);
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Itens de Serviço por Categoria</h2>
      <form className="mb-4 flex flex-wrap items-end gap-4" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="listCategoryId">
            Categoria (ID)
          </label>
          <input
            id="listCategoryId"
            type="number"
            min={1}
            value={categoryId}
            onChange={(e) => setCategoryId(Number(e.target.value))}
            className={`${inputClass} w-28`}
          />
        </div>
        <button type="submit" disabled={loading} className="rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:opacity-60">
          {loading ? "Carregando..." : "Listar"}
        </button>
      </form>

      {error && (
        <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
      )}

      {items.length > 0 ? (
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-slate-200 text-sm">
            <thead>
              <tr className="text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
                <th className="py-2 pr-4">ID</th>
                <th className="py-2 pr-4">Nome</th>
                <th className="py-2 pr-4">Duração</th>
                <th className="py-2 pr-4 text-right">Preço Base</th>
                <th className="py-2 pr-4">Ativo</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {items.map((item) => (
                <tr key={item.id}>
                  <td className="py-2 pr-4">{item.id}</td>
                  <td className="py-2 pr-4 font-medium text-slate-800">{item.name}</td>
                  <td className="py-2 pr-4">{item.durationMinutes} min</td>
                  <td className="py-2 pr-4 text-right">
                    {item.baseCost.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}
                  </td>
                  <td className="py-2 pr-4">{item.isActive ? "Sim" : "Não"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <p className="text-sm text-slate-500">Nenhum item carregado.</p>
      )}
    </section>
  );
}

function CreateScheduleForm() {
  const [branchId, setBranchId] = useState(1);
  const [vehicleEntryId, setVehicleEntryId] = useState(1);
  const [scheduledTime, setScheduledTime] = useState("");
  const [employeeId, setEmployeeId] = useState(1);

  const [result, setResult] = useState<WashScheduleDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const schedule = await createWashSchedule({
        branchId,
        vehicleEntryId,
        scheduledTime,
        employeeId,
      });
      setResult(schedule);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o agendamento."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Agendamento</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="schedBranchId">
              Filial (ID)
            </label>
            <input
              id="schedBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="schedEntryId">
              Entrada do Veículo (ID)
            </label>
            <input
              id="schedEntryId"
              type="number"
              min={1}
              value={vehicleEntryId}
              onChange={(e) => setVehicleEntryId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>
        <div>
          <label className={labelClass} htmlFor="schedTime">
            Data/Hora Agendada
          </label>
          <input
            id="schedTime"
            type="datetime-local"
            required
            value={scheduledTime}
            onChange={(e) => setScheduledTime(e.target.value)}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="schedEmployeeId">
            Funcionário (ID)
          </label>
          <input
            id="schedEmployeeId"
            type="number"
            min={1}
            value={employeeId}
            onChange={(e) => setEmployeeId(Number(e.target.value))}
            className={inputClass}
          />
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Agendamento criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Agendamento"}
        </button>
      </form>
    </section>
  );
}

function AssignEmployeeForm() {
  const [scheduleId, setScheduleId] = useState(1);
  const [employeeId, setEmployeeId] = useState(1);

  const [result, setResult] = useState<WashScheduleDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const schedule = await assignWashEmployee(scheduleId, employeeId);
      setResult(schedule);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível atribuir o funcionário."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Atribuir Funcionário</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="assignScheduleId">
            Agendamento (ID)
          </label>
          <input
            id="assignScheduleId"
            type="number"
            min={1}
            value={scheduleId}
            onChange={(e) => setScheduleId(Number(e.target.value))}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="assignEmployeeId">
            Funcionário (ID)
          </label>
          <input
            id="assignEmployeeId"
            type="number"
            min={1}
            value={employeeId}
            onChange={(e) => setEmployeeId(Number(e.target.value))}
            className={inputClass}
          />
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Funcionário atribuído ao agendamento #{result.id}.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Atribuindo..." : "Atribuir"}
        </button>
      </form>
    </section>
  );
}
