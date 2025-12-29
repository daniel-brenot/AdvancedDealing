using System.Collections.Generic;

namespace AdvancedDealing.Persistence.Datas
{
    public class SaveDataContainer : DataContainer
    {
        public List<DealerDataContainer> Dealers = [];

        public SaveDataContainer(string identifier) : base(identifier) { }
    }
}
