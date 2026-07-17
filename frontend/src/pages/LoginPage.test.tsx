import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import axios from "axios";
import LoginPage from "./LoginPage";
import { useAuth } from "../context/AuthContext";

vi.mock("../context/AuthContext", () => ({
  useAuth: vi.fn(),
}));

const mockedNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual<typeof import("react-router-dom")>("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockedNavigate,
  };
});

const mockedUseAuth = vi.mocked(useAuth);

function renderLoginPage() {
  return render(
    <MemoryRouter>
      <LoginPage />
    </MemoryRouter>,
  );
}

describe("LoginPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("calls the login API with the entered credentials and navigates away on success", async () => {
    const mockLogin = vi.fn().mockResolvedValue(undefined);
    mockedUseAuth.mockReturnValue({
      user: null,
      token: null,
      isAuthenticated: false,
      login: mockLogin,
      logout: vi.fn(),
    });

    const user = userEvent.setup();
    renderLoginPage();

    await user.type(screen.getByLabelText("Usuário"), "operator1");
    await user.type(screen.getByLabelText("Senha"), "secret123");
    await user.click(screen.getByRole("button", { name: /Entrar/i }));

    await waitFor(() => expect(mockLogin).toHaveBeenCalledWith("operator1", "secret123"));
    await waitFor(() => expect(mockedNavigate).toHaveBeenCalledWith("/", { replace: true }));
  });

  it("shows an invalid credentials message on 401", async () => {
    const axiosError = Object.assign(new Error("Request failed"), {
      isAxiosError: true,
      response: { status: 401, data: {} },
    });
    const mockLogin = vi.fn().mockRejectedValue(axiosError);
    vi.spyOn(axios, "isAxiosError").mockReturnValue(true);
    mockedUseAuth.mockReturnValue({
      user: null,
      token: null,
      isAuthenticated: false,
      login: mockLogin,
      logout: vi.fn(),
    });

    const user = userEvent.setup();
    renderLoginPage();

    await user.type(screen.getByLabelText("Usuário"), "operator1");
    await user.type(screen.getByLabelText("Senha"), "wrongpass");
    await user.click(screen.getByRole("button", { name: /Entrar/i }));

    expect(await screen.findByText("Usuário ou senha inválidos")).toBeInTheDocument();
    expect(mockedNavigate).not.toHaveBeenCalled();
  });

  it("shows the API error message for non-auth failures", async () => {
    const axiosError = Object.assign(new Error("Request failed"), {
      isAxiosError: true,
      response: { status: 500, data: { error: "Serviço indisponível" } },
    });
    const mockLogin = vi.fn().mockRejectedValue(axiosError);
    vi.spyOn(axios, "isAxiosError").mockReturnValue(true);
    mockedUseAuth.mockReturnValue({
      user: null,
      token: null,
      isAuthenticated: false,
      login: mockLogin,
      logout: vi.fn(),
    });

    const user = userEvent.setup();
    renderLoginPage();

    await user.type(screen.getByLabelText("Usuário"), "operator1");
    await user.type(screen.getByLabelText("Senha"), "secret123");
    await user.click(screen.getByRole("button", { name: /Entrar/i }));

    expect(await screen.findByText("Serviço indisponível")).toBeInTheDocument();
  });
});
