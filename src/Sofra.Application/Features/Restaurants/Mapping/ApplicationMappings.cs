using Mapster;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.Mapping;

/// <summary>
/// RestaurantApplication → ApplicationResponse. The status is a string in the API contract
/// and the applicant's email comes from the Applicant navigation; the navigation is
/// null-guarded because single reads (GetApplication) load the application without it.
/// </summary>
public class ApplicationMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RestaurantApplication, ApplicationResponse>()
            .Map(d => d.Status, s => s.Status.ToString())
            .Map(d => d.ApplicantEmail, s => s.Applicant != null ? s.Applicant.Email : string.Empty);
    }
}
