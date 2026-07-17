import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { useState } from "react";
import ModelAutocomplete from "./ModelAutocomplete";
import { searchVehicleModels, createVehicleModel } from "../api/vehicleModel";
import type { VehicleModelDto } from "../types/api";

vi.mock("../api/vehicleModel");

const mockedSearch = vi.mocked(searchVehicleModels);
const mockedCreate = vi.mocked(createVehicleModel);

function ControlledWrapper() {
  const [value, setValue] = useState("");
  return <ModelAutocomplete id="model" value={value} onChange={setValue} />;
}

describe("ModelAutocomplete", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("searches and shows suggestions as the user types", async () => {
    const results: VehicleModelDto[] = [
      { id: 1, name: "Gol", isActive: true },
      { id: 2, name: "Gol G5", isActive: true },
    ];
    mockedSearch.mockResolvedValue(results);
    mockedCreate.mockResolvedValue({ id: 3, name: "Gol", isActive: true });

    const user = userEvent.setup();
    render(<ControlledWrapper />);

    const input = screen.getByRole("textbox");
    await user.type(input, "Gol");

    await waitFor(() => expect(mockedSearch).toHaveBeenCalledWith("Gol"));

    expect(await screen.findByText("Gol G5")).toBeInTheDocument();
    expect(screen.getAllByText("Gol").length).toBeGreaterThan(0);
  });

  it("fills the input when a suggestion is selected", async () => {
    const results: VehicleModelDto[] = [{ id: 1, name: "Onix LTZ", isActive: true }];
    mockedSearch.mockResolvedValue(results);
    mockedCreate.mockResolvedValue({ id: 1, name: "Onix LTZ", isActive: true });

    const user = userEvent.setup();
    render(<ControlledWrapper />);

    const input = screen.getByRole("textbox") as HTMLInputElement;
    await user.type(input, "Onix");

    const suggestionButton = await screen.findByRole("button", { name: "Onix LTZ" });
    await user.click(suggestionButton);

    expect(input.value).toBe("Onix LTZ");
    // The list should close after selecting a suggestion.
    expect(screen.queryByRole("button", { name: "Onix LTZ" })).not.toBeInTheDocument();
  });

  it("registers a brand-new model (get-or-create) on blur when it isn't in the suggestions", async () => {
    mockedSearch.mockResolvedValue([]);
    mockedCreate.mockResolvedValue({ id: 10, name: "Brand New Model", isActive: true });

    const user = userEvent.setup();
    render(
      <div>
        <ControlledWrapper />
        <button type="button">outside</button>
      </div>,
    );

    const input = screen.getByRole("textbox");
    await user.type(input, "Brand New Model");
    await user.click(screen.getByRole("button", { name: "outside" }));

    await waitFor(() =>
      expect(mockedCreate).toHaveBeenCalledWith({ name: "Brand New Model" }),
    );
  });

  it("does not create a duplicate when the typed value already matches a known suggestion", async () => {
    const results: VehicleModelDto[] = [{ id: 5, name: "Civic", isActive: true }];
    mockedSearch.mockResolvedValue(results);

    const user = userEvent.setup();
    render(
      <div>
        <ControlledWrapper />
        <button type="button">outside</button>
      </div>,
    );

    const input = screen.getByRole("textbox");
    await user.type(input, "Civic");
    await waitFor(() => expect(mockedSearch).toHaveBeenCalled());
    await user.click(screen.getByRole("button", { name: "outside" }));

    await waitFor(() => {
      // small delay window for handleBlur's setTimeout to run
    });
    // give the blur timeout (150ms) time to run without asserting a call happened
    await new Promise((resolve) => setTimeout(resolve, 250));
    expect(mockedCreate).not.toHaveBeenCalled();
  });
});
