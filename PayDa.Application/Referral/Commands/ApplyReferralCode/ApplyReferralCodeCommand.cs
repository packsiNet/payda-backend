using MediatR;

namespace PayDa.Application.Referral.Commands.ApplyReferralCode;

public record ApplyReferralCodeCommand(string ReferralCode) : IRequest;
