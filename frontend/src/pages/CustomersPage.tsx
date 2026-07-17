import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import axios from "axios";
import {
  createCustomer,
  getCustomerByDocument,
  getAllCustomersByBranch,
  createVehicle,
  createAgreementContract,
  createMonthlyContract,
  createAgreementMerchant,
} from "../api/customer";
import ModelAutocomplete from "../components/ModelAutocomplete";
import type {
  CustomerDto,
  VehicleDto,
  AgreementCustomerContractDto,
  MonthlyCustomerContractDto,
  AgreementMerchantDto,
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
  "rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:opacity-60 self-start";

const CUSTOMER_TYPES = [
  { value: 1, label: "1 - Rotativo" },
  { value: 2, label: "2 - Convênio" },
  { value: 3, label: "3 - Mensalista" },
];

export default function CustomersPage() {
  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-bold text-slate-900">Clientes</h1>
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <CreateCustomerForm />
        <SearchCustomerForm />
      </div>
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <CreateVehicleForm />
        <CreateAgreementMerchantForm />
      </div>
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <CreateAgreementContractForm />
        <CreateMonthlyContractForm />
      </div>
    </div>
  );
}

function CreateCustomerForm() {
  const [branchId, setBranchId] = useState(1);
  const [customerType, setCustomerType] = useState(1);
  const [name, setName] = useState("");
  const [document, setDocument] = useState("");
  const [phone, setPhone] = useState("");
  const [email, setEmail] = useState("");

  const [result, setResult] = useState<CustomerDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const customer = await createCustomer({
        branchId,
        customerType,
        name,
        document,
        phone: phone || undefined,
        email: email || undefined,
      });
      setResult(customer);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o cliente."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Cliente</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="custBranchId">
              Filial (ID)
            </label>
            <input
              id="custBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="custType">
              Tipo
            </label>
            <select
              id="custType"
              value={customerType}
              onChange={(e) => setCustomerType(Number(e.target.value))}
              className={inputClass}
            >
              {CUSTOMER_TYPES.map((t) => (
                <option key={t.value} value={t.value}>
                  {t.label}
                </option>
              ))}
            </select>
          </div>
        </div>
        <div>
          <label className={labelClass} htmlFor="custName">
            Nome
          </label>
          <input
            id="custName"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="custDocument">
            Documento
          </label>
          <input
            id="custDocument"
            required
            value={document}
            onChange={(e) => setDocument(e.target.value)}
            className={inputClass}
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="custPhone">
              Telefone
            </label>
            <input
              id="custPhone"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="custEmail">
              E-mail
            </label>
            <input
              id="custEmail"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Cliente criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Cliente"}
        </button>
      </form>
    </section>
  );
}

function SearchCustomerForm() {
  const [document, setDocument] = useState("");
  const [result, setResult] = useState<CustomerDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const customer = await getCustomerByDocument(document);
      setResult(customer);
    } catch (err) {
      setError(extractErrorMessage(err, "Cliente não encontrado."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Buscar Cliente por Documento</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="searchDocument">
            Documento
          </label>
          <input
            id="searchDocument"
            required
            value={document}
            onChange={(e) => setDocument(e.target.value)}
            className={inputClass}
          />
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-slate-50 px-3 py-2 text-sm text-slate-700">
            <p>
              <strong>ID:</strong> {result.id}
            </p>
            <p>
              <strong>Nome:</strong> {result.name}
            </p>
            <p>
              <strong>Tipo:</strong> {result.customerType}
            </p>
            <p>
              <strong>Telefone:</strong> {result.phone ?? "-"}
            </p>
            <p>
              <strong>E-mail:</strong> {result.email ?? "-"}
            </p>
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Buscando..." : "Buscar"}
        </button>
      </form>
    </section>
  );
}

function CreateVehicleForm() {
  const [branchId, setBranchId] = useState(1);
  const [customerId, setCustomerId] = useState<number | "">("");
  const [licensePlate, setLicensePlate] = useState("");
  const [model, setModel] = useState("");
  const [color, setColor] = useState("");

  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [customersLoading, setCustomersLoading] = useState(false);
  const [customersError, setCustomersError] = useState<string | null>(null);

  const [result, setResult] = useState<VehicleDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setCustomersLoading(true);
    setCustomersError(null);
    getAllCustomersByBranch(branchId)
      .then((data) => {
        if (cancelled) return;
        setCustomers(data);
        setCustomerId((prev) => (prev === "" && data.length > 0 ? data[0].id : prev));
      })
      .catch((err) => {
        if (cancelled) return;
        setCustomersError(extractErrorMessage(err, "Não foi possível carregar os clientes."));
        setCustomers([]);
      })
      .finally(() => {
        if (!cancelled) setCustomersLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (customerId === "") {
      setError("Selecione um cliente.");
      return;
    }
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const vehicle = await createVehicle({
        customerId,
        licensePlate,
        model: model || undefined,
        color: color || undefined,
      });
      setResult(vehicle);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o veículo."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Veículo</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="vehBranchId">
              Filial (ID) — para listar clientes
            </label>
            <input
              id="vehBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="vehCustomerId">
              Cliente
            </label>
            <select
              id="vehCustomerId"
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value ? Number(e.target.value) : "")}
              className={inputClass}
              disabled={customersLoading}
            >
              <option value="">Selecione...</option>
              {customers.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name} - {c.document}
                </option>
              ))}
            </select>
            {customersError && <p className="mt-1 text-xs text-red-600">{customersError}</p>}
          </div>
        </div>
        <div>
          <label className={labelClass} htmlFor="vehPlate">
            Placa
          </label>
          <input
            id="vehPlate"
            required
            value={licensePlate}
            onChange={(e) => setLicensePlate(e.target.value)}
            className={inputClass}
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="vehModel">
              Modelo
            </label>
            <ModelAutocomplete
              id="vehModel"
              value={model}
              onChange={setModel}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="vehColor">
              Cor
            </label>
            <input
              id="vehColor"
              value={color}
              onChange={(e) => setColor(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Veículo criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Veículo"}
        </button>
      </form>
    </section>
  );
}

function CreateAgreementMerchantForm() {
  const [branchId, setBranchId] = useState(1);
  const [companyName, setCompanyName] = useState("");
  const [discountPercentage, setDiscountPercentage] = useState(0);

  const [result, setResult] = useState<AgreementMerchantDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const merchant = await createAgreementMerchant({
        branchId,
        companyName,
        discountPercentage,
      });
      setResult(merchant);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar a empresa conveniada."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Empresa Conveniada</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="merchantBranchId">
            Filial (ID)
          </label>
          <input
            id="merchantBranchId"
            type="number"
            min={1}
            value={branchId}
            onChange={(e) => setBranchId(Number(e.target.value))}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="merchantName">
            Nome da Empresa
          </label>
          <input
            id="merchantName"
            required
            value={companyName}
            onChange={(e) => setCompanyName(e.target.value)}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="merchantDiscount">
            Desconto (%)
          </label>
          <input
            id="merchantDiscount"
            type="number"
            min={0}
            max={100}
            step="0.01"
            value={discountPercentage}
            onChange={(e) => setDiscountPercentage(Number(e.target.value))}
            className={inputClass}
          />
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Empresa criada com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Empresa"}
        </button>
      </form>
    </section>
  );
}

function CreateAgreementContractForm() {
  const [customerId, setCustomerId] = useState(1);
  const [agreementMerchantId, setAgreementMerchantId] = useState(1);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");

  const [result, setResult] = useState<AgreementCustomerContractDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const contract = await createAgreementContract({
        customerId,
        agreementMerchantId,
        startDate,
        endDate,
      });
      setResult(contract);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o contrato de convênio."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Contrato Convênio</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="agrCustomerId">
              Cliente (ID)
            </label>
            <input
              id="agrCustomerId"
              type="number"
              min={1}
              value={customerId}
              onChange={(e) => setCustomerId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="agrMerchantId">
              Empresa Conveniada (ID)
            </label>
            <input
              id="agrMerchantId"
              type="number"
              min={1}
              value={agreementMerchantId}
              onChange={(e) => setAgreementMerchantId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="agrStartDate">
              Início
            </label>
            <input
              id="agrStartDate"
              type="date"
              required
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="agrEndDate">
              Fim
            </label>
            <input
              id="agrEndDate"
              type="date"
              required
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Contrato criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Contrato de Convênio"}
        </button>
      </form>
    </section>
  );
}

function CreateMonthlyContractForm() {
  const [customerId, setCustomerId] = useState(1);
  const [branchId, setBranchId] = useState(1);
  const [monthlyFee, setMonthlyFee] = useState(0);
  const [maxVehicles, setMaxVehicles] = useState(1);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");

  const [result, setResult] = useState<MonthlyCustomerContractDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const contract = await createMonthlyContract({
        customerId,
        branchId,
        monthlyFee,
        maxVehicles,
        startDate,
        endDate,
      });
      setResult(contract);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o contrato mensalista."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Contrato Mensalista</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="monthCustomerId">
              Cliente (ID)
            </label>
            <input
              id="monthCustomerId"
              type="number"
              min={1}
              value={customerId}
              onChange={(e) => setCustomerId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="monthBranchId">
              Filial (ID)
            </label>
            <input
              id="monthBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="monthFee">
              Mensalidade (R$)
            </label>
            <input
              id="monthFee"
              type="number"
              min={0}
              step="0.01"
              value={monthlyFee}
              onChange={(e) => setMonthlyFee(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="monthMaxVehicles">
              Máx. de Veículos
            </label>
            <input
              id="monthMaxVehicles"
              type="number"
              min={1}
              value={maxVehicles}
              onChange={(e) => setMaxVehicles(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="monthStartDate">
              Início
            </label>
            <input
              id="monthStartDate"
              type="date"
              required
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="monthEndDate">
              Fim
            </label>
            <input
              id="monthEndDate"
              type="date"
              required
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Contrato criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Contrato Mensalista"}
        </button>
      </form>
    </section>
  );
}
