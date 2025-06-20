namespace BattleCatsEntityFramework.Models
{
    public class BattleCatsCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ChonkRating { get; set; }
        public string SpecialAttack { get; set; }
        public int SpecialAttackPower { get; set; }
        public int Fluffiness { get; set; }
        public int Attitude { get; set; }

        public List<UserCard> UserCards { get; set; }
    }
}
