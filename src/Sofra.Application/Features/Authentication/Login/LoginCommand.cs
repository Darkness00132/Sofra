using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.Dtos;

namespace Sofra.Application.Features.Authentication.Login;

/// <summary>Signs a user in with email and password, returning a fresh token pair.</summary>
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
