using System;
public interface IPlayerControllerV1 
{
    public void SetDependencies(IPlayerLineDrawing lineDrawing, IFishingRTPRNGService rtpRNGService, PlayerUIModel playerUIModel);
}
