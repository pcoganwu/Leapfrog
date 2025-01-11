namespace Leapfrog.Core.Entities
{
    public class FlashPatternItem
    {
        public int Mode { get; set; }
        public string Duration { get; set; }
        public int Ticks { get; set; }

        public FlashPatternItem(int mode, string duration)
        {
            Mode = mode;
            Duration = duration;

            if (int.TryParse(duration, out int parsedDuration))
            {
                Ticks = parsedDuration / 50;
            }
            else
            {
                Ticks = 0; // or handle the error as needed
            }
        }
    }
}
