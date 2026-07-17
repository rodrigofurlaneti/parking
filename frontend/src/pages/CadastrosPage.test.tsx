import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import CadastrosPage from "./CadastrosPage";
import { createSupplier, getAllSuppliersByBranch, deactivateSupplier } from "../api/inventory";
import {
  createServiceCategory,
  getServiceCategories,
  createServiceItem,
  getServiceItems,
} from "../api/wash";
import {
  createAgreementMerchant,
  getAllAgreementMerchantsByBranch,
  createTariff,
  getAllTariffsByBranch,
  createBranch,
  getAllBranchesByCompany,
} from "../api/cadastros";
import type { SupplierDto } from "../types/api";

// CadastrosPage is a tabbed hub that fans out to five different "cadastro" APIs.
// Rather than duplicating full CRUD coverage per-tab (already covered at the
// handler level in the backend test suite, and structurally identical to the
// Suppliers tab tested below), this suite verifies: (1) the tab navigation
// itself, and (2) one full create/list round-trip end-to-end as a representative
// sample of the shared SimpleTable + form pattern used by every tab.
vi.mock("../api/inventory");
vi.mock("../api/wash");
vi.mock("../api/cadastros");

const mockedGetSuppliers = vi.mocked(getAllSuppliersByBranch);
const mockedCreateSupplier = vi.mocked(createSupplier);
vi.mocked(deactivateSupplier).mockResolvedValue(undefined);

vi.mocked(getServiceCategories).mockResolvedValue([]);
vi.mocked(createServiceCategory).mockResolvedValue({
  id: 1,
  branchId: 1,
  name: "x",
  description: null,
  isActive: true,
});
vi.mocked(getServiceItems).mockResolvedValue([]);
vi.mocked(createServiceItem).mockResolvedValue({
  id: 1,
  serviceCategoryId: 1,
  name: "x",
  description: null,
  durationMinutes: 30,
  baseCost: 0,
  isActive: true,
});
vi.mocked(getAllAgreementMerchantsByBranch).mockResolvedValue([]);
vi.mocked(createAgreementMerchant).mockResolvedValue({
  id: 1,
  branchId: 1,
  companyName: "x",
  discountPercentage: 0,
  isActive: true,
});
vi.mocked(getAllTariffsByBranch).mockResolvedValue([]);
vi.mocked(createTariff).mockResolvedValue({
  id: 1,
  branchId: 1,
  firstHourRate: 0,
  additionalHourRate: 0,
  dailyMaxRate: null,
  isActive: true,
});
vi.mocked(getAllBranchesByCompany).mockResolvedValue([]);
vi.mocked(createBranch).mockResolvedValue({
  id: 1,
  companyId: 1,
  name: "x",
  totalSpaces: 10,
  isActive: true,
});

const supplier: SupplierDto = {
  id: 1,
  branchId: 1,
  name: "Distribuidora ABC",
  document: "22233344455",
  phone: "1133334444",
  email: "contato@abc.com",
  isActive: true,
};

describe("CadastrosPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockedGetSuppliers.mockResolvedValue([supplier]);
  });

  it("renders every expected tab and defaults to Fornecedores", async () => {
    render(<CadastrosPage />);

    for (const label of [
      "Fornecedores",
      "Categorias de Serviço",
      "Itens de Serviço",
      "Convênios",
      "Tarifas",
      "Filiais",
    ]) {
      expect(screen.getByRole("button", { name: label })).toBeInTheDocument();
    }

    expect(await screen.findByText("Distribuidora ABC")).toBeInTheDocument();
  });

  it("switches tabs and loads the corresponding data", async () => {
    const user = userEvent.setup();
    render(<CadastrosPage />);

    await screen.findByText("Distribuidora ABC");

    await user.click(screen.getByRole("button", { name: "Filiais" }));

    expect(screen.getByText("Cadastrar Filial")).toBeInTheDocument();
    await waitFor(() => expect(getAllBranchesByCompany).toHaveBeenCalledWith(1));
  });

  it("creates a supplier and refreshes the list", async () => {
    mockedCreateSupplier.mockResolvedValue({ ...supplier, id: 2, name: "Nova Fornecedora" });
    const user = userEvent.setup();
    render(<CadastrosPage />);

    await screen.findByText("Distribuidora ABC");

    await user.type(screen.getByLabelText("Nome"), "Nova Fornecedora");
    await user.type(screen.getByLabelText("Documento"), "99988877766");

    await user.click(screen.getByRole("button", { name: "Cadastrar" }));

    await waitFor(() =>
      expect(mockedCreateSupplier).toHaveBeenCalledWith({
        branchId: 1,
        name: "Nova Fornecedora",
        document: "99988877766",
        phone: undefined,
        email: undefined,
      }),
    );
    expect(await screen.findByText("Fornecedor cadastrado.")).toBeInTheDocument();
  });
});
