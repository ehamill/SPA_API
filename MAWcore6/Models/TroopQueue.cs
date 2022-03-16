using MAWcore6.Data;

namespace MAWcore6.Models
{
    public class TroopQueue
    {
        public int TroopQueueId { get; set; }
        public int CityId { get; set; } = 0;
        //public virtual City City { get; set; }
        public int BuildingId { get; set; } = 0;
        public TroopType TroopType { get; set; }
        public DateTime Starts { get; set; } = DateTime.UtcNow;
        public DateTime Ends { get; set; } = DateTime.UtcNow.AddMinutes(5);
        public int Qty { get; set; } = 0;
    }
}
