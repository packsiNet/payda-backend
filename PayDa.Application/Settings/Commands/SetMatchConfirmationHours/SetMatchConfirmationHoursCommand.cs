using MediatR;

namespace PayDa.Application.Settings.Commands.SetMatchConfirmationHours;

public record SetMatchConfirmationHoursCommand(int Hours) : IRequest;
