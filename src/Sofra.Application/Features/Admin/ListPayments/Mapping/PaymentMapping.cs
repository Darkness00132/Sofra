using Mapster;
using Sofra.Application.Features.Admin.ListPayments;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Admin.ListPayments.Mapping;

public class PaymentMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment, PaymentResponse>()
            .Map(d => d.PaymentMethod, s => s.PaymentMethod.ToString())
            .Map(d => d.Status, s => s.Status.ToString());
    }
}
