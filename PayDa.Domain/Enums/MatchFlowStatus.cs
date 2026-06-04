namespace PayDa.Domain.Enums;

public enum MatchFlowStatus
{
    WaitingForBothConfirmations,
    WaitingForCounterpartConfirmation,
    WaitingForMyConfirmation,
    WaitingForTomanPayment,
    TomanPaymentDeclared,
    WaitingForForeignTransfer,
    ForeignReceiptUploaded,
    Completed,
    Disputed
}
