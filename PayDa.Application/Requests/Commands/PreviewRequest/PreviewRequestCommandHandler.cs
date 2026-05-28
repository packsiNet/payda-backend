using MediatR;

namespace PayDa.Application.Requests.Commands.PreviewRequest;

public class PreviewRequestCommandHandler : IRequestHandler<PreviewRequestCommand, PreviewRequestResult>
{
    public Task<PreviewRequestResult> Handle(PreviewRequestCommand cmd, CancellationToken ct)
        => Task.FromResult(new PreviewRequestResult(cmd.Amount, cmd.Currency, cmd.PricePreference));
}
