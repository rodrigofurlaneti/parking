import { useEffect, useRef, useState } from "react";
import { searchVehicleModels, createVehicleModel } from "../api/vehicleModel";
import type { VehicleModelDto } from "../types/api";

// Autocomplete de modelo de veiculo. Existe para padronizar o que os funcionarios
// digitam (evitar "gol", "Gol G5", "VW Gol" para o mesmo carro): conforme a pessoa
// digita, sugere os modelos mais proximos ja cadastrados no catalogo. Se o funcionario
// terminar de digitar algo que não existe ainda, o modelo novo é adicionado ao catalogo
// (get-or-create) para ficar disponível nas próximas entradas.
export default function ModelAutocomplete({
  id,
  value,
  onChange,
  className,
  placeholder,
}: {
  id?: string;
  value: string;
  onChange: (value: string) => void;
  className?: string;
  placeholder?: string;
}) {
  const [suggestions, setSuggestions] = useState<VehicleModelDto[]>([]);
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    let cancelled = false;
    if (!value.trim()) {
      setSuggestions([]);
      return;
    }
    const timer = setTimeout(() => {
      searchVehicleModels(value)
        .then((results) => {
          if (!cancelled) setSuggestions(results);
        })
        .catch(() => {
          if (!cancelled) setSuggestions([]);
        });
    }, 200);
    return () => {
      cancelled = true;
      clearTimeout(timer);
    };
  }, [value]);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function handleBlur() {
    // Pequeno atraso para permitir o clique numa sugestão antes de fechar/registrar.
    setTimeout(() => {
      setOpen(false);
      const trimmed = value.trim();
      const alreadyKnown = suggestions.some(
        (s) => s.name.toLowerCase() === trimmed.toLowerCase(),
      );
      if (trimmed && !alreadyKnown) {
        // Registra o modelo novo no catálogo (get-or-create) para ficar disponível nas
        // próximas sugestões. Silencioso: não bloqueia o operador se falhar.
        createVehicleModel({ name: trimmed }).catch(() => {});
      }
    }, 150);
  }

  return (
    <div ref={containerRef} className="relative">
      <input
        id={id}
        value={value}
        onChange={(e) => {
          onChange(e.target.value);
          setOpen(true);
        }}
        onFocus={() => setOpen(true)}
        onBlur={handleBlur}
        className={className}
        placeholder={placeholder}
        autoComplete="off"
      />
      {open && suggestions.length > 0 && (
        <ul className="absolute z-10 mt-1 max-h-48 w-full overflow-auto rounded-md border border-slate-200 bg-white text-sm shadow-lg">
          {suggestions.map((s) => (
            <li key={s.id}>
              <button
                type="button"
                className="block w-full px-3 py-1.5 text-left hover:bg-slate-100"
                onMouseDown={(e) => e.preventDefault()}
                onClick={() => {
                  onChange(s.name);
                  setOpen(false);
                }}
              >
                {s.name}
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
