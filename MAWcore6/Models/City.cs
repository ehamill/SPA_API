namespace MAWcore6.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string UserId { get; set; }
        public int ServerId { get; set; }
        public string Image { get; set; }
        public int Food { get; set; }
        public int Stone { get; set; }
        public int Wood { get; set; }
        public int Iron { get; set; }
        public int Gold { get; set; }
        public DateTime ResourcesLastUpdated { get; set; }
        public List<Building> Buildings { get; set; }
        //public UserResearch UserResearch { get; set; }
        public DateTime? Construction1Started { get; set; }
        public DateTime? Construction1Ends { get; set; }
        public int Construction1BuildingId { get; set; } = 0;
        public bool Builder1Busy { get; set; } = false;
        public int Builder1Time { get; set; } = 0;


    }
}
