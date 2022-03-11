using MAWcore6.Data;

namespace MAWcore6.Models
{
    public class BuildingCost
    {
        //{ type: "cottage", reqMet: false, preReq: "TH level 2", food: 100, stone: 100, wood: 500, iron: 50, time: 75 },
        public string type {get; set;} = "";
        public bool reqMet { get; set; } = true;
        public string preReq { get; set; } = "";
        public int food { get; set; }
        public int stone { get; set; }
        public int wood { get; set; }
        public int iron { get; set; }
        public int time { get; set; }
        public string description { get; set; } = "";
    }
}
