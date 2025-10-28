using System.Collections.Generic;

namespace AIInstructor.src.Gamification.Settings
{
    public class GamificationSettings
    {
        public int PointPerSuccess { get; set; } = 10;
        public Dictionary<string, int> BadgeThresholds { get; set; } = new();
        public string DefaultBadge { get; set; } = "Bronz";
    }
}
