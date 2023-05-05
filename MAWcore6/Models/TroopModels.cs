using MAWcore6.Data;

namespace MAWcore6.Models
{
    public class TroopModels
    {
        public class Troop
        {
            public string TypeString { get; set; } = "";
            public TroopType TypeInt { get; set; }
            public string PreReq { get; set; } = "";
            public bool ReqMet { get; set; } = false;
            public string Description { get; set; } = "";
            public int Qty { get; set; } = 0;
            public int FoodCost { get; set; } = 0;
            public int StoneCost { get; set; } = 0;
            public int WoodCost { get; set; } = 0;
            public int IronCost { get; set; } = 0;
            public int TimeCost { get; set; } = 0;
            public bool ForWalls { get; set; } = false;
            public int Attack { get; set; } = 0;
            public int Defense { get; set; } = 0;
            public int Speed { get; set; } = 0;
            public int Load { get; set; } = 0;
            public int Life { get; set; } = 0;
            public int Range { get; set; } = 0;
            public string Image { get; set; } = "missing.jpg";
        }

        public class TroopPreReqCheck
        {
            public string BuildingType { get; set; }
            public bool ReqMet { get; set; }
        }
        public class TrainTroopsModel
        {
            public int CityId { get; set; }
            public int BuildingId { get; set; }
            public int TroopTypeInt { get; set; }
            public int Qty { get; set; }
        }

    }
}
