using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.Dtos;

namespace Sofra.Application.Features.Authentication.Register;

/// <summary>Creates a customer account and immediately signs the user in with a fresh token pair.</summary>
public record RegisterCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
