using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.Admin.GetApplication;

public class GetApplicationQueryHandler(IRestaurantApplicationRepository applications) : IRequestHandler<GetApplicationQuery, Result<ApplicationResponse>>
{
    public async Task<Result<ApplicationResponse>> Handle(GetApplicationQuery request, CancellationToken cancellationToken)
    {
        var application = await applications.FindWithApplicantAsync(request.ApplicationId, cancellationToken);
        if (application is null) return Result<ApplicationResponse>.Failure(Error.NotFound("APPLICATION_NOT_FOUND", "The restaurant application was not found."));
        return application.Adapt<ApplicationResponse>();
    }
}
