import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import axios from "axios";
import {
  createSupplier,
  createProduct,
  getProductStock,
  getAllSuppliersByBranch,
  createPurchase,
  receivePurchaseItems,
  adjustStock,
  getBelowMinimum,
} from "../api/inventory";
import type {
  SupplierDto,
  ProductDto,
  PurchaseDto,
  PurchaseItemInput,
  ReceivePurchaseItemInput,
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

function formatCurrency(value: number): string {
  return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

export default function InventoryPage() {
  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-bold text-slate-900">Estoque</h1>
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <CreateSupplierForm />
        <CreateProductForm />
      </div>
      <StockTable />
      <BelowMinimumTable />
      <CreatePurchaseForm />
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <ReceivePurchaseForm />
        <AdjustStockForm />
      </div>
    </div>
  );
}

function CreateSupplierForm() {
  const [branchId, setBranchId] = useState(1);
  const [name, setName] = useState("");
  const [document, setDocument] = useState("");
  const [phone, setPhone] = useState("");
  const [email, setEmail] = useState("");

  const [result, setResult] = useState<SupplierDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const supplier = await createSupplier({
        branchId,
        name,
        document,
        phone: phone || undefined,
        email: email || undefined,
      });
      setResult(supplier);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o fornecedor."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Fornecedor</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="supBranchId">
            Filial (ID)
          </label>
          <input
            id="supBranchId"
            type="number"
            min={1}
            value={branchId}
            onChange={(e) => setBranchId(Number(e.target.value))}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="supName">
            Nome
          </label>
          <input
            id="supName"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="supDocument">
            Documento
          </label>
          <input
            id="supDocument"
            required
            value={document}
            onChange={(e) => setDocument(e.target.value)}
            className={inputClass}
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="supPhone">
              Telefone
            </label>
            <input
              id="supPhone"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="supEmail">
              E-mail
            </label>
            <input
              id="supEmail"
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
            Fornecedor criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Fornecedor"}
        </button>
      </form>
    </section>
  );
}

function CreateProductForm() {
  const [branchId, setBranchId] = useState(1);
  const [name, setName] = useState("");
  const [sku, setSku] = useState("");
  const [category, setCategory] = useState("");
  const [cost, setCost] = useState(0);
  const [sellingPrice, setSellingPrice] = useState(0);
  const [stock, setStock] = useState(0);
  const [minimumStock, setMinimumStock] = useState(0);
  const [supplierId, setSupplierId] = useState<string>("");

  const [suppliers, setSuppliers] = useState<SupplierDto[]>([]);
  const [suppliersLoading, setSuppliersLoading] = useState(false);
  const [suppliersError, setSuppliersError] = useState<string | null>(null);

  const [result, setResult] = useState<ProductDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setSuppliersLoading(true);
    setSuppliersError(null);
    getAllSuppliersByBranch(branchId)
      .then((data) => {
        if (cancelled) return;
        setSuppliers(data);
      })
      .catch((err) => {
        if (cancelled) return;
        setSuppliersError(extractErrorMessage(err, "Não foi possível carregar fornecedores."));
        setSuppliers([]);
      })
      .finally(() => {
        if (!cancelled) setSuppliersLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const product = await createProduct({
        branchId,
        name,
        sku,
        category,
        cost,
        sellingPrice,
        stock,
        minimumStock,
        supplierId: supplierId ? Number(supplierId) : null,
      });
      setResult(product);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível criar o produto."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Criar Produto</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="prodBranchId">
              Filial (ID)
            </label>
            <input
              id="prodBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="prodSupplierId">
              Fornecedor (opcional)
            </label>
            <select
              id="prodSupplierId"
              value={supplierId}
              onChange={(e) => setSupplierId(e.target.value)}
              className={inputClass}
              disabled={suppliersLoading}
            >
              <option value="">Nenhum</option>
              {suppliers.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
            {suppliersError && <p className="mt-1 text-xs text-red-600">{suppliersError}</p>}
          </div>
        </div>
        <div>
          <label className={labelClass} htmlFor="prodName">
            Nome
          </label>
          <input
            id="prodName"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={inputClass}
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="prodSku">
              SKU
            </label>
            <input
              id="prodSku"
              required
              value={sku}
              onChange={(e) => setSku(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="prodCategory">
              Categoria
            </label>
            <input
              id="prodCategory"
              required
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className={inputClass}
            />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="prodCost">
              Custo (R$)
            </label>
            <input
              id="prodCost"
              type="number"
              min={0}
              step="0.01"
              value={cost}
              onChange={(e) => setCost(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="prodSellingPrice">
              Preço de Venda (R$)
            </label>
            <input
              id="prodSellingPrice"
              type="number"
              min={0}
              step="0.01"
              value={sellingPrice}
              onChange={(e) => setSellingPrice(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="prodStock">
              Estoque Inicial
            </label>
            <input
              id="prodStock"
              type="number"
              min={0}
              step="0.01"
              value={stock}
              onChange={(e) => setStock(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="prodMinStock">
              Estoque Mínimo
            </label>
            <input
              id="prodMinStock"
              type="number"
              min={0}
              step="0.01"
              value={minimumStock}
              onChange={(e) => setMinimumStock(Number(e.target.value))}
              className={inputClass}
            />
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Produto criado com ID <strong>{result.id}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Criando..." : "Criar Produto"}
        </button>
      </form>
    </section>
  );
}

function StockTable() {
  const [branchId, setBranchId] = useState(1);
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const data = await getProductStock(branchId);
      setProducts(data);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível carregar o estoque."));
      setProducts([]);
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Estoque Atual</h2>
      <form className="mb-4 flex flex-wrap items-end gap-4" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="stockBranchId">
            Filial (ID)
          </label>
          <input
            id="stockBranchId"
            type="number"
            min={1}
            value={branchId}
            onChange={(e) => setBranchId(Number(e.target.value))}
            className={`${inputClass} w-28`}
          />
        </div>
        <button type="submit" disabled={loading} className="rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:opacity-60">
          {loading ? "Carregando..." : "Consultar"}
        </button>
      </form>

      {error && (
        <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
      )}

      {products.length > 0 ? (
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-slate-200 text-sm">
            <thead>
              <tr className="text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
                <th className="py-2 pr-4">ID</th>
                <th className="py-2 pr-4">Nome</th>
                <th className="py-2 pr-4">SKU</th>
                <th className="py-2 pr-4">Categoria</th>
                <th className="py-2 pr-4 text-right">Estoque</th>
                <th className="py-2 pr-4 text-right">Mínimo</th>
                <th className="py-2 pr-4 text-right">Preço de Venda</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {products.map((p) => (
                <tr key={p.id}>
                  <td className="py-2 pr-4">{p.id}</td>
                  <td className="py-2 pr-4 font-medium text-slate-800">{p.name}</td>
                  <td className="py-2 pr-4 text-slate-500">{p.sku}</td>
                  <td className="py-2 pr-4 text-slate-500">{p.category}</td>
                  <td className={`py-2 pr-4 text-right ${p.stock < p.minimumStock ? "text-red-600" : ""}`}>
                    {p.stock}
                  </td>
                  <td className="py-2 pr-4 text-right text-slate-500">{p.minimumStock}</td>
                  <td className="py-2 pr-4 text-right">{formatCurrency(p.sellingPrice)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <p className="text-sm text-slate-500">Nenhum produto carregado.</p>
      )}
    </section>
  );
}

function BelowMinimumTable() {
  const [branchId, setBranchId] = useState(1);
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const data = await getBelowMinimum(branchId);
      setProducts(data);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível carregar os produtos abaixo do mínimo."));
      setProducts([]);
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Produtos Abaixo do Mínimo</h2>
      <form className="mb-4 flex flex-wrap items-end gap-4" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="belowMinBranchId">
            Filial (ID)
          </label>
          <input
            id="belowMinBranchId"
            type="number"
            min={1}
            value={branchId}
            onChange={(e) => setBranchId(Number(e.target.value))}
            className={`${inputClass} w-28`}
          />
        </div>
        <button type="submit" disabled={loading} className="rounded-md bg-slate-900 px-4 py-1.5 text-sm font-semibold text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:opacity-60">
          {loading ? "Carregando..." : "Consultar"}
        </button>
      </form>

      {error && (
        <div className="mb-3 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
      )}

      {products.length > 0 ? (
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-slate-200 text-sm">
            <thead>
              <tr className="text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
                <th className="py-2 pr-4">Produto</th>
                <th className="py-2 pr-4">SKU</th>
                <th className="py-2 pr-4 text-right">Estoque Atual</th>
                <th className="py-2 pr-4 text-right">Estoque Mínimo</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {products.map((p) => (
                <tr key={p.id}>
                  <td className="py-2 pr-4 font-medium text-slate-800">{p.name}</td>
                  <td className="py-2 pr-4 text-slate-500">{p.sku}</td>
                  <td className="py-2 pr-4 text-right text-red-600">{p.stock}</td>
                  <td className="py-2 pr-4 text-right text-slate-500">{p.minimumStock}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <p className="text-sm text-slate-500">Nenhum produto abaixo do mínimo (ou nada consultado ainda).</p>
      )}
    </section>
  );
}

function emptyPurchaseItem(): PurchaseItemInput {
  return { productId: 1, quantityOrdered: 1, unitCost: 0 };
}

function CreatePurchaseForm() {
  const [branchId, setBranchId] = useState(1);
  const [supplierId, setSupplierId] = useState<number | "">("");
  const [items, setItems] = useState<PurchaseItemInput[]>([emptyPurchaseItem()]);

  const [suppliers, setSuppliers] = useState<SupplierDto[]>([]);
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [optionsLoading, setOptionsLoading] = useState(false);
  const [optionsError, setOptionsError] = useState<string | null>(null);

  const [result, setResult] = useState<PurchaseDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setOptionsLoading(true);
    setOptionsError(null);
    Promise.all([getAllSuppliersByBranch(branchId), getProductStock(branchId)])
      .then(([sups, prods]) => {
        if (cancelled) return;
        setSuppliers(sups);
        setProducts(prods);
        setSupplierId((prev) => (prev === "" && sups.length > 0 ? sups[0].id : prev));
      })
      .catch((err) => {
        if (cancelled) return;
        setOptionsError(extractErrorMessage(err, "Não foi possível carregar fornecedores/produtos."));
        setSuppliers([]);
        setProducts([]);
      })
      .finally(() => {
        if (!cancelled) setOptionsLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId]);

  function updateItem(index: number, patch: Partial<PurchaseItemInput>) {
    setItems((prev) => prev.map((it, i) => (i === index ? { ...it, ...patch } : it)));
  }

  function addItem() {
    setItems((prev) => [...prev, emptyPurchaseItem()]);
  }

  function removeItem(index: number) {
    setItems((prev) => prev.filter((_, i) => i !== index));
  }

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (supplierId === "") {
      setError("Selecione um fornecedor.");
      return;
    }
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const purchase = await createPurchase({ branchId, supplierId, items });
      setResult(purchase);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível registrar a compra."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Registrar Compra</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="purchBranchId">
              Filial (ID)
            </label>
            <input
              id="purchBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="purchSupplierId">
              Fornecedor
            </label>
            <select
              id="purchSupplierId"
              value={supplierId}
              onChange={(e) => setSupplierId(e.target.value ? Number(e.target.value) : "")}
              className={inputClass}
              disabled={optionsLoading}
            >
              <option value="">Selecione...</option>
              {suppliers.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
            {optionsError && <p className="mt-1 text-xs text-red-600">{optionsError}</p>}
          </div>
        </div>

        <div>
          <h3 className="mb-2 text-xs font-semibold uppercase text-slate-500">Itens</h3>
          <div className="flex flex-col gap-2">
            {items.map((item, index) => (
              <div key={index} className="grid grid-cols-1 gap-2 sm:grid-cols-4 sm:items-end">
                <div>
                  <label className={labelClass} htmlFor={`purchItemProduct-${index}`}>
                    Produto
                  </label>
                  <select
                    id={`purchItemProduct-${index}`}
                    value={item.productId}
                    onChange={(e) => updateItem(index, { productId: Number(e.target.value) })}
                    className={inputClass}
                    disabled={optionsLoading}
                  >
                    <option value="">Selecione...</option>
                    {products.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.name} ({p.sku})
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className={labelClass} htmlFor={`purchItemQty-${index}`}>
                    Quantidade
                  </label>
                  <input
                    id={`purchItemQty-${index}`}
                    type="number"
                    min={0}
                    step="0.01"
                    value={item.quantityOrdered}
                    onChange={(e) =>
                      updateItem(index, { quantityOrdered: Number(e.target.value) })
                    }
                    className={inputClass}
                  />
                </div>
                <div>
                  <label className={labelClass} htmlFor={`purchItemCost-${index}`}>
                    Custo Unitário (R$)
                  </label>
                  <input
                    id={`purchItemCost-${index}`}
                    type="number"
                    min={0}
                    step="0.01"
                    value={item.unitCost}
                    onChange={(e) => updateItem(index, { unitCost: Number(e.target.value) })}
                    className={inputClass}
                  />
                </div>
                <button
                  type="button"
                  onClick={() => removeItem(index)}
                  disabled={items.length === 1}
                  className="rounded-md border border-slate-300 px-3 py-1.5 text-sm text-slate-600 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
                >
                  Remover
                </button>
              </div>
            ))}
          </div>
          <button
            type="button"
            onClick={addItem}
            className="mt-2 rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50"
          >
            + Adicionar item
          </button>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Compra #{result.purchaseNumber} registrada (ID {result.id}).
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Registrando..." : "Registrar Compra"}
        </button>
      </form>
    </section>
  );
}

function emptyReceiveItem(): ReceivePurchaseItemInput {
  return { purchaseItemId: 1, quantityReceived: 1 };
}

function ReceivePurchaseForm() {
  const [purchaseId, setPurchaseId] = useState(1);
  const [items, setItems] = useState<ReceivePurchaseItemInput[]>([emptyReceiveItem()]);

  const [result, setResult] = useState<PurchaseDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function updateItem(index: number, patch: Partial<ReceivePurchaseItemInput>) {
    setItems((prev) => prev.map((it, i) => (i === index ? { ...it, ...patch } : it)));
  }

  function addItem() {
    setItems((prev) => [...prev, emptyReceiveItem()]);
  }

  function removeItem(index: number) {
    setItems((prev) => prev.filter((_, i) => i !== index));
  }

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const purchase = await receivePurchaseItems(purchaseId, items);
      setResult(purchase);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível receber os itens da compra."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Receber Itens de Compra</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div>
          <label className={labelClass} htmlFor="recvPurchaseId">
            Compra (ID)
          </label>
          <input
            id="recvPurchaseId"
            type="number"
            min={1}
            value={purchaseId}
            onChange={(e) => setPurchaseId(Number(e.target.value))}
            className={inputClass}
          />
        </div>

        <div>
          <h3 className="mb-2 text-xs font-semibold uppercase text-slate-500">Itens Recebidos</h3>
          <div className="flex flex-col gap-2">
            {items.map((item, index) => (
              <div key={index} className="grid grid-cols-1 gap-2 sm:grid-cols-3 sm:items-end">
                <div>
                  <label className={labelClass} htmlFor={`recvItemId-${index}`}>
                    Item da Compra (ID)
                  </label>
                  <input
                    id={`recvItemId-${index}`}
                    type="number"
                    min={1}
                    value={item.purchaseItemId}
                    onChange={(e) =>
                      updateItem(index, { purchaseItemId: Number(e.target.value) })
                    }
                    className={inputClass}
                  />
                </div>
                <div>
                  <label className={labelClass} htmlFor={`recvItemQty-${index}`}>
                    Quantidade Recebida
                  </label>
                  <input
                    id={`recvItemQty-${index}`}
                    type="number"
                    min={0}
                    step="0.01"
                    value={item.quantityReceived}
                    onChange={(e) =>
                      updateItem(index, { quantityReceived: Number(e.target.value) })
                    }
                    className={inputClass}
                  />
                </div>
                <button
                  type="button"
                  onClick={() => removeItem(index)}
                  disabled={items.length === 1}
                  className="rounded-md border border-slate-300 px-3 py-1.5 text-sm text-slate-600 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
                >
                  Remover
                </button>
              </div>
            ))}
          </div>
          <button
            type="button"
            onClick={addItem}
            className="mt-2 rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50"
          >
            + Adicionar item
          </button>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Recebimento registrado para a compra #{result.purchaseNumber}.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Registrando..." : "Registrar Recebimento"}
        </button>
      </form>
    </section>
  );
}

function AdjustStockForm() {
  const [branchId, setBranchId] = useState(1);
  const [productId, setProductId] = useState<number | "">("");
  const [adjustment, setAdjustment] = useState(0);
  const [reason, setReason] = useState("");

  const [products, setProducts] = useState<ProductDto[]>([]);
  const [productsLoading, setProductsLoading] = useState(false);
  const [productsError, setProductsError] = useState<string | null>(null);

  const [result, setResult] = useState<ProductDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setProductsLoading(true);
    setProductsError(null);
    getProductStock(branchId)
      .then((data) => {
        if (cancelled) return;
        setProducts(data);
        setProductId((prev) => (prev === "" && data.length > 0 ? data[0].id : prev));
      })
      .catch((err) => {
        if (cancelled) return;
        setProductsError(extractErrorMessage(err, "Não foi possível carregar os produtos."));
        setProducts([]);
      })
      .finally(() => {
        if (!cancelled) setProductsLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [branchId]);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (productId === "") {
      setError("Selecione um produto.");
      return;
    }
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const product = await adjustStock(productId, adjustment, reason);
      setResult(product);
    } catch (err) {
      setError(extractErrorMessage(err, "Não foi possível ajustar o estoque."));
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className={sectionClass}>
      <h2 className="mb-3 text-sm font-semibold text-slate-700">Ajustar Estoque</h2>
      <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass} htmlFor="adjBranchId">
              Filial (ID) — para listar produtos
            </label>
            <input
              id="adjBranchId"
              type="number"
              min={1}
              value={branchId}
              onChange={(e) => setBranchId(Number(e.target.value))}
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} htmlFor="adjProductId">
              Produto
            </label>
            <select
              id="adjProductId"
              value={productId}
              onChange={(e) => setProductId(e.target.value ? Number(e.target.value) : "")}
              className={inputClass}
              disabled={productsLoading}
            >
              <option value="">Selecione...</option>
              {products.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name} ({p.sku})
                </option>
              ))}
            </select>
            {productsError && <p className="mt-1 text-xs text-red-600">{productsError}</p>}
          </div>
        </div>
        <div>
          <label className={labelClass} htmlFor="adjAmount">
            Ajuste (use negativo para reduzir)
          </label>
          <input
            id="adjAmount"
            type="number"
            step="0.01"
            value={adjustment}
            onChange={(e) => setAdjustment(Number(e.target.value))}
            className={inputClass}
          />
        </div>
        <div>
          <label className={labelClass} htmlFor="adjReason">
            Motivo
          </label>
          <input
            id="adjReason"
            required
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            className={inputClass}
          />
        </div>

        {error && (
          <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{error}</div>
        )}
        {result && (
          <div className="rounded-md bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
            Estoque de "{result.name}" ajustado. Novo estoque: <strong>{result.stock}</strong>.
          </div>
        )}

        <button type="submit" disabled={loading} className={buttonClass}>
          {loading ? "Ajustando..." : "Ajustar Estoque"}
        </button>
      </form>
    </section>
  );
}
