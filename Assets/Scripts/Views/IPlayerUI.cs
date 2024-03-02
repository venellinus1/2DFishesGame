public interface IPlayerUI 
{    
    public void UpdatePlayerStats(int collectedFish, int attemptsCount);
    public void SetDependencies(PlayerUIModel playerUIModel);
}
