namespace MAWcore6.Models
{
    public class UserItems
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NickName { get; set; }
        public int Cents { get; set; }
        public int FiveMinuteSpeedups { get; set; }
        public int FifteenMinuteSpeedup { get; set; }
        public int OneHourSpeedup { get; set; }
        public int RedGems { get; set; }
        public int BlueGems { get; set; }
        public int FarmBonuses { get; set; }
    }
}
