namespace Parking.Domain.Entities;

using Parking.Domain.Common;

// Catalogo compartilhado de modelos de veiculo. Existe para padronizar o que os
// funcionarios digitam na entrada de veiculo (autocomplete no frontend), evitando
// que cada um registre o mesmo modelo de um jeito diferente (ex.: "gol", "Gol G5",
// "VW Gol").
public sealed class VehicleModel : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private VehicleModel() { }

    private VehicleModel(string name) : base(0)
    {
        Name = name;
    }

    public static Result<VehicleModel> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<VehicleModel>(new Error("VehicleModel.InvalidName", "Name required."));

        return Result.Success(new VehicleModel(name.Trim()));
    }

    public Result Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(new Error("VehicleModel.InvalidName", "Name required."));

        Name = name.Trim();
        return Result.Success();
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
