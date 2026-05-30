namespace PayDa.Domain.Enums;

public enum TransactionStatus
{
    WaitingForTomanPayment,
    TomanPaymentDeclared,
    TomanConfirmed,
    ForeignReceiptUploaded,
    Completed,
    Disputed
}
