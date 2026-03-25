using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Application.Common.Interfaces;

public interface IStockService
{
    Task ReceiveStockAsync(
        int itemId, int warehouseId,
        decimal qty, decimal unitCost,
        StockTransactionType transactionType,
        string referenceType, int referenceId,
        string referenceNumber, string createdBy);

    Task IssueStockAsync(
        int itemId, int warehouseId,
        decimal qty,
        StockTransactionType transactionType,
        string referenceType, int referenceId,
        string referenceNumber, string createdBy);

    Task<decimal> GetBalanceAsync(int itemId, int warehouseId);
}