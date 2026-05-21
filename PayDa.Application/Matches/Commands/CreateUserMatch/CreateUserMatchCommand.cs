using MediatR;
using PayDa.Application.Requests.Commands.CreateRequest;

namespace PayDa.Application.Matches.Commands.CreateUserMatch;

public record CreateUserMatchCommand(
    Guid RequestId,
    List<ForeignAccountDto>? ForeignAccounts,
    ReceiverInfoDto? ReceiverInfo
) : IRequest<CreateUserMatchResult>;

public record ReceiverInfoDto(
    string FirstName,
    string LastName,
    string NationalId,
    string MobileNumber,
    string IBAN
);

public record CreateUserMatchResult(Guid MatchId, string Message);
