namespace MAWcore6.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string UserId { get; set; }
        public int ServerId { get; set; }
        public string Image { get; set; }
        public int Food { get; set; } = 5000;
        public int FoodRate { get; set; } = 100;
        public int Stone { get; set; } = 5000;
        public int StoneRate { get; set; } = 100;
        public int Wood { get; set; } = 5000;
        public int WoodRate { get; set; } = 100;
        public int Iron { get; set; } = 5000;
        public int IronRate { get; set; } = 100;
        public int Gold { get; set; } = 5000;
        public int GoldRate { get; set; } = 0;
        public DateTime ResourcesLastUpdated { get; set; }
        public List<Building> Buildings { get; set; }
        //public UserResearch UserResearch { get; set; }
        public DateTime Construction1Started { get; set; } = DateTime.UtcNow;
        public DateTime Construction1Ends { get; set; } = DateTime.UtcNow;
        public int Construction1BuildingId { get; set; } = 0;
        public int Construction1BuildingLevel { get; set; } = 0;
        public bool Builder1Busy { get; set; } = false;
        public int Builder1Time { get; set; } = 0;
        public string? BuildingWhat { get; set; }


    }
}
