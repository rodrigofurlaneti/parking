namespace Parking.Application.Features.VehicleEntry.RegisterVehicleEntryByPlate;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

// Fluxo de "entrada rapida": o operador informa so a placa (+ vaga). O handler resolve
// sozinho quem e o cliente:
// - Se a placa ja tiver um veiculo cadastrado, usa o cliente dono desse veiculo (com todas as
//   regras de mensalista/convenio que ja existiam em RegisterVehicleEntryCommand).
// - Se for uma placa nunca vista, cria automaticamente um cliente "Avulso" (Rotativo) e o
//   veiculo correspondente, sem exigir nenhum cadastro manual previo.
public sealed record RegisterVehicleEntryByPlateCommand(
    long BranchId,
    long ParkingSpaceId,
    string LicensePlate,
    string? VehicleModel,
    string? VehicleColor) : ICommand<VehicleEntryByPlateResultDto>;
