namespace ErpCrm.Core.Entities {
    public class TenantSubscription {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int SubscriptionPlanId { get; set; }
    }
}