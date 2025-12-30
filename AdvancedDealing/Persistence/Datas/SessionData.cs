namespace AdvancedDealing.Persistence.Datas
{
    public class SessionData : DataBase
    {
        public bool LoyalityMode;

        public bool AccessInventory;

        public bool CheatMenu;

        public SessionData(string identifier) : base(identifier) { }
    }
}
