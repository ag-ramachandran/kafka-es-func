namespace com.microsoft.entity.wallet
{
    public class WalletTransaction
    {
        public string id { get; set; }
        public double amount { get; set; }
        public string type { get; set; }
        public string payoutId { get; set; }
        public string payoutStatus { get; set; }
        public string currency { get; set; }
        public double fee { get; set; }
        public string sourceId { get; set; }
        public string sourceType { get; set; }
        public string processedAt { get; set; }
        public string customerId { get; set; }
        public string walletId { get; set; }

        public override string ToString()
        {
            return $"id: {id}, amount: {amount}, type: {type}, payoutId: {payoutId}, payoutStatus: {payoutStatus}, currency: {currency}, fee: {fee}, sourceId: {sourceId}, sourceType: {sourceType}, processedAt: {processedAt}, customerId: {customerId}, walletId: {walletId}";
        }
    }
}
