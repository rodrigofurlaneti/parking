namespace Parking.Application.Features.VehicleModel.Search;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

// Usado pelo autocomplete: conforme o funcionario digita a placa/modelo, o frontend
// consulta esse endpoint para sugerir o modelo mais proximo ja cadastrado.
public sealed record SearchVehicleModelsQuery(string Query) : IQuery<List<VehicleModelDto>>;
