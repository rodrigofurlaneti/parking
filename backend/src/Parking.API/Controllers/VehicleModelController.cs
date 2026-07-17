namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.VehicleModel.CreateVehicleModel;
using Parking.Application.Features.VehicleModel.GetAll;
using Parking.Application.Features.VehicleModel.Search;

// Catalogo compartilhado de modelos de veiculo, usado pelo autocomplete de entrada rapida
// (ver VehicleEntryController.RegisterEntryByPlate) para padronizar o que os funcionarios
// digitam.
[Route("api/vehicle-model")]
public sealed class VehicleModelController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateVehicleModelCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllVehicleModelsQuery(), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SearchVehicleModelsQuery(q), cancellationToken);
        return HandleFailure(result);
    }
}
