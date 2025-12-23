namespace AdvancedDealing.Persistence.Datas
{
    public abstract class DataBase(string identifier)
    {
        public virtual string DataType => GetType().Name;

        public string ModVersion = ModInfo.k_Version;

        public string Identifier = identifier;

        public virtual void SetDefaults() { }
    }
}
