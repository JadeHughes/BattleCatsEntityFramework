using System.Text.Json.Serialization;

namespace BattleCatsEntityFramework.Models
{
    public class UserCard
    {
        public int UserId { get; set; }
   
        public User User { get; set; }

        public int CardId { get; set; }
        public BattleCatsCard Card { get; set; }
    }
}
