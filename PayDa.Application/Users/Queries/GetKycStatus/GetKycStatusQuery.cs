using MediatR;
using PayDa.Domain.Enums;

namespace PayDa.Application.Users.Queries.GetKycStatus;

public record GetKycStatusQuery : IRequest<KycStatusDto>;

public record KycStatusDto(KycStatus Status, string DisplayName);
