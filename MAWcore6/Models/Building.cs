using MAWcore6.Data;

namespace MAWcore6.Models
{
    public class Building
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public int Location { get; set; } // 0-25 ..2 is always TH
        public BuildingType BuildingType {get; set;}
        public int Level { get; set; }
        public string Image { get; set; }

    }
}
