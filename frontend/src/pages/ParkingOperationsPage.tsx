import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import axios from "axios";
import {
  getParkingSpaceOccupancy,
  getAllParkingSpacesByBranch,
  createParkingSpace,
  getOpenVehicleEntriesByBranch,
  getOpenCashRegisterByBranch,
  registerVehicleEntry,
  registerVehicleEntryByPlate,
  registerVehicleExit,
  openCashRegister,
  recordCashMovement,
  closeCashRegister,
} from "../api/vehicleEntry";
import { getAllCustomersByBranch } from "../api/customer";
import ModelAutocomplete from "../components/ModelAutocomplete";
import type {
  ParkingSpaceOccupancyDto,
  ParkingSpaceDto,
  VehicleEntryDto,
  VehicleEntryByPlateResultDto,
  VehicleExitDto,
  CashRegisterDto,
  CashMovementDto,
  CustomerDto,
} from "../types/api";

const CUSTOMER_TYPE_LABELS: Record<number, string> = {
  1: "Avulso/Rotativo",
  2: "Convênio",
  3: "Mensalista",
};

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

const SPACE_TYPES = [
  { value: 1, label: "1 - Coberta" },
  { value: 2, label: "2 - Descoberta" },
  { value: 3, label: "3 - Reservada" },
  { value: 4, label: "4 - PCD" },
];

export default function ParkingOperationsPage() {
  // Incrementado sempre que uma vaga é criada, para que EntrySection e
  // OccupancySection recarreguem a lista/ocupação sem precisar duplicar
  // o fetch aqui em cima (cada seção continua responsável pelo próprio estado).
  const [spacesVersion, setSpacesVersion] = useState(0);

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-bold text-slate-900">Operação de Pátio</h1>
      <OccupancySection
        spacesVersion={spacesVersion}
        onSpaceCreated={() => setSpacesVersion((v) => v + 1)}
      />
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <QuickEntrySection
          spacesVersion={spacesVersion}
          onEntryRegistered={() => setSpacesVersion((v) => v + 1)}
        />
        <ExitSection />
      </div>
      <AdvancedEntrySection
        spacesVersion={spacesVersion}
        onEntryRegistered={() => setSpacesVersion((v) => v + 1)}
      />
      <CashRegisterSection />
    </div>
  );
}

function OccupancySection({
  spacesVersion,
  onSpaceCreated,
}: {
  spacesVersion: number;
  onSpaceCreated: () => void;
}) {
  const [branchId, setBranchId] = useState(1);
  const [occupancy, setOccupancy] = useState<ParkingSpaceOccupancyDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [spaceNumber, setSpaceNumber] = useState("");
  const [spaceType, setSpaceType] = useState(1);
  const [createLoading, setCreateLoading] = useState(false);
  const [createError, setCreateError] = useState<string | null>(null);
  const [createSuccess, setCreateSuccess] = useState<string | null>(null);

  async function loadOccupancy() {
    setLoading(true);
    setError(null);
    try {
      const data = await getParkingSpaceOccupancy(branchId);
      setOccupancy(data);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível carregar a ocupação da filial."));
      setOccupancy(null);
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    await loadOccupancy();
  }

  // Recarrega automaticamente a ocupação quando uma vaga é criada (nesta ou
  // em outra sessão desta página), sem exigir que o usuário clique de novo
  // em "Consultar".
  useEffect(() => {
    if (occupancy !== null) {
      loadOccupancy();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [spacesVersion]);

  async function handleCreateSpace(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!spaceNumber.trim()) {
      setCreateError("Informe o número/identificação da vaga.");
      return;
    }
    setCreateLoading(true);
    setCreateError(null);
    setCreateSuccess(null);
    try {
      const space = await createParkingSpace({
        branchId,
        spaceNumber: spaceNumber.trim(),
        type: spaceType,
      });
      setCreateSuccess(`Vaga ${space.spaceNumber} cadastrada com sucesso.`);
      setSpaceNumber("");
      onSpaceCreated();
    } catch (err) {
      setCreateError(extractErrorMessage(err, "Não foi possível cadastrar a vaga."));
    } finally {
      setCreateLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Vagas / Ocupação</h2>
      <form className="flex flex-wrap items-end gap-4" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="occBranchId">
            Filial (ID)
          </label>
          <input
            id="occBranchId"
            type="number"
            min={1}
            value={branchId}
            onChange={(e) => setBranchId(Number(e.target.value))}
            className={`${inputClass} w-28`}
          />
        </div>
        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Consultando..." : "Consultar"}
        </button>
      </form>

      {error && <div className="mt-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}

      {occupancy && (
        <div className="mt-4 grid grid-cols-2 gap-3 sm:grid-cols-4">
          <Stat label="Total de Vagas" value={occupancy.totalSpaces} />
          <Stat label="Ocupadas" value={occupancy.occupiedSpaces} accent="text-red-600" />
          <Stat label="Disponíveis" value={occupancy.availableSpaces} accent="text-emerald-600" />
          <Stat label="Taxa de Ocupação" value={`${occupancy.occupancyRate.toFixed(1)}%`} />
        </div>
      )}

      <div className="mt-5 border-t border-slate-100 pt-4">
        <h3 className="mb-3 text-xs font-semibold uppercase text-slate-500">Cadastrar Vaga</h3>
        <form className="flex flex-wrap items-end gap-4" onSubmit={handleCreateSpace}>
          <div>
            <label className={labelClass} htmlFor="newSpaceNumber">
              Número/ID da Vaga
            </label>
            <input
              id="newSpaceNumber"
              value={spaceNumber}
              onChange={(e) => setSpaceNumber(e.target.value)}
              className={`${inputClass} w-32`}
              placeholder="A-01"
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="newSpaceType">
              Tipo
            </label>
            <select
              id="newSpaceType"
              value={spaceType}
              onChange={(e) => setSpaceType(Number(e.target.value))}
              className={`${inputClass} w-40`}
            >
              {SPACE_TYPES.map((t) => (
                <option key={t.value} value={t.value}>
                  {t.label}
                </option>
              ))}
            </select>
          </div>
          <button type="submit" disabled={createLoading} className={buttonClass}>
            {createLoading ? "Cadastrando..." : "Cadastrar Vaga"}
          </button>
        </form>
        {createError && (
          <div className="mt-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{createError}</div>
        )}
        {createSuccess && (
          <div className="mt-3 rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            {createSuccess}
          </div>
        )}
      </div>
    </section>
  );
}

function Stat({
  label,
  value,
  accent,
}: {
  label: string;
  value: string | number;
  accent?: string;
}) {
  return (
    <div className="rounded-md bg-slate-50 p-3">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-500">{label}</p>
      <p className={`mt-1 text-lg font-bold ${accent ?? "text-slate-800"}`}>{value}</p>
    </div>
  );
}

function QuickEntrySection({
  spacesVersion,
  onEntryRegistered,
}: {
  spacesVersion: number;
  onEntryRegistered: () => void;
}) {
  const [branchId, setBranchId] = useState(1);
  const [parkingSpaceId, setParkingSpaceId] = useState<number | "">("");
  const [licensePlate, setLicensePlate] = useState("");
  const [vehicleModel, setVehicleModel] = useState("");
  const [vehicleColor, setVehicleColor] = useState("");

  const [parkingSpaces, setParkingSpaces] = useState<ParkingSpaceDto[]>([]);
  const [optionsLoading, setOptionsLoading] = useState(false);
  const [optionsError, setOptionsError] = useState<string | null>(null);

  const [result, setResult] = useState<VehicleEntryByPlateResultDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setOptionsLoading(true);
    setOptionsError(null);
    getAllParkingSpacesByBranch(branchId)
      .then((spaces) => {
        if (cancelled) return;
        setParkingSpaces(spaces);
        setParkingSpaceId((prev) => {
          if (prev !== "" && spaces.some((s) => s.id === prev)) return prev;
          const firstAvailable = spaces.find((s) => s.status === 0);
          return firstAvailable ? firstAvailable.id : "";
        });
      })
      .catch((err) => {
        if (cancelled) return;
        setOptionsError(extractErrorMessage(err, "Não foi possível carregar as vagas."));
        setParkingSpaces([]);
      })
      .finally(() => {
        if (!cancelled) setOptionsLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId, spacesVersion]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (parkingSpaceId === "" || !licensePlate.trim()) {
      setError("Informe a placa e selecione uma vaga.");
      return;
    }
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const entry = await registerVehicleEntryByPlate({
        branchId,
        parkingSpaceId,
        licensePlate: licensePlate.trim(),
        vehicleModel: vehicleModel.trim() || undefined,
        vehicleColor: vehicleColor.trim() || undefined,
      });
      setResult(entry);
      setLicensePlate("");
      setVehicleModel("");
      setVehicleColor("");
      onEntryRegistered();
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível registrar a entrada."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-1 text-sm font-semibold text-slate-700">Entrada Rápida (por Placa)</h2>
      <p className="mb-3 text-xs text-slate-500">
        Só a placa e a vaga. O sistema reconhece o cliente automaticamente (mensalista, convênio
        ou avulso) — não é preciso cadastrar o cliente antes.
      </p>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="quickBranchId">
              Filial (ID)
            </label>
            <input
              id="quickBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="quickSpaceId">
              Vaga
            </label>
            <select
              id="quickSpaceId"
              value={parkingSpaceId}
              onChange={(e) => setParkingSpaceId(e.target.value ? Number(e.target.value) : "")}
              className={inputClass}
              disabled={optionsLoading}
            >
              <option value="">Selecione...</option>
              {parkingSpaces.map((s) => (
                <option key={s.id} value={s.id} disabled={s.status !== 0}>
                  {s.spaceNumber} {s.status === 0 ? "(disponível)" : "(ocupada/manutenção)"}
                </option>
              ))}
            </select>
            {optionsError && <p className="mt-1 text-xs text-red-600">{optionsError}</p>}
          </div>
        </div>
        <div>
          <label className={labelClass} htmlFor="quickPlate">
            Placa
          </label>
          <input
            id="quickPlate"
            required
            autoFocus
            value={licensePlate}
            onChange={(e) => setLicensePlate(e.target.value.toUpperCase())}
            className={inputClass}
            placeholder="ABC1D23"
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="quickModel">
              Modelo (opcional)
            </label>
            <ModelAutocomplete
              id="quickModel"
              value={vehicleModel}
              onChange={setVehicleModel}
              className={inputClass}
              placeholder="Comece a digitar..."
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="quickColor">
              Cor (opcional)
            </label>
            <input
              id="quickColor"
              value={vehicleColor}
              onChange={(e) => setVehicleColor(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>

        {error && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            <p>
              Entrada #{result.id} registrada para a placa <strong>{result.licensePlate}</strong>.
            </p>
            <p>
              Cliente: <strong>{result.customerName}</strong> (
              {CUSTOMER_TYPE_LABELS[result.customerType] ?? result.customerType}){" "}
              {result.isNewCustomer ? (
                <span className="text-amber-700">— cadastrado automaticamente agora</span>
              ) : (
                <span className="text-slate-500">— cliente já reconhecido</span>
              )}
            </p>
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Registrando..." : "Registrar Entrada"}
        </button>
      </form>
    </section>
  );
}

function AdvancedEntrySection({
  spacesVersion,
  onEntryRegistered,
}: {
  spacesVersion: number;
  onEntryRegistered: () => void;
}) {
  const [expanded, setExpanded] = useState(false);

  return (
    <section className={sectionClass}>
      <button
        type="button"
        onClick={() => setExpanded((v) => !v)}
        className="flex w-full items-center justify-between text-left"
      >
        <span className="text-sm font-semibold text-slate-700">
          Entrada Avançada (selecionar cliente manualmente)
        </span>
        <span className="text-xs text-slate-400">{expanded ? "Ocultar ▲" : "Mostrar ▼"}</span>
      </button>
      <p className="mt-1 text-xs text-slate-500">
        Use apenas em casos especiais. Para o dia a dia, prefira a Entrada Rápida por Placa acima.
      </p>
      {expanded && (
        <div className="mt-4 border-t border-slate-100 pt-4">
          <EntrySection spacesVersion={spacesVersion} onEntryRegistered={onEntryRegistered} />
        </div>
      )}
    </section>
  );
}

function EntrySection({
  spacesVersion,
  onEntryRegistered,
}: {
  spacesVersion: number;
  onEntryRegistered: () => void;
}) {
  const [branchId, setBranchId] = useState(1);
  const [parkingSpaceId, setParkingSpaceId] = useState<number | "">("");
  const [customerId, setCustomerId] = useState<number | "">("");
  const [licensePlate, setLicensePlate] = useState("");
  const [vehicleModel, setVehicleModel] = useState("");
  const [vehicleColor, setVehicleColor] = useState("");

  const [parkingSpaces, setParkingSpaces] = useState<ParkingSpaceDto[]>([]);
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [optionsLoading, setOptionsLoading] = useState(false);
  const [optionsError, setOptionsError] = useState<string | null>(null);

  const [result, setResult] = useState<VehicleEntryDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setOptionsLoading(true);
    setOptionsError(null);
    Promise.all([getAllParkingSpacesByBranch(branchId), getAllCustomersByBranch(branchId)])
      .then(([spaces, custs]) => {
        if (cancelled) return;
        setParkingSpaces(spaces);
        setCustomers(custs);
        setParkingSpaceId((prev) => {
          if (prev !== "") return prev;
          const firstAvailable = spaces.find((s) => s.status === 0);
          return firstAvailable ? firstAvailable.id : "";
        });
        setCustomerId((prev) => (prev === "" && custs.length > 0 ? custs[0].id : prev));
      })
      .catch((err) => {
        if (cancelled) return;
        setOptionsError(extractErrorMessage(err, "Não foi possível carregar vagas/clientes."));
        setParkingSpaces([]);
        setCustomers([]);
      })
      .finally(() => {
        if (!cancelled) setOptionsLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId, spacesVersion]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (parkingSpaceId === "" || customerId === "") {
      setError("Selecione uma vaga e um cliente.");
      return;
    }
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const entry = await registerVehicleEntry({
        branchId,
        parkingSpaceId,
        customerId,
        licensePlate,
        vehicleModel,
        vehicleColor,
      });
      setResult(entry);
      onEntryRegistered();
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível registrar a entrada."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <div>
      <h3 className="mb-3 text-xs font-semibold uppercase text-slate-500">
        Registrar Entrada (manual)
      </h3>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="entryBranchId">
              Filial (ID)
            </label>
            <input
              id="entryBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="entrySpaceId">
              Vaga
            </label>
            <select
              id="entrySpaceId"
              value={parkingSpaceId}
              onChange={(e) => setParkingSpaceId(e.target.value ? Number(e.target.value) : "")}
              className={inputClass}
              disabled={optionsLoading}
            >
              <option value="">Selecione...</option>
              {parkingSpaces.map((s) => (
                <option key={s.id} value={s.id} disabled={s.status !== 0}>
                  {s.spaceNumber} {s.status === 0 ? "(disponível)" : "(ocupada/manutenção)"}
                </option>
              ))}
            </select>
            {parkingSpaces.length === 0 && !optionsLoading && (
              <p className="mt-1 text-xs text-slate-400">
                Nenhuma vaga cadastrada para esta filial. Cadastre em "Vagas / Ocupação" acima.
              </p>
            )}
          </div>
        </div>
        <div>
          <label className={labelClass} htmlFor="entryCustomerId">
            Cliente
          </label>
          <select
            id="entryCustomerId"
            value={customerId}
            onChange={(e) => setCustomerId(e.target.value ? Number(e.target.value) : "")}
            className={inputClass}
            disabled={optionsLoading}
          >
            <option value="">Selecione...</option>
            {customers.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name} - {c.document}
              </option>
            ))}
          </select>
          {optionsError && (
            <p className="mt-1 text-xs text-red-600">{optionsError}</p>
          )}
        </div>
        <div>
          <label className={labelClass} htmlFor="entryPlate">
            Placa
          </label>
          <input
            id="entryPlate"
            required
            value={licensePlate}
            onChange={(e) => setLicensePlate(e.target.value)}
            className={inputClass}
            placeholder="ABC1D23"
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="entryModel">
              Modelo
            </label>
            <ModelAutocomplete
              id="entryModel"
              value={vehicleModel}
              onChange={setVehicleModel}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="entryColor">
              Cor
            </label>
            <input
              id="entryColor"
              required
              value={vehicleColor}
              onChange={(e) => setVehicleColor(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>

        {error && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Entrada registrada com sucesso. ID da entrada: <strong>{result.id}</strong>
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Registrando..." : "Registrar Entrada"}
        </button>
      </form>
    </div>
  );
}

function ExitSection() {
  const [branchId, setBranchId] = useState(1);
  const [vehicleEntryId, setVehicleEntryId] = useState<number | "">("");

  const [openEntries, setOpenEntries] = useState<VehicleEntryDto[]>([]);
  const [optionsLoading, setOptionsLoading] = useState(false);
  const [optionsError, setOptionsError] = useState<string | null>(null);

  const [result, setResult] = useState<VehicleExitDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setOptionsLoading(true);
    setOptionsError(null);
    getOpenVehicleEntriesByBranch(branchId)
      .then((entries) => {
        if (cancelled) return;
        setOpenEntries(entries);
        setVehicleEntryId((prev) => (prev === "" && entries.length > 0 ? entries[0].id : prev));
      })
      .catch((err) => {
        if (cancelled) return;
        setOptionsError(extractErrorMessage(err, "Não foi possível carregar entradas em aberto."));
        setOpenEntries([]);
      })
      .finally(() => {
        if (!cancelled) setOptionsLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (vehicleEntryId === "") {
      setError("Selecione uma entrada em aberto.");
      return;
    }
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const exit = await registerVehicleExit({ vehicleEntryId });
      setResult(exit);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível registrar a saída."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Registrar Saída</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="exitBranchId">
            Filial (ID)
          </label>
          <input
            id="exitBranchId"
            type="number"
            min={1}
            value={branchId}
            onChange={(e) => setBranchId(Number(e.target.value))}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="exitEntryId">
            Entrada em Aberto
          </label>
          <select
            id="exitEntryId"
            value={vehicleEntryId}
            onChange={(e) => setVehicleEntryId(e.target.value ? Number(e.target.value) : "")}
            className={inputClass}
            disabled={optionsLoading}
          >
            <option value="">Selecione...</option>
            {openEntries.map((entry) => (
              <option key={entry.id} value={entry.id}>
                {entry.licensePlate} - entrada às {new Date(entry.entryTime).toLocaleString("pt-BR")}
              </option>
            ))}
          </select>
          {optionsError && <p className="mt-1 text-xs text-red-600">{optionsError}</p>}
        </div>

        {error && <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            <p>
              Saída registrada. Duração: <strong>{result.durationMinutes} min</strong>
            </p>
            <p>
              Valor: <strong>{result.totalAmount.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}</strong>
            </p>
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Registrando..." : "Registrar Saída"}
        </button>
      </form>
    </section>
  );
}

const MOVEMENT_TYPES = [
  { value: 1, label: "1 - Entrada" },
  { value: 2, label: "2 - Saída" },
  { value: 3, label: "3 - Sangria" },
];

function CashRegisterSection() {
  const [branchId, setBranchId] = useState(1);
  const [employeeId, setEmployeeId] = useState(1);
  const [openingBalance, setOpeningBalance] = useState(0);
  const [cashRegister, setCashRegister] = useState<CashRegisterDto | null>(null);
  const [openLoading, setOpenLoading] = useState(false);
  const [openError, setOpenError] = useState<string | null>(null);

  const [movementType, setMovementType] = useState(1);
  const [movementAmount, setMovementAmount] = useState(0);
  const [movementDescription, setMovementDescription] = useState("");
  const [movementResult, setMovementResult] = useState<CashMovementDto | null>(null);
  const [movementLoading, setMovementLoading] = useState(false);
  const [movementError, setMovementError] = useState<string | null>(null);

  const [closingBalance, setClosingBalance] = useState(0);
  const [closeResult, setCloseResult] = useState<CashRegisterDto | null>(null);
  const [closeLoading, setCloseLoading] = useState(false);
  const [closeError, setCloseError] = useState<string | null>(null);

  const [existingOpenRegister, setExistingOpenRegister] = useState<CashRegisterDto | null>(null);
  const [checkLoading, setCheckLoading] = useState(false);
  const [checkError, setCheckError] = useState<string | null>(null);

  async function handleCheckOpen() {
    setCheckLoading(true);
    setCheckError(null);
    try {
      const register = await getOpenCashRegisterByBranch(branchId);
      setExistingOpenRegister(register);
    } catch (err) {
      setCheckError(extractErrorMessage(err, "Não foi possível verificar o caixa aberto."));
      setExistingOpenRegister(null);
    } finally {
      setCheckLoading(false);
    }
  }

  useEffect(() => {
    handleCheckOpen();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [branchId]);

  async function handleOpen(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setOpenLoading(true);
    setOpenError(null);
    try {
      const register = await openCashRegister({ branchId, employeeId, openingBalance });
      setCashRegister(register);
      setExistingOpenRegister(register);
      setCloseResult(null);
    } catch (err) {
      setOpenError(extractErrorMessage(err, "Não foi possível abrir o caixa."));
    } finally {
      setOpenLoading(false);
    }
  }

  async function handleMovement(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!cashRegister) return;
    setMovementLoading(true);
    setMovementError(null);
    try {
      const movement = await recordCashMovement(cashRegister.id, {
        type: movementType,
        amount: movementAmount,
        description: movementDescription,
      });
      setMovementResult(movement);
    } catch (err) {
      setMovementError(extractErrorMessage(err, "Não foi possível registrar a movimentação."));
    } finally {
      setMovementLoading(false);
    }
  }

  async function handleClose(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!cashRegister) return;
    setCloseLoading(true);
    setCloseError(null);
    try {
      const closed = await closeCashRegister(cashRegister.id, closingBalance);
      setCloseResult(closed);
      setExistingOpenRegister(null);
    } catch (err) {
      setCloseError(extractErrorMessage(err, "Não foi possível fechar o caixa."));
    } finally {
      setCloseLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <div className="mb-3 flex items-center justify-between">
        <h2 className="text-sm font-semibold text-slate-700">Caixa</h2>
        <button
          type="button"
          onClick={handleCheckOpen}
          disabled={checkLoading}
          className="rounded-md border border-slate-300 px-3 py-1 text-xs font-medium text-slate-600 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {checkLoading ? "Verificando..." : "Verificar caixa aberto"}
        </button>
      </div>

      {checkError && (
        <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{checkError}</div>
      )}

      {existingOpenRegister ? (
        <div className="mb-4 rounded-md bg-amber-50 px-3 py-2 text-sm text-amber-800">
          Já existe um caixa aberto para esta filial: <strong>#{existingOpenRegister.id}</strong>{" "}
          (aberto em {new Date(existingOpenRegister.openedAt).toLocaleString("pt-BR")}). Não é
          necessário abrir outro.
        </div>
      ) : (
        !checkLoading && (
          <div className="mb-4 rounded-md bg-slate-50 px-3 py-2 text-sm text-slate-500">
            Nenhum caixa aberto para esta filial no momento.
          </div>
        )
      )}

      {cashRegister && (
        <div className="mb-4 rounded-md bg-slate-50 px-3 py-2 text-sm text-slate-700">
          Caixa aberto nesta sessão: <strong>#{cashRegister.id}</strong>{" "}
          {closeResult && <span className="text-emerald-700">(fechado)</span>}
        </div>
      )}

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <form className="flex flex-col gap-3" onSubmit={handleOpen}>
          <h3 className="text-xs font-semibold uppercase text-slate-500">Abrir Caixa</h3>
          <div>
            <label className={labelClass} htmlFor="cashBranchId">
              Filial (ID)
            </label>
            <input
              id="cashBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="cashEmployeeId">
              Funcionário (ID)
            </label>
            <input
              id="cashEmployeeId"
              type="number"
              min={1}
              value={employeeId}
              onChange={(e) => setEmployeeId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="cashOpeningBalance">
              Saldo Inicial (R$)
            </label>
            <input
              id="cashOpeningBalance"
              type="number"
              min={0}
              step="0.01"
              value={openingBalance}
              onChange={(e) => setOpeningBalance(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          {openError && (
            <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{openError}</div>
          )}
          <button type="submit" disabled={openLoading} className={buttonClass}>
            {openLoading ? "Abrindo..." : "Abrir Caixa"}
          </button>
        </form>

        <form className="flex flex-col gap-3" onSubmit={handleMovement}>
          <h3 className="text-xs font-semibold uppercase text-slate-500">Movimentação</h3>
          <div>
            <label className={labelClass} htmlFor="movementType">
              Tipo
            </label>
            <select
              id="movementType"
              value={movementType}
              onChange={(e) => setMovementType(Number(e.target.value))}
              className={inputClass}
            >
              {MOVEMENT_TYPES.map((m) => (
                <option key={m.value} value={m.value}>
                  {m.label}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className={labelClass} htmlFor="movementAmount">
              Valor (R$)
            </label>
            <input
              id="movementAmount"
              type="number"
              min={0}
              step="0.01"
              value={movementAmount}
              onChange={(e) => setMovementAmount(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="movementDescription">
              Descrição
            </label>
            <input
              id="movementDescription"
              value={movementDescription}
              onChange={(e) => setMovementDescription(e.target.value)}
              className={inputClass}
            />
          </div>
          {movementError && (
            <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{movementError}</div>
          )}
          {movementResult && (
            <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
              Movimentação #{movementResult.id} registrada.
            </div>
          )}
          <button
            type="submit"
            disabled={movementLoading || !cashRegister}
            className={buttonClass}
          >
            {movementLoading ? "Registrando..." : "Registrar Movimentação"}
          </button>
          {!cashRegister && (
            <p className="text-xs text-slate-400">Abra um caixa nesta sessão primeiro.</p>
          )}
        </form>

        <form className="flex flex-col gap-3" onSubmit={handleClose}>
          <h3 className="text-xs font-semibold uppercase text-slate-500">Fechar Caixa</h3>
          <div>
            <label className={labelClass} htmlFor="closingBalance">
              Saldo de Fechamento (R$)
            </label>
            <input
              id="closingBalance"
              type="number"
              min={0}
              step="0.01"
              value={closingBalance}
              onChange={(e) => setClosingBalance(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          {closeError && (
            <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{closeError}</div>
          )}
          {closeResult && (
            <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
              Caixa #{closeResult.id} fechado com saldo{" "}
              {closeResult.closingBalance.toLocaleString("pt-BR", {
                style: "currency",
                currency: "BRL",
              })}
              .
            </div>
          )}
          <button
            type="submit"
            disabled={closeLoading || !cashRegister}
            className={buttonClass}
          >
            {closeLoading ? "Fechando..." : "Fechar Caixa"}
          </button>
          {!cashRegister && (
            <p className="text-xs text-slate-400">Abra um caixa nesta sessão primeiro.</p>
          )}
        </form>
      </div>
    </section>
  );
}
