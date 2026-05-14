namespace DAB.API.Models
{
    /// <summary>
    /// Enum for transaction categories
    /// </summary>
    public enum CatégorieTransaction
    {
        Retrait = 0,           // Withdrawal at ATM
        Transfert = 1,         // Transfer to another account
        Dépôt = 2,             // Deposit (future feature)
        Frais = 3,             // Service fees
        Intérêt = 4            // Interest or dividend
    }

    /// <summary>
    /// Enum for transaction status
    /// </summary>
    public enum StatutTransaction
    {
        Réussie = 0,           // Successful
        Échouée = 1,           // Failed
        Annulée = 2,           // Cancelled
        Remboursée = 3         // Refunded (dispute resolution)
    }
}
