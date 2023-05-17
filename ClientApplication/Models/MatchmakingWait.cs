namespace ClientApplication.Models
{
    public class MatchmakingWait
    {
        public int UserId { get; set; }
        
        public bool MatchFound { get; set; }

        public int MatchId { get; set; }
    }
}
