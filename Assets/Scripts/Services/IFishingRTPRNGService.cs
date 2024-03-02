public interface IFishingRTPRNGService 
{
    int SuccessRate { get; set; }
    public bool TryFishCollecting(int currentFishRareness);
}