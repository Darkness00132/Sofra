using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Authentication.ForgotPassword;

/// <summary>
/// Emails a one-time password reset code to the user. The response is the same whether or
/// not the email is registered, so the endpoint never reveals account existence.
/// </summary>
public record ForgotPasswordCommand(string Email) : IRequest<Result>;
