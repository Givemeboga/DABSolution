namespace DAB.API.Models
{
    /// <summary>
    /// Enum representing account status in the banking system
    /// </summary>
    public enum EtatCompte
    {
        Actif = 0,           // Active account - normal operations
        Gelé = 1,            // Frozen - temporary restriction (requires admin action)
        Suspendu = 2,        // Suspended - automatic due to security (e.g., multiple failed PINs)
        Fermé = 3            // Closed - permanently closed account
    }

    /// <summary>
    /// Enum for account security status
    /// </summary>
    public enum StatutSecurité
    {
        Normal = 0,
        TentativesÉchoueesMultiples = 1,  // Multiple failed PIN attempts
        ActivitéSuspecte = 2               // Suspicious activity detected
    }
}
