import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import axios from "axios";
import {
  createEmployee,
  getAllEmployeesByBranch,
  updateEmployee,
  terminateEmployee,
} from "../api/employee";
import type { EmployeeDto } from "../types/api";

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
  "rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:opacity-60";

export default function EmployeesPage() {
  const [branchId, setBranchId] = useState(1);
  const [employees, setEmployees] = useState<EmployeeDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshCount, setRefreshCount] = useState(0);
  const [editingEmployee, setEditingEmployee] = useState<EmployeeDto | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);
    getAllEmployeesByBranch(branchId)
      .then((data) => {
        if (!cancelled) setEmployees(data);
      })
      .catch((err) => {
        if (!cancelled) {
          setError(extractErrorMessage(err, "Não foi possível carregar os funcionários."));
          setEmployees([]);
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId, refreshCount]);

  async function handleTerminate(employee: EmployeeDto) {
    if (!confirm(`Desligar o funcionário "${employee.name}"?`)) return;
    try {
      await terminateEmployee(employee.id);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      alert(extractErrorMessage(err, "Não foi possível desligar o funcionário."));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-bold text-slate-900">Funcionários</h1>

      <EmployeeForm
        editingEmployee={editingEmployee}
        onDone={() => {
          setEditingEmployee(null);
          setRefreshCount((v) => v + 1);
        }}
        onCancelEdit={() => setEditingEmployee(null)}
        defaultBranchId={branchId}
      />

      <section className={sectionClass}>
        <div className="mb-3 flex flex-wrap items-end justify-between gap-3">
          <h2 className="text-sm font-semibold text-slate-700">Lista de Funcionários</h2>
          <div>
            <label className={labelClass} htmlFor="empListBranchId">
              Filial (ID)
            </label>
            <input
              id="empListBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={`${inputClass} w-28`}
            />
          </div>
        </div>

        {error && (
          <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}

        {loading ? (
          <p className="text-sm text-slate-400">Carregando...</p>
        ) : employees.length === 0 ? (
          <p className="text-sm text-slate-400">Nenhum funcionário cadastrado nesta filial.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead>
                <tr className="border-b border-slate-200 text-xs uppercase text-slate-500">
                  <th className="py-2 pr-3">Nome</th>
                  <th className="py-2 pr-3">Email</th>
                  <th className="py-2 pr-3">Telefone</th>
                  <th className="py-2 pr-3">CPF</th>
                  <th className="py-2 pr-3">Cargo (Role ID)</th>
                  <th className="py-2 pr-3">Status</th>
                  <th className="py-2 pr-3">Ações</th>
                </tr>
              </thead>
              <tbody>
                {employees.map((emp) => (
                  <tr key={emp.id} className="border-b border-slate-100">
                    <td className="py-2 pr-3 font-medium text-slate-800">{emp.name}</td>
                    <td className="py-2 pr-3 text-slate-600">{emp.email}</td>
                    <td className="py-2 pr-3 text-slate-600">{emp.phone}</td>
                    <td className="py-2 pr-3 text-slate-600">{emp.cpf}</td>
                    <td className="py-2 pr-3 text-slate-600">{emp.roleId}</td>
                    <td className="py-2 pr-3">
                      {emp.isActive ? (
                        <span className="rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-medium text-emerald-700">
                          Ativo
                        </span>
                      ) : (
                        <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-500">
                          Desligado
                        </span>
                      )}
                    </td>
                    <td className="py-2 pr-3">
                      <div className="flex gap-2">
                        <button
                          type="button"
                          onClick={() => setEditingEmployee(emp)}
                          className="text-xs font-medium text-slate-600 underline hover:text-slate-900"
                        >
                          Editar
                        </button>
                        {emp.isActive && (
                          <button
                            type="button"
                            onClick={() => handleTerminate(emp)}
                            className="text-xs font-medium text-red-600 underline hover:text-red-800"
                          >
                            Desligar
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
}

function EmployeeForm({
  editingEmployee,
  onDone,
  onCancelEdit,
  defaultBranchId,
}: {
  editingEmployee: EmployeeDto | null;
  onDone: () => void;
  onCancelEdit: () => void;
  defaultBranchId: number;
}) {
  const isEditing = editingEmployee !== null;

  const [companyId, setCompanyId] = useState(1);
  const [branchId, setBranchId] = useState(defaultBranchId);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [cpf, setCpf] = useState("");
  const [roleId, setRoleId] = useState(1);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    if (editingEmployee) {
      setCompanyId(editingEmployee.companyId);
      setBranchId(editingEmployee.branchId);
      setName(editingEmployee.name);
      setEmail(editingEmployee.email);
      setPhone(editingEmployee.phone);
      setCpf(editingEmployee.cpf);
      setRoleId(editingEmployee.roleId);
    } else {
      setName("");
      setEmail("");
      setPhone("");
      setCpf("");
      setRoleId(1);
      setBranchId(defaultBranchId);
    }
    setError(null);
    setSuccess(null);
  }, [editingEmployee, defaultBranchId]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);
    try {
      if (isEditing) {
        await updateEmployee(editingEmployee.id, { name, email, phone, roleId });
        setSuccess("Funcionário atualizado com sucesso.");
      } else {
        await createEmployee({ companyId, branchId, name, email, phone, cpf, roleId });
        setSuccess("Funcionário cadastrado com sucesso.");
      }
      onDone();
    } catch (err) {
      setError(
        extractErrorMessage(
          err,
          isEditing ? "Não foi possível atualizar o funcionário." : "Não foi possível cadastrar o funcionário.",
        ),
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <div className="mb-3 flex items-center justify-between">
        <h2 className="text-sm font-semibold text-slate-700">
          {isEditing ? `Editar Funcionário — ${editingEmployee.name}` : "Cadastrar Funcionário"}
        </h2>
        {isEditing && (
          <button
            type="button"
            onClick={onCancelEdit}
            className="text-xs font-medium text-slate-500 underline hover:text-slate-800"
          >
            Cancelar edição
          </button>
        )}
      </div>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        {!isEditing && (
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className={labelClass} htmlFor="empCompanyId">
                Empresa (ID)
              </label>
              <input
                id="empCompanyId"
                type="number"
                min={1}
                value={companyId}
                onChange={(e) => setCompanyId(Number(e.target.value))}
                className={inputClass}
              />
            </div>
            <div>
              <label className={labelClass} htmlFor="empBranchId">
                Filial (ID)
              </label>
              <input
                id="empBranchId"
                type="number"
                min={1}
                value={branchId}
                onChange={(e) => setBranchId(Number(e.target.value))}
                className={inputClass}
              />
            </div>
          </div>
        )}
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="empName">
              Nome
            </label>
            <input
              id="empName"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="empEmail">
              Email
            </label>
            <input
              id="empEmail"
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>
        <div className="grid grid-cols-3 gap-3">
          <div>
            <label className={labelClass} htmlFor="empPhone">
              Telefone
            </label>
            <input
              id="empPhone"
              required
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="empCpf">
              CPF
            </label>
            <input
              id="empCpf"
              required
              disabled={isEditing}
              value={cpf}
              onChange={(e) => setCpf(e.target.value)}
              className={inputClass}
              placeholder="Só números, 11 dígitos"
              maxLength={11}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="empRoleId">
              Cargo (Role ID)
            </label>
            <input
              id="empRoleId"
              type="number"
              min={1}
              value={roleId}
              onChange={(e) => setRoleId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>

        {error && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {success && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{success}</div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Salvando..." : isEditing ? "Salvar Alterações" : "Cadastrar Funcionário"}
        </button>
      </form>
    </section>
  );
}
