using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Inventory;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _context;

    public StockService(AppDbContext context) => _context = context;

    public async Task ReceiveStockAsync(
        int itemId, int warehouseId,
        decimal qty, decimal unitCost,
        StockTransactionType transactionType,
        string referenceType, int referenceId,
        string referenceNumber, string createdBy)
    {
        // Update or create stock balance
        var balance = await _context.StockBalances
            .FirstOrDefaultAsync(s =>
                s.ItemId == itemId && s.WarehouseId == warehouseId);

        if (balance == null)
        {
            balance = new StockBalance
            {
                ItemId      = itemId,
                WarehouseId = warehouseId,
                OnHandQty   = 0,
                ReservedQty = 0
            };
            _context.StockBalances.Add(balance);
            await _context.SaveChangesAsync();
        }

        balance.OnHandQty      += qty;
        balance.LastUpdatedAt  = DateTime.UtcNow;

        // Create ledger entry
        var newBalance = balance.OnHandQty;
        _context.StockLedgers.Add(new StockLedger
        {
            ItemId          = itemId,
            WarehouseId     = warehouseId,
            TransactionType = transactionType,
            ReferenceType   = referenceType,
            ReferenceId     = referenceId,
            ReferenceNumber = referenceNumber,
            InwardQty       = qty,
            OutwardQty      = 0,
            BalanceQty      = newBalance,
            UnitCost        = unitCost,
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task IssueStockAsync(
        int itemId, int warehouseId,
        decimal qty,
        StockTransactionType transactionType,
        string referenceType, int referenceId,
        string referenceNumber, string createdBy)
    {
        var balance = await _context.StockBalances
            .FirstOrDefaultAsync(s =>
                s.ItemId == itemId && s.WarehouseId == warehouseId);

        if (balance == null || balance.OnHandQty < qty)
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {balance?.OnHandQty ?? 0}");

        balance.OnHandQty     -= qty;
        balance.LastUpdatedAt  = DateTime.UtcNow;

        var newBalance = balance.OnHandQty;
        _context.StockLedgers.Add(new StockLedger
        {
            ItemId          = itemId,
            WarehouseId     = warehouseId,
            TransactionType = transactionType,
            ReferenceType   = referenceType,
            ReferenceId     = referenceId,
            ReferenceNumber = referenceNumber,
            InwardQty       = 0,
            OutwardQty      = qty,
            BalanceQty      = newBalance,
            UnitCost        = 0,
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetBalanceAsync(int itemId, int warehouseId)
    {
        var balance = await _context.StockBalances
            .FirstOrDefaultAsync(s =>
                s.ItemId == itemId && s.WarehouseId == warehouseId);

        return balance?.OnHandQty ?? 0;
    }
}