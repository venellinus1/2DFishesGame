using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    void Start()
    {
        ConnectToPhoton();
    }

    void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {        
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 5,
            IsVisible = true,
            IsOpen = true
        };
        string roomName = "MyfishingRoom";

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        Vector2 randomPosition = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        GameObject controller = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

        //sample Object graph building (though incomplete as it doesnt handle the dependencies of the dependencies... )
        //abstracting the dependencies for the Player Controller allows avoiding tight coupling
        PlayerControllerV1 playerController = controller.GetComponent<PlayerControllerV1>();
        IPlayerLineDrawing lineDrawing = controller.GetComponent<IPlayerLineDrawing>();
        IPlayerUI playerUI = controller.GetComponent<IPlayerUI>();
        IFishingRTPRNGService fishingRTPRNGService = controller.GetComponent<IFishingRTPRNGService>();

        PlayerUIModel playerUIModel = new PlayerUIModel();

        playerUI.SetDependencies(playerUIModel);
        playerController.SetDependencies(lineDrawing, fishingRTPRNGService, playerUIModel);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("OnJoinRoomFailed: " + message);        
    }
}
