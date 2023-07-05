using MAWcore6.Data;

namespace MAWcore6.Models
{
    public class Element
    {
        public int Id { get; set; } 
        public int CoordX { get; set; } = 0;
        public int CoordY { get; set; } = 0;    
        public bool Occupied { get; set; }  = false;
        public int CityId { get; set; } = 0;
        public TerrainType TerrainType { get; set; } = TerrainType.Flat;
        public DateTime? LastHit { get; set; }
        public int Workers { get; set; } = 0;
        public int Warriors { get; set; } = 0;
        public int Scout { get; set; } = 0;
        public int Pikemen { get; set; } = 0;   
        public int Swordsman { get; set; } = 0;
        public int Archer { get; set; } = 0;
        public int Cavalry { get; set; } = 0;
        public int Cataphract { get; set; } = 0;
        public int Transporter { get; set; } = 0;
        public int Ballista { get; set; } = 0;
        public int Battering_Ram { get; set; } = 0;
        public int Catapult { get; set; } = 0;
        public int Trap { get; set; } = 0;
        public int Abatis { get; set; } = 0;
        public int Archers_Tower { get; set; } = 0;
        public int Rolling_Log { get; set; } = 0;
        public int Defensive_Trebuchet { get; set; } = 0;
    }
}
