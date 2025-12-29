using System.Collections.Generic;

namespace AdvancedDealing.Persistence.Datas
{
    public class SaveData(string identifier) : Data(identifier)
    {
        public List<DealerData> Dealers;
        
        public override void SetDefaults()
        {
            Dealers = [];
        }
    }
}
