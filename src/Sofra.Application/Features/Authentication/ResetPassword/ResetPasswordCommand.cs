using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Authentication.ResetPassword;

/// <summary>Resets the password using the one-time code that was emailed by the forgot-password flow.</summary>
public record ResetPasswordCommand(string Email, string Otp, string NewPassword) : IRequest<Result>;
