namespace MAWcore6.Models
{
    public class CityModels
    {
        public class UpdateCityModel
        {
            public int CityId { get; set; }
            public int BuildingId { get; set; }
            public string? BuildingTypeString { get; set; }
            public int BuildingTypeInt { get; set; }
            public int Level { get; set; }
            public int Location { get; set; } = -1;
        }
        public class Resources
        {
            public int Food { get; set; } = 0;
            public int Stone { get; set; } = 0;
            public int Wood { get; set; } = 0;
            public int Iron { get; set; } = 0;
            public int Gold { get; set; } = 0;
        }
        public class HireHeroModel
        {
            public int CityId { get; set; }
            public int HeroId { get; set; }
        }
        public class Result
        {
            public bool Failed { get; set; }
            public string Message { get; set; }
        }
        public class SpeedUpModel
        {
            public int CityId { get; set; }
            public bool SpeedUp5min { get; set; }
            public string UsedOn { get; set; }
        }
    }
}
