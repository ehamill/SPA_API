namespace MAWcore6.Models
{
    public class City
    {
        public int CityId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ServerId { get; set; }
        public string Image { get; set; }

        public List<Building> Buildings { get; set; }

    }
}
