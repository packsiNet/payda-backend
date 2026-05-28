using MediatR;

namespace PayDa.Application.Settings.Queries.GetMatchConfirmationHours;

public record GetMatchConfirmationHoursQuery : IRequest<int>;
