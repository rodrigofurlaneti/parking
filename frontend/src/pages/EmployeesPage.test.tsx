import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import EmployeesPage from "./EmployeesPage";
import {
  createEmployee,
  getAllEmployeesByBranch,
  updateEmployee,
  terminateEmployee,
} from "../api/employee";
import type { EmployeeDto } from "../types/api";

vi.mock("../api/employee");

const mockedCreate = vi.mocked(createEmployee);
const mockedGetAll = vi.mocked(getAllEmployeesByBranch);
const mockedUpdate = vi.mocked(updateEmployee);
const mockedTerminate = vi.mocked(terminateEmployee);

const employee: EmployeeDto = {
  id: 1,
  companyId: 1,
  branchId: 1,
  name: "Maria Souza",
  email: "maria@example.com",
  phone: "11999998888",
  cpf: "12345678900",
  hireDate: "2024-01-01",
  terminationDate: null,
  roleId: 2,
  isActive: true,
};

describe("EmployeesPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockedGetAll.mockResolvedValue([employee]);
  });

  it("renders the employee list from the mocked API", async () => {
    render(<EmployeesPage />);

    expect(await screen.findByText("Maria Souza")).toBeInTheDocument();
    expect(screen.getByText("maria@example.com")).toBeInTheDocument();
    expect(mockedGetAll).toHaveBeenCalledWith(1);
  });

  it("submits the create form and calls createEmployee with the right payload", async () => {
    mockedCreate.mockResolvedValue({ ...employee, id: 2 });
    const user = userEvent.setup();
    render(<EmployeesPage />);

    await screen.findByText("Maria Souza");

    await user.type(screen.getByLabelText("Nome"), "João Lima");
    await user.type(screen.getByLabelText("Email"), "joao@example.com");
    await user.type(screen.getByLabelText("Telefone"), "11988887777");
    await user.type(screen.getByLabelText("CPF"), "98765432100");

    await user.click(screen.getByRole("button", { name: "Cadastrar Funcionário" }));

    await waitFor(() =>
      expect(mockedCreate).toHaveBeenCalledWith({
        companyId: 1,
        branchId: 1,
        name: "João Lima",
        email: "joao@example.com",
        phone: "11988887777",
        cpf: "98765432100",
        roleId: 1,
      }),
    );
    expect(await screen.findByText("Funcionário cadastrado com sucesso.")).toBeInTheDocument();
  });

  it("switches to edit mode and calls updateEmployee with the updated fields", async () => {
    mockedUpdate.mockResolvedValue({ ...employee, name: "Maria Souza Silva" });
    const user = userEvent.setup();
    render(<EmployeesPage />);

    await screen.findByText("Maria Souza");
    await user.click(screen.getByRole("button", { name: "Editar" }));

    const nameInput = screen.getByLabelText("Nome") as HTMLInputElement;
    expect(nameInput.value).toBe("Maria Souza");
    await user.clear(nameInput);
    await user.type(nameInput, "Maria Souza Silva");

    await user.click(screen.getByRole("button", { name: "Salvar Alterações" }));

    await waitFor(() =>
      expect(mockedUpdate).toHaveBeenCalledWith(1, {
        name: "Maria Souza Silva",
        email: "maria@example.com",
        phone: "11999998888",
        roleId: 2,
      }),
    );
  });

  it("terminates an employee after confirmation", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(true);
    mockedTerminate.mockResolvedValue(undefined);
    const user = userEvent.setup();
    render(<EmployeesPage />);

    const row = (await screen.findByText("Maria Souza")).closest("tr")!;
    await user.click(within(row).getByRole("button", { name: "Desligar" }));

    await waitFor(() => expect(mockedTerminate).toHaveBeenCalledWith(1));
  });
});
