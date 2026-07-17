import { useEffect, useState } from "react";
import type { FormEvent, ReactNode } from "react";
import axios from "axios";
import {
  createSupplier,
  getAllSuppliersByBranch,
  updateSupplier,
  deactivateSupplier,
} from "../api/inventory";
import {
  createServiceCategory,
  getServiceCategories,
  updateServiceCategory,
  deactivateServiceCategory,
  createServiceItem,
  getServiceItems,
  updateServiceItem,
  deactivateServiceItem,
} from "../api/wash";
import {
  createAgreementMerchant,
  getAllAgreementMerchantsByBranch,
  updateAgreementMerchant,
  deactivateAgreementMerchant,
  createTariff,
  getAllTariffsByBranch,
  updateTariff,
  deactivateTariff,
  createBranch,
  getAllBranchesByCompany,
  updateBranch,
  deactivateBranch,
} from "../api/cadastros";
import type {
  SupplierDto,
  ServiceCategoryDto,
  ServiceItemDto,
  AgreementMerchantDto,
  TariffDto,
  BranchDto,
} from "../types/api";

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

const TABS = [
  { key: "suppliers", label: "Fornecedores" },
  { key: "categories", label: "Categorias de Serviço" },
  { key: "items", label: "Itens de Serviço" },
  { key: "agreements", label: "Convênios" },
  { key: "tariffs", label: "Tarifas" },
  { key: "branches", label: "Filiais" },
] as const;

type TabKey = (typeof TABS)[number]["key"];

export default function CadastrosPage() {
  const [tab, setTab] = useState<TabKey>("suppliers");

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-bold text-slate-900">Cadastros</h1>

      <div className="flex flex-wrap gap-1 border-b border-slate-200">
        {TABS.map((t) => (
          <button
            key={t.key}
            type="button"
            onClick={() => setTab(t.key)}
            className={`rounded-t-md px-4 py-2 text-sm font-medium transition ${
              tab === t.key
                ? "border-b-2 border-slate-900 text-slate-900"
                : "text-slate-500 hover:text-slate-800"
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {tab === "suppliers" && <SuppliersTab />}
      {tab === "categories" && <ServiceCategoriesTab />}
      {tab === "items" && <ServiceItemsTab />}
      {tab === "agreements" && <AgreementMerchantsTab />}
      {tab === "tariffs" && <TariffsTab />}
      {tab === "branches" && <BranchesTab />}
    </div>
  );
}

// ===== Fornecedores =====
function SuppliersTab() {
  const [branchId, setBranchId] = useState(1);
  const [items, setItems] = useState<SupplierDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshCount, setRefreshCount] = useState(0);
  const [editing, setEditing] = useState<SupplierDto | null>(null);

  const [name, setName] = useState("");
  const [document, setDocument] = useState("");
  const [phone, setPhone] = useState("");
  const [email, setEmail] = useState("");
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [formSuccess, setFormSuccess] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);
    getAllSuppliersByBranch(branchId)
      .then((data) => !cancelled && setItems(data))
      .catch((err) => {
        if (!cancelled) {
          setError(extractErrorMessage(err, "Não foi possível carregar os fornecedores."));
          setItems([]);
        }
      })
      .finally(() => !cancelled && setLoading(false));
    return () => {
      cancelled = true;
    };
  }, [branchId, refreshCount]);

  useEffect(() => {
    if (editing) {
      setName(editing.name);
      setDocument(editing.document);
      setPhone(editing.phone ?? "");
      setEmail(editing.email ?? "");
    } else {
      setName("");
      setDocument("");
      setPhone("");
      setEmail("");
    }
    setFormError(null);
    setFormSuccess(null);
  }, [editing]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setFormLoading(true);
    setFormError(null);
    setFormSuccess(null);
    try {
      const payload = { branchId, name, document, phone: phone || undefined, email: email || undefined };
      if (editing) {
        await updateSupplier(editing.id, payload);
        setFormSuccess("Fornecedor atualizado.");
      } else {
        await createSupplier(payload);
        setFormSuccess("Fornecedor cadastrado.");
      }
      setEditing(null);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      setFormError(extractErrorMessage(err, "Não foi possível salvar o fornecedor."));
    } finally {
      setFormLoading(false);
    }
  }

  async function handleDeactivate(item: SupplierDto) {
    if (!confirm(`Desativar o fornecedor "${item.name}"?`)) return;
    try {
      await deactivateSupplier(item.id);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      alert(extractErrorMessage(err, "Não foi possível desativar."));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <section className={sectionClass}>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-700">
            {editing ? `Editar — ${editing.name}` : "Cadastrar Fornecedor"}
          </h2>
          {editing && (
            <button type="button" onClick={() => setEditing(null)} className="text-xs font-medium text-slate-500 underline">
              Cancelar edição
            </button>
          )}
        </div>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className={labelClass}>Filial (ID)</label>
              <input type="number" min={1} value={branchId} onChange={(e) => setBranchId(Number(e.target.value))} className={inputClass} disabled={!!editing} />
            </div>
            <div>
              <label className={labelClass}>Nome</label>
              <input required value={name} onChange={(e) => setName(e.target.value)} className={inputClass} />
            </div>
          </div>
          <div className="grid grid-cols-3 gap-3">
            <div>
              <label className={labelClass}>Documento</label>
              <input required value={document} onChange={(e) => setDocument(e.target.value)} className={inputClass} />
            </div>
            <div>
              <label className={labelClass}>Telefone</label>
              <input value={phone} onChange={(e) => setPhone(e.target.value)} className={inputClass} />
            </div>
            <div>
              <label className={labelClass}>Email</label>
              <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} className={inputClass} />
            </div>
          </div>
          {formError && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{formError}</div>}
          {formSuccess && <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{formSuccess}</div>}
          <button type="submit" disabled={formLoading} className={buttonClass}>
            {formLoading ? "Salvando..." : editing ? "Salvar Alterações" : "Cadastrar"}
          </button>
        </form>
      </section>

      <section className={sectionClass}>
        <h2 className="mb-3 text-sm font-semibold text-slate-700">Lista de Fornecedores</h2>
        {error && <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {loading ? (
          <p className="text-sm text-slate-400">Carregando...</p>
        ) : items.length === 0 ? (
          <p className="text-sm text-slate-400">Nenhum fornecedor cadastrado.</p>
        ) : (
          <SimpleTable
            rows={items}
            columns={[
              { header: "Nome", render: (i) => i.name },
              { header: "Documento", render: (i) => i.document },
              { header: "Telefone", render: (i) => i.phone ?? "-" },
              { header: "Email", render: (i) => i.email ?? "-" },
            ]}
            onEdit={setEditing}
            onDeactivate={handleDeactivate}
          />
        )}
      </section>
    </div>
  );
}

// ===== Categorias de Serviço =====
function ServiceCategoriesTab() {
  const [branchId, setBranchId] = useState(1);
  const [items, setItems] = useState<ServiceCategoryDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshCount, setRefreshCount] = useState(0);
  const [editing, setEditing] = useState<ServiceCategoryDto | null>(null);

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [formSuccess, setFormSuccess] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);
    getServiceCategories(branchId)
      .then((data) => !cancelled && setItems(data))
      .catch((err) => {
        if (!cancelled) {
          setError(extractErrorMessage(err, "Não foi possível carregar as categorias."));
          setItems([]);
        }
      })
      .finally(() => !cancelled && setLoading(false));
    return () => {
      cancelled = true;
    };
  }, [branchId, refreshCount]);

  useEffect(() => {
    if (editing) {
      setName(editing.name);
      setDescription(editing.description ?? "");
    } else {
      setName("");
      setDescription("");
    }
    setFormError(null);
    setFormSuccess(null);
  }, [editing]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setFormLoading(true);
    setFormError(null);
    setFormSuccess(null);
    try {
      if (editing) {
        await updateServiceCategory(editing.id, { name, description: description || undefined });
        setFormSuccess("Categoria atualizada.");
      } else {
        await createServiceCategory({ branchId, name, description: description || undefined });
        setFormSuccess("Categoria cadastrada.");
      }
      setEditing(null);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      setFormError(extractErrorMessage(err, "Não foi possível salvar a categoria."));
    } finally {
      setFormLoading(false);
    }
  }

  async function handleDeactivate(item: ServiceCategoryDto) {
    if (!confirm(`Desativar a categoria "${item.name}"?`)) return;
    try {
      await deactivateServiceCategory(item.id);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      alert(extractErrorMessage(err, "Não foi possível desativar."));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <section className={sectionClass}>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-700">
            {editing ? `Editar — ${editing.name}` : "Cadastrar Categoria de Serviço"}
          </h2>
          {editing && (
            <button type="button" onClick={() => setEditing(null)} className="text-xs font-medium text-slate-500 underline">
              Cancelar edição
            </button>
          )}
        </div>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className={labelClass}>Filial (ID)</label>
              <input type="number" min={1} value={branchId} onChange={(e) => setBranchId(Number(e.target.value))} className={inputClass} disabled={!!editing} />
            </div>
            <div>
              <label className={labelClass}>Nome</label>
              <input required value={name} onChange={(e) => setName(e.target.value)} className={inputClass} />
            </div>
          </div>
          <div>
            <label className={labelClass}>Descrição</label>
            <input value={description} onChange={(e) => setDescription(e.target.value)} className={inputClass} />
          </div>
          {formError && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{formError}</div>}
          {formSuccess && <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{formSuccess}</div>}
          <button type="submit" disabled={formLoading} className={buttonClass}>
            {formLoading ? "Salvando..." : editing ? "Salvar Alterações" : "Cadastrar"}
          </button>
        </form>
      </section>

      <section className={sectionClass}>
        <h2 className="mb-3 text-sm font-semibold text-slate-700">Lista de Categorias</h2>
        {error && <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {loading ? (
          <p className="text-sm text-slate-400">Carregando...</p>
        ) : items.length === 0 ? (
          <p className="text-sm text-slate-400">Nenhuma categoria cadastrada.</p>
        ) : (
          <SimpleTable
            rows={items}
            columns={[
              { header: "Nome", render: (i) => i.name },
              { header: "Descrição", render: (i) => i.description ?? "-" },
            ]}
            onEdit={setEditing}
            onDeactivate={handleDeactivate}
          />
        )}
      </section>
    </div>
  );
}

// ===== Itens de Serviço =====
function ServiceItemsTab() {
  const [categoryId, setCategoryId] = useState(1);
  const [items, setItems] = useState<ServiceItemDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshCount, setRefreshCount] = useState(0);
  const [editing, setEditing] = useState<ServiceItemDto | null>(null);

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [durationMinutes, setDurationMinutes] = useState(30);
  const [baseCost, setBaseCost] = useState(0);
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [formSuccess, setFormSuccess] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);
    getServiceItems(categoryId)
      .then((data) => !cancelled && setItems(data))
      .catch((err) => {
        if (!cancelled) {
          setError(extractErrorMessage(err, "Não foi possível carregar os itens."));
          setItems([]);
        }
      })
      .finally(() => !cancelled && setLoading(false));
    return () => {
      cancelled = true;
    };
  }, [categoryId, refreshCount]);

  useEffect(() => {
    if (editing) {
      setName(editing.name);
      setDescription(editing.description ?? "");
      setDurationMinutes(editing.durationMinutes);
      setBaseCost(editing.baseCost);
    } else {
      setName("");
      setDescription("");
      setDurationMinutes(30);
      setBaseCost(0);
    }
    setFormError(null);
    setFormSuccess(null);
  }, [editing]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setFormLoading(true);
    setFormError(null);
    setFormSuccess(null);
    try {
      if (editing) {
        await updateServiceItem(editing.id, { name, description: description || undefined, durationMinutes, baseCost });
        setFormSuccess("Item atualizado.");
      } else {
        await createServiceItem({ serviceCategoryId: categoryId, name, description: description || undefined, durationMinutes, baseCost });
        setFormSuccess("Item cadastrado.");
      }
      setEditing(null);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      setFormError(extractErrorMessage(err, "Não foi possível salvar o item."));
    } finally {
      setFormLoading(false);
    }
  }

  async function handleDeactivate(item: ServiceItemDto) {
    if (!confirm(`Desativar o item "${item.name}"?`)) return;
    try {
      await deactivateServiceItem(item.id);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      alert(extractErrorMessage(err, "Não foi possível desativar."));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <section className={sectionClass}>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-700">
            {editing ? `Editar — ${editing.name}` : "Cadastrar Item de Serviço"}
          </h2>
          {editing && (
            <button type="button" onClick={() => setEditing(null)} className="text-xs font-medium text-slate-500 underline">
              Cancelar edição
            </button>
          )}
        </div>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className={labelClass}>Categoria (ID)</label>
              <input type="number" min={1} value={categoryId} onChange={(e) => setCategoryId(Number(e.target.value))} className={inputClass} disabled={!!editing} />
            </div>
            <div>
              <label className={labelClass}>Nome</label>
              <input required value={name} onChange={(e) => setName(e.target.value)} className={inputClass} />
            </div>
          </div>
          <div>
            <label className={labelClass}>Descrição</label>
            <input value={description} onChange={(e) => setDescription(e.target.value)} className={inputClass} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className={labelClass}>Duração (min)</label>
              <input type="number" min={1} required value={durationMinutes} onChange={(e) => setDurationMinutes(Number(e.target.value))} className={inputClass} />
            </div>
            <div>
              <label className={labelClass}>Custo Base (R$)</label>
              <input type="number" min={0} step="0.01" required value={baseCost} onChange={(e) => setBaseCost(Number(e.target.value))} className={inputClass} />
            </div>
          </div>
          {formError && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{formError}</div>}
          {formSuccess && <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{formSuccess}</div>}
          <button type="submit" disabled={formLoading} className={buttonClass}>
            {formLoading ? "Salvando..." : editing ? "Salvar Alterações" : "Cadastrar"}
          </button>
        </form>
      </section>

      <section className={sectionClass}>
        <h2 className="mb-3 text-sm font-semibold text-slate-700">Lista de Itens</h2>
        {error && <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {loading ? (
          <p className="text-sm text-slate-400">Carregando...</p>
        ) : items.length === 0 ? (
          <p className="text-sm text-slate-400">Nenhum item cadastrado para esta categoria.</p>
        ) : (
          <SimpleTable
            rows={items}
            columns={[
              { header: "Nome", render: (i) => i.name },
              { header: "Duração", render: (i) => `${i.durationMinutes} min` },
              { header: "Custo Base", render: (i) => i.baseCost.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) },
            ]}
            onEdit={setEditing}
            onDeactivate={handleDeactivate}
          />
        )}
      </section>
    </div>
  );
}

// ===== Convênios =====
function AgreementMerchantsTab() {
  const [branchId, setBranchId] = useState(1);
  const [items, setItems] = useState<AgreementMerchantDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshCount, setRefreshCount] = useState(0);
  const [editing, setEditing] = useState<AgreementMerchantDto | null>(null);

  const [companyName, setCompanyName] = useState("");
  const [discountPercentage, setDiscountPercentage] = useState(0);
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [formSuccess, setFormSuccess] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);
    getAllAgreementMerchantsByBranch(branchId)
      .then((data) => !cancelled && setItems(data))
      .catch((err) => {
        if (!cancelled) {
          setError(extractErrorMessage(err, "Não foi possível carregar os convênios."));
          setItems([]);
        }
      })
      .finally(() => !cancelled && setLoading(false));
    return () => {
      cancelled = true;
    };
  }, [branchId, refreshCount]);

  useEffect(() => {
    if (editing) {
      setCompanyName(editing.companyName);
      setDiscountPercentage(editing.discountPercentage);
    } else {
      setCompanyName("");
      setDiscountPercentage(0);
    }
    setFormError(null);
    setFormSuccess(null);
  }, [editing]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setFormLoading(true);
    setFormError(null);
    setFormSuccess(null);
    try {
      if (editing) {
        await updateAgreementMerchant(editing.id, { companyName, discountPercentage });
        setFormSuccess("Convênio atualizado.");
      } else {
        await createAgreementMerchant({ branchId, companyName, discountPercentage });
        setFormSuccess("Convênio cadastrado.");
      }
      setEditing(null);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      setFormError(extractErrorMessage(err, "Não foi possível salvar o convênio."));
    } finally {
      setFormLoading(false);
    }
  }

  async function handleDeactivate(item: AgreementMerchantDto) {
    if (!confirm(`Desativar o convênio "${item.companyName}"?`)) return;
    try {
      await deactivateAgreementMerchant(item.id);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      alert(extractErrorMessage(err, "Não foi possível desativar."));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <section className={sectionClass}>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-700">
            {editing ? `Editar — ${editing.companyName}` : "Cadastrar Convênio"}
          </h2>
          {editing && (
            <button type="button" onClick={() => setEditing(null)} className="text-xs font-medium text-slate-500 underline">
              Cancelar edição
            </button>
          )}
        </div>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          <div className="grid grid-cols-3 gap-3">
            <div>
              <label className={labelClass}>Filial (ID)</label>
              <input type="number" min={1} value={branchId} onChange={(e) => setBranchId(Number(e.target.value))} className={inputClass} disabled={!!editing} />
            </div>
            <div>
              <label className={labelClass}>Empresa Conveniada</label>
              <input required value={companyName} onChange={(e) => setCompanyName(e.target.value)} className={inputClass} />
            </div>
            <div>
              <label className={labelClass}>Desconto (%)</label>
              <input type="number" min={0} max={100} step="0.01" required value={discountPercentage} onChange={(e) => setDiscountPercentage(Number(e.target.value))} className={inputClass} />
            </div>
          </div>
          {formError && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{formError}</div>}
          {formSuccess && <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{formSuccess}</div>}
          <button type="submit" disabled={formLoading} className={buttonClass}>
            {formLoading ? "Salvando..." : editing ? "Salvar Alterações" : "Cadastrar"}
          </button>
        </form>
      </section>

      <section className={sectionClass}>
        <h2 className="mb-3 text-sm font-semibold text-slate-700">Lista de Convênios</h2>
        {error && <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {loading ? (
          <p className="text-sm text-slate-400">Carregando...</p>
        ) : items.length === 0 ? (
          <p className="text-sm text-slate-400">Nenhum convênio cadastrado.</p>
        ) : (
          <SimpleTable
            rows={items}
            columns={[
              { header: "Empresa", render: (i) => i.companyName },
              { header: "Desconto", render: (i) => `${i.discountPercentage}%` },
            ]}
            onEdit={setEditing}
            onDeactivate={handleDeactivate}
          />
        )}
      </section>
    </div>
  );
}

// ===== Tarifas =====
function TariffsTab() {
  const [branchId, setBranchId] = useState(1);
  const [items, setItems] = useState<TariffDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshCount, setRefreshCount] = useState(0);
  const [editing, setEditing] = useState<TariffDto | null>(null);

  const [firstHourRate, setFirstHourRate] = useState(0);
  const [additionalHourRate, setAdditionalHourRate] = useState(0);
  const [dailyMaxRate, setDailyMaxRate] = useState<number | "">("");
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [formSuccess, setFormSuccess] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);
    getAllTariffsByBranch(branchId)
      .then((data) => !cancelled && setItems(data))
      .catch((err) => {
        if (!cancelled) {
          setError(extractErrorMessage(err, "Não foi possível carregar as tarifas."));
          setItems([]);
        }
      })
      .finally(() => !cancelled && setLoading(false));
    return () => {
      cancelled = true;
    };
  }, [branchId, refreshCount]);

  useEffect(() => {
    if (editing) {
      setFirstHourRate(editing.firstHourRate);
      setAdditionalHourRate(editing.additionalHourRate);
      setDailyMaxRate(editing.dailyMaxRate ?? "");
    } else {
      setFirstHourRate(0);
      setAdditionalHourRate(0);
      setDailyMaxRate("");
    }
    setFormError(null);
    setFormSuccess(null);
  }, [editing]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setFormLoading(true);
    setFormError(null);
    setFormSuccess(null);
    try {
      const payload = {
        firstHourRate,
        additionalHourRate,
        dailyMaxRate: dailyMaxRate === "" ? null : dailyMaxRate,
      };
      if (editing) {
        await updateTariff(editing.id, payload);
        setFormSuccess("Tarifa atualizada.");
      } else {
        await createTariff({ branchId, ...payload });
        setFormSuccess("Tarifa cadastrada.");
      }
      setEditing(null);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      setFormError(extractErrorMessage(err, "Não foi possível salvar a tarifa."));
    } finally {
      setFormLoading(false);
    }
  }

  async function handleDeactivate(item: TariffDto) {
    if (!confirm(`Desativar esta tarifa (#${item.id})?`)) return;
    try {
      await deactivateTariff(item.id);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      alert(extractErrorMessage(err, "Não foi possível desativar."));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <section className={sectionClass}>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-700">
            {editing ? `Editar Tarifa #${editing.id}` : "Cadastrar Tarifa"}
          </h2>
          {editing && (
            <button type="button" onClick={() => setEditing(null)} className="text-xs font-medium text-slate-500 underline">
              Cancelar edição
            </button>
          )}
        </div>
        <p className="mb-3 text-xs text-slate-500">
          O sistema usa sempre a tarifa ATIVA da filial. Para trocar de tarifa, cadastre uma nova e
          desative a anterior.
        </p>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          <div className="grid grid-cols-4 gap-3">
            <div>
              <label className={labelClass}>Filial (ID)</label>
              <input type="number" min={1} value={branchId} onChange={(e) => setBranchId(Number(e.target.value))} className={inputClass} disabled={!!editing} />
            </div>
            <div>
              <label className={labelClass}>1ª Hora (R$)</label>
              <input type="number" min={0} step="0.01" required value={firstHourRate} onChange={(e) => setFirstHourRate(Number(e.target.value))} className={inputClass} />
            </div>
            <div>
              <label className={labelClass}>Hora Adicional (R$)</label>
              <input type="number" min={0} step="0.01" required value={additionalHourRate} onChange={(e) => setAdditionalHourRate(Number(e.target.value))} className={inputClass} />
            </div>
            <div>
              <label className={labelClass}>Teto Diário (R$, opcional)</label>
              <input type="number" min={0} step="0.01" value={dailyMaxRate} onChange={(e) => setDailyMaxRate(e.target.value ? Number(e.target.value) : "")} className={inputClass} />
            </div>
          </div>
          {formError && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{formError}</div>}
          {formSuccess && <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{formSuccess}</div>}
          <button type="submit" disabled={formLoading} className={buttonClass}>
            {formLoading ? "Salvando..." : editing ? "Salvar Alterações" : "Cadastrar"}
          </button>
        </form>
      </section>

      <section className={sectionClass}>
        <h2 className="mb-3 text-sm font-semibold text-slate-700">Lista de Tarifas</h2>
        {error && <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {loading ? (
          <p className="text-sm text-slate-400">Carregando...</p>
        ) : items.length === 0 ? (
          <p className="text-sm text-slate-400">Nenhuma tarifa cadastrada.</p>
        ) : (
          <SimpleTable
            rows={items}
            columns={[
              { header: "1ª Hora", render: (i) => i.firstHourRate.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) },
              { header: "Hora Adicional", render: (i) => i.additionalHourRate.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) },
              { header: "Teto Diário", render: (i) => (i.dailyMaxRate != null ? i.dailyMaxRate.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) : "-") },
            ]}
            onEdit={setEditing}
            onDeactivate={handleDeactivate}
          />
        )}
      </section>
    </div>
  );
}

// ===== Filiais =====
function BranchesTab() {
  const [companyId, setCompanyId] = useState(1);
  const [items, setItems] = useState<BranchDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshCount, setRefreshCount] = useState(0);
  const [editing, setEditing] = useState<BranchDto | null>(null);

  const [name, setName] = useState("");
  const [address, setAddress] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [totalSpaces, setTotalSpaces] = useState(10);
  const [formLoading, setFormLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [formSuccess, setFormSuccess] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);
    getAllBranchesByCompany(companyId)
      .then((data) => !cancelled && setItems(data))
      .catch((err) => {
        if (!cancelled) {
          setError(extractErrorMessage(err, "Não foi possível carregar as filiais."));
          setItems([]);
        }
      })
      .finally(() => !cancelled && setLoading(false));
    return () => {
      cancelled = true;
    };
  }, [companyId, refreshCount]);

  useEffect(() => {
    if (editing) {
      setName(editing.name);
      setAddress("");
      setPhoneNumber("");
      setTotalSpaces(editing.totalSpaces);
    } else {
      setName("");
      setAddress("");
      setPhoneNumber("");
      setTotalSpaces(10);
    }
    setFormError(null);
    setFormSuccess(null);
  }, [editing]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setFormLoading(true);
    setFormError(null);
    setFormSuccess(null);
    try {
      if (editing) {
        await updateBranch(editing.id, { name, address, phoneNumber: phoneNumber || undefined, totalSpaces });
        setFormSuccess("Filial atualizada.");
      } else {
        await createBranch({ companyId, name, address, totalSpaces });
        setFormSuccess("Filial cadastrada.");
      }
      setEditing(null);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      setFormError(extractErrorMessage(err, "Não foi possível salvar a filial."));
    } finally {
      setFormLoading(false);
    }
  }

  async function handleDeactivate(item: BranchDto) {
    if (!confirm(`Desativar a filial "${item.name}"?`)) return;
    try {
      await deactivateBranch(item.id);
      setRefreshCount((v) => v + 1);
    } catch (err) {
      alert(extractErrorMessage(err, "Não foi possível desativar."));
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <section className={sectionClass}>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-700">
            {editing ? `Editar — ${editing.name}` : "Cadastrar Filial"}
          </h2>
          {editing && (
            <button type="button" onClick={() => setEditing(null)} className="text-xs font-medium text-slate-500 underline">
              Cancelar edição
            </button>
          )}
        </div>
        {editing && (
          <p className="mb-3 text-xs text-amber-700">
            Endereço e telefone não são retornados pela API ao listar — preencha novamente se quiser
            alterá-los; caso contrário, apenas nome e total de vagas serão atualizados de forma
            confiável.
          </p>
        )}
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className={labelClass}>Empresa (ID)</label>
              <input type="number" min={1} value={companyId} onChange={(e) => setCompanyId(Number(e.target.value))} className={inputClass} disabled={!!editing} />
            </div>
            <div>
              <label className={labelClass}>Nome</label>
              <input required value={name} onChange={(e) => setName(e.target.value)} className={inputClass} />
            </div>
          </div>
          <div>
            <label className={labelClass}>Endereço</label>
            <input required value={address} onChange={(e) => setAddress(e.target.value)} className={inputClass} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className={labelClass}>Telefone</label>
              <input value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} className={inputClass} />
            </div>
            <div>
              <label className={labelClass}>Total de Vagas</label>
              <input type="number" min={1} required value={totalSpaces} onChange={(e) => setTotalSpaces(Number(e.target.value))} className={inputClass} />
            </div>
          </div>
          {formError && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{formError}</div>}
          {formSuccess && <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{formSuccess}</div>}
          <button type="submit" disabled={formLoading} className={buttonClass}>
            {formLoading ? "Salvando..." : editing ? "Salvar Alterações" : "Cadastrar"}
          </button>
        </form>
      </section>

      <section className={sectionClass}>
        <h2 className="mb-3 text-sm font-semibold text-slate-700">Lista de Filiais</h2>
        {error && <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {loading ? (
          <p className="text-sm text-slate-400">Carregando...</p>
        ) : items.length === 0 ? (
          <p className="text-sm text-slate-400">Nenhuma filial cadastrada.</p>
        ) : (
          <SimpleTable
            rows={items}
            columns={[
              { header: "Nome", render: (i) => i.name },
              { header: "Total de Vagas", render: (i) => i.totalSpaces },
            ]}
            onEdit={setEditing}
            onDeactivate={handleDeactivate}
          />
        )}
      </section>
    </div>
  );
}

// ===== Tabela genérica reutilizada por todas as abas =====
interface Column<T> {
  header: string;
  render: (item: T) => ReactNode;
}

function SimpleTable<T extends { id: number; isActive: boolean }>({
  rows,
  columns,
  onEdit,
  onDeactivate,
}: {
  rows: T[];
  columns: Column<T>[];
  onEdit: (item: T) => void;
  onDeactivate: (item: T) => void;
}) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left text-sm">
        <thead>
          <tr className="border-b border-slate-200 text-xs uppercase text-slate-500">
            {columns.map((c) => (
              <th key={c.header} className="py-2 pr-3">
                {c.header}
              </th>
            ))}
            <th className="py-2 pr-3">Status</th>
            <th className="py-2 pr-3">Ações</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row.id} className="border-b border-slate-100">
              {columns.map((c) => (
                <td key={c.header} className="py-2 pr-3 text-slate-700">
                  {c.render(row)}
                </td>
              ))}
              <td className="py-2 pr-3">
                {row.isActive ? (
                  <span className="rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-medium text-emerald-700">
                    Ativo
                  </span>
                ) : (
                  <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-500">
                    Inativo
                  </span>
                )}
              </td>
              <td className="py-2 pr-3">
                <div className="flex gap-2">
                  <button type="button" onClick={() => onEdit(row)} className="text-xs font-medium text-slate-600 underline hover:text-slate-900">
                    Editar
                  </button>
                  {row.isActive && (
                    <button type="button" onClick={() => onDeactivate(row)} className="text-xs font-medium text-red-600 underline hover:text-red-800">
                      Desativar
                    </button>
                  )}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
