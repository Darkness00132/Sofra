using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Admin.PlatformDashboard;

/// <summary>Platform-wide statistics for the administrator dashboard.</summary>
public record PlatformDashboardQuery : IRequest<Result<AdminDashboardResponse>>;
