namespace AdvancedDealing.Persistence.Datas
{
    public class DealerData(string identifier) : DataBase(identifier)
    {
        public string DeadDrop;

        public bool IsFired;

        public int MaxCustomers;

        public int ItemSlots;

        public float Cut;

        public float SpeedMultiplier;

        // Stats
        public float Experience;

        public float Loyality;

        // Behavior
        public bool DeliverCash;

        public bool NotifyOnCashDelivery;

        public float CashThreshold;

        public override void SetDefaults()
        {
            DeadDrop = null;
            IsFired = false;
            MaxCustomers = 8;
            ItemSlots = 5;
            Cut = 0.2f;
            SpeedMultiplier = 1f;
            Experience = 0f;
            Loyality = 50f;
            DeliverCash = false;
            NotifyOnCashDelivery = true;
            CashThreshold = 1500f;
        }
    }
}
