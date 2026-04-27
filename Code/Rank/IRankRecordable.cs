namespace HN.Code.Rank
{
    public interface IRankRecordable
    {
        public int CurrentCorner { get; set; }
        public int CurrentLap { get; set; }

        public void CompleteLap(float completeTime);
        public void Goal();
    }
}