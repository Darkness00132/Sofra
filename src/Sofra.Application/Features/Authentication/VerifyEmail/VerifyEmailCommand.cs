using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Authentication.VerifyEmail;

/// <summary>Confirms a user's email address using the token issued at registration.</summary>
public record VerifyEmailCommand(string Email, string Token) : IRequest<Result>;