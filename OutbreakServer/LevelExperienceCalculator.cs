namespace Outbreak.Server
{
    public class LevelExperienceCalculator
    {
        public int GetLevelExperienceRequirement(int level)
        {
            return (level * 750) + 500;
        }
    }
}