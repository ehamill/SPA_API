using MAWcore6.Data;

namespace MAWcore6.Models
{
    public class BuildingCost
    {
        //{ type: "cottage", reqMet: false, preReq: "TH level 2", food: 100, stone: 100, wood: 500, iron: 50, time: 75 },
        public string typeString {get; set;} = "";
        public bool reqMet { get; set; } = true;
        public string preReq { get; set; } = "";
        public int food { get; set; } = 0;
        public int stone { get; set; } = 0;
        public int wood { get; set; } = 0;
        public int iron { get; set; } = 0;
        public int time { get; set; } = 0;
        public string description { get; set; } = "";
        public BuildingType? buildingTypeInt { get; set; }
        public TroopType? troopType { get; set; }
        public bool farm { get; set; } = false;

    }
}
