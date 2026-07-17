import type { ReactNode } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const NAV_ITEMS = [
  { to: "/", label: "Dashboard" },
  { to: "/patio", label: "Pátio" },
  { to: "/vendas", label: "Vendas" },
  { to: "/clientes", label: "Clientes" },
  { to: "/lava-rapido", label: "Lava Rápido" },
  { to: "/estoque", label: "Estoque" },
  { to: "/funcionarios", label: "Funcionários" },
  { to: "/cadastros", label: "Cadastros" },
];

export default function Layout({ children }: { children: ReactNode }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login", { replace: true });
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <header className="bg-slate-900 text-white shadow-md">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-3 sm:px-6 lg:px-8">
          <div className="flex items-center gap-2">
            <span className="text-xl font-bold tracking-tight">Parking</span>
            <span className="hidden text-sm text-slate-400 sm:inline">
              Gestão de Estacionamento
            </span>
          </div>
          <div className="flex items-center gap-4">
            {user && (
              <span className="text-sm text-slate-200">
                Olá, <strong>{user.userName}</strong>
              </span>
            )}
            <button
              type="button"
              onClick={handleLogout}
              className="rounded-md bg-slate-700 px-3 py-1.5 text-sm font-medium text-white transition hover:bg-slate-600"
            >
              Sair
            </button>
          </div>
        </div>
        <nav className="border-t border-slate-800">
          <div className="mx-auto flex max-w-7xl flex-wrap gap-1 px-4 py-2 sm:px-6 lg:px-8">
            {NAV_ITEMS.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.to === "/"}
                className={({ isActive }) =>
                  `rounded-md px-3 py-1.5 text-sm font-medium transition ${
                    isActive
                      ? "bg-slate-700 text-white"
                      : "text-slate-300 hover:bg-slate-800 hover:text-white"
                  }`
                }
              >
                {item.label}
              </NavLink>
            ))}
          </div>
        </nav>
      </header>
      <main className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">{children}</main>
    </div>
  );
}
