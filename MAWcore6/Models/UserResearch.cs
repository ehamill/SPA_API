namespace MAWcore6.Models
{
    public class UserResearch
    {
        public int UserResearchId { get; set; }
        public string UserId { get; set; }
        public int Agg { get; set; } = 0;
        public int Lumbering { get; set; } = 0;
        public int Mining { get; set; } = 0;
        public int Masonry { get; set; } = 0;
        public int MetalCasting { get; set; } = 0;
        public int IronWorking { get; set; } = 0;
        public int TroopSpeed { get; set; } = 0;
        public int Archery { get; set; } = 0;
        public int Attack { get; set; } = 0;
        public int HorsebackRiding { get; set; } = 0;
        public int Defense { get; set; } = 0;
    }
}
