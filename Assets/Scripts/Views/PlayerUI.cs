using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(PlayerControllerV1))]
public class PlayerUI : MonoBehaviourPun, IPlayerUI
{
    public TextMeshProUGUI fishCountInfo;
    public TextMeshProUGUI fishAttemptsInfo;
    private PlayerUIModel playerUIModel;
    public void UpdateFishCountUI(int fishCount, int attemptsCount)
    {
        fishCountInfo.text = "Fish: " + fishCount;
        fishAttemptsInfo.text = "Attempts: " + attemptsCount;
    }

    public void SetDependencies(PlayerUIModel playerUIModel)
    {
        this.playerUIModel = playerUIModel;
        if (playerUIModel != null)
        {
            playerUIModel.OnDataChanged += ApplyUIChanges;
        }
    }
    [PunRPC]
    public void UpdatePlayerStatsRPC(int fishCount, int attemptsCount)
    {
        UpdateFishCountUI(fishCount, attemptsCount);
    }
    public void UpdatePlayerStats(int fishCount, int attemptsCount)
    {
        photonView.RPC("UpdatePlayerStatsRPC", RpcTarget.All, fishCount, attemptsCount);
    }
    
    private void ApplyUIChanges()
    {
        UpdatePlayerStats(playerUIModel.CollectedFish, playerUIModel.AttemptsCount);
    }
    

    private void OnDisable()
    {        
        if (playerUIModel != null)
        {
            playerUIModel.OnDataChanged -= ApplyUIChanges;
        }
    }
}
