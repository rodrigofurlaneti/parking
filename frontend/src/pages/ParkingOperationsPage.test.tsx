import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import ParkingOperationsPage from "./ParkingOperationsPage";
import {
  getParkingSpaceOccupancy,
  getAllParkingSpacesByBranch,
  getOpenVehicleEntriesByBranch,
  getOpenCashRegisterByBranch,
  registerVehicleEntryByPlate,
} from "../api/vehicleEntry";
import { getAllCustomersByBranch } from "../api/customer";
import { searchVehicleModels, createVehicleModel } from "../api/vehicleModel";
import type { ParkingSpaceDto, VehicleEntryByPlateResultDto } from "../types/api";

// ParkingOperationsPage bundles five independent sections (occupancy/space
// creation, quick entry by plate, manual entry, exit, cash register), each
// with its own data fetching on mount. Per the task brief, this suite focuses
// on the Quick Entry by Plate flow (QuickEntrySection) since it's the newest
// and most business-critical piece; the other sections are exercised only
// enough to let the page mount without errors (their mocks resolve to empty
// data so they don't interfere with the quick-entry assertions).
vi.mock("../api/vehicleEntry");
vi.mock("../api/customer");
vi.mock("../api/vehicleModel");

const mockedGetOccupancy = vi.mocked(getParkingSpaceOccupancy);
const mockedGetSpaces = vi.mocked(getAllParkingSpacesByBranch);
const mockedGetOpenEntries = vi.mocked(getOpenVehicleEntriesByBranch);
const mockedGetOpenCashRegister = vi.mocked(getOpenCashRegisterByBranch);
const mockedRegisterByPlate = vi.mocked(registerVehicleEntryByPlate);
const mockedGetCustomers = vi.mocked(getAllCustomersByBranch);
const mockedSearchModels = vi.mocked(searchVehicleModels);
const mockedCreateModel = vi.mocked(createVehicleModel);

const availableSpace: ParkingSpaceDto = {
  id: 10,
  branchId: 1,
  spaceNumber: "A-01",
  type: 1,
  status: 0,
  isActive: true,
};

function setupCommonMocks() {
  mockedGetOccupancy.mockResolvedValue({
    branchId: 1,
    totalSpaces: 10,
    occupiedSpaces: 2,
    availableSpaces: 8,
    occupancyRate: 20,
  });
  mockedGetSpaces.mockResolvedValue([availableSpace]);
  mockedGetOpenEntries.mockResolvedValue([]);
  mockedGetOpenCashRegister.mockResolvedValue(null);
  mockedGetCustomers.mockResolvedValue([]);
  mockedSearchModels.mockResolvedValue([]);
  mockedCreateModel.mockResolvedValue({ id: 1, name: "x", isActive: true });
}

describe("ParkingOperationsPage — Quick Entry by Plate", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setupCommonMocks();
  });

  it("loads the available parking spaces for the quick entry dropdown", async () => {
    render(<ParkingOperationsPage />);

    await waitFor(() => expect(mockedGetSpaces).toHaveBeenCalledWith(1));
    expect(await screen.findByText(/A-01 \(disponível\)/)).toBeInTheDocument();
  });

  it("submits the quick entry form and calls registerVehicleEntryByPlate with the right payload", async () => {
    const result: VehicleEntryByPlateResultDto = {
      id: 42,
      branchId: 1,
      parkingSpaceId: 10,
      customerId: 7,
      customerName: "Fernanda Alves",
      customerType: 1,
      isNewCustomer: true,
      licensePlate: "ABC1D23",
      vehicleModel: "Onix",
      vehicleColor: "Prata",
      entryTime: "2026-07-17T10:00:00Z",
      exitTime: null,
      status: 1,
      isActive: true,
    };
    mockedRegisterByPlate.mockResolvedValue(result);

    const user = userEvent.setup();
    render(<ParkingOperationsPage />);

    // Wait for the parking-space dropdown to populate and auto-select A-01.
    await screen.findByText(/A-01 \(disponível\)/);

    const plateInput = screen.getByLabelText("Placa");
    await user.type(plateInput, "abc1d23");

    const colorInput = screen.getByLabelText("Cor (opcional)");
    await user.type(colorInput, "Prata");

    const submitButton = screen.getByRole("button", { name: "Registrar Entrada" });
    await user.click(submitButton);

    await waitFor(() =>
      expect(mockedRegisterByPlate).toHaveBeenCalledWith({
        branchId: 1,
        parkingSpaceId: 10,
        licensePlate: "ABC1D23",
        vehicleModel: undefined,
        vehicleColor: "Prata",
      }),
    );

    expect(await screen.findByText(/Entrada #42 registrada para a placa/)).toBeInTheDocument();
    expect(screen.getByText("Fernanda Alves", { exact: false })).toBeInTheDocument();
    expect(screen.getByText(/cadastrado automaticamente agora/)).toBeInTheDocument();

    // The plate field is cleared after a successful submission.
    expect((plateInput as HTMLInputElement).value).toBe("");
  });

  it("shows an error message when the entry-by-plate request fails", async () => {
    mockedRegisterByPlate.mockRejectedValue({
      isAxiosError: true,
      response: { status: 409, data: { error: "Placa já está estacionada." } },
    });

    const user = userEvent.setup();
    render(<ParkingOperationsPage />);

    await screen.findByText(/A-01 \(disponível\)/);

    await user.type(screen.getByLabelText("Placa"), "XYZ9Z99");
    await user.click(screen.getByRole("button", { name: "Registrar Entrada" }));

    expect(await screen.findByText("Placa já está estacionada.")).toBeInTheDocument();
  });

  it("requires both plate and parking space before submitting", async () => {
    mockedGetSpaces.mockResolvedValue([]);
    const user = userEvent.setup();
    render(<ParkingOperationsPage />);

    await waitFor(() => expect(mockedGetSpaces).toHaveBeenCalled());

    // No spaces available => no plate typed either; submit should surface
    // the client-side validation message and never call the API.
    const plateInput = screen.getByLabelText("Placa") as HTMLInputElement;
    plateInput.removeAttribute("required");
    await user.click(screen.getByRole("button", { name: "Registrar Entrada" }));

    expect(await screen.findByText("Informe a placa e selecione uma vaga.")).toBeInTheDocument();
    expect(mockedRegisterByPlate).not.toHaveBeenCalled();
  });
});
