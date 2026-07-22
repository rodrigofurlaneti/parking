import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import CustomersPage from "./CustomersPage";
import {
  createCustomer,
  getCustomerByDocument,
  getAllCustomersByBranch,
  createAgreementMerchant,
} from "../api/customer";
import { searchVehicleModels, createVehicleModel } from "../api/vehicleModel";
import type { CustomerDto } from "../types/api";

vi.mock("../api/customer");
vi.mock("../api/vehicleModel");

const mockedCreateCustomer = vi.mocked(createCustomer);
const mockedGetByDocument = vi.mocked(getCustomerByDocument);
const mockedGetAllByBranch = vi.mocked(getAllCustomersByBranch);
const mockedCreateAgreementMerchant = vi.mocked(createAgreementMerchant);
const mockedSearchVehicleModels = vi.mocked(searchVehicleModels);
const mockedCreateVehicleModel = vi.mocked(createVehicleModel);

const customer: CustomerDto = {
  id: 1,
  branchId: 1,
  customerType: 1,
  name: "Carlos Pereira",
  document: "11122233344",
  phone: "11977776666",
  email: "carlos@example.com",
  isActive: true,
};

describe("CustomersPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockedGetAllByBranch.mockResolvedValue([customer]);
    mockedSearchVehicleModels.mockResolvedValue([]);
    mockedCreateVehicleModel.mockResolvedValue({ id: 1, name: "x", isActive: true });
  });

  it("renders all the creation/search sections", async () => {
    render(<CustomersPage />);

    expect(screen.getByText("Criar Cliente")).toBeInTheDocument();
    expect(screen.getByText("Buscar Cliente por Documento")).toBeInTheDocument();
    expect(screen.getByText("Criar Veículo")).toBeInTheDocument();
    await waitFor(() => expect(mockedGetAllByBranch).toHaveBeenCalledWith(1));
  });

  it("submits the create-customer form and calls createCustomer with the right payload", async () => {
    mockedCreateCustomer.mockResolvedValue({ ...customer, id: 5 });
    const user = userEvent.setup();
    render(<CustomersPage />);

    await user.type(screen.getByLabelText("Nome"), "Ana Costa");
    await user.type(document.getElementById("custDocument")!, "55566677788");
    await user.type(screen.getByLabelText("Telefone"), "11955554444");
    await user.type(screen.getByLabelText("E-mail"), "ana@example.com");

    await user.click(screen.getByRole("button", { name: "Criar Cliente" }));

    await waitFor(() =>
      expect(mockedCreateCustomer).toHaveBeenCalledWith({
        branchId: 1,
        customerType: 1,
        name: "Ana Costa",
        document: "55566677788",
        phone: "11955554444",
        email: "ana@example.com",
      }),
    );
    expect(await screen.findByText(/Cliente criado com ID/)).toBeInTheDocument();
  });

  it("searches a customer by document and displays the result", async () => {
    mockedGetByDocument.mockResolvedValue(customer);
    const user = userEvent.setup();
    render(<CustomersPage />);

    await user.type(document.getElementById("searchDocument")!, "11122233344");
    await user.click(screen.getByRole("button", { name: "Buscar" }));

    await waitFor(() => expect(mockedGetByDocument).toHaveBeenCalledWith("11122233344"));
    expect(await screen.findByText("Carlos Pereira")).toBeInTheDocument();
  });

  it("shows an error message when the customer search fails", async () => {
    mockedGetByDocument.mockRejectedValue(new Error("not found"));
    const user = userEvent.setup();
    render(<CustomersPage />);

    await user.type(document.getElementById("searchDocument")!, "00000000000");
    await user.click(screen.getByRole("button", { name: "Buscar" }));

    expect(await screen.findByText("Cliente não encontrado.")).toBeInTheDocument();
  });

  it("submits the create-agreement-merchant form", async () => {
    mockedCreateAgreementMerchant.mockResolvedValue({
      id: 9,
      branchId: 1,
      companyName: "Empresa X",
      discountPercentage: 10,
      isActive: true,
    });
    const user = userEvent.setup();
    render(<CustomersPage />);

    await user.type(screen.getByLabelText("Nome da Empresa"), "Empresa X");
    await user.click(screen.getByRole("button", { name: "Criar Empresa" }));

    await waitFor(() =>
      expect(mockedCreateAgreementMerchant).toHaveBeenCalledWith({
        branchId: 1,
        companyName: "Empresa X",
        discountPercentage: 0,
      }),
    );
  });
});
