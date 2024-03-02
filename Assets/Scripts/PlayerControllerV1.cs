using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using FishingGame.Services;
using System;
[RequireComponent(typeof(LineRenderer))]
public class PlayerControllerV1 : MonoBehaviourPunCallbacks, IPunObservable, IPlayerControllerV1
{    
    public IPlayerLineDrawing playerLineDrawing;    
    private IFishingRTPRNGService fishingRTPRNGService;
    private FishMovement hookedFish = null;//todo turn it abstraction/interface

    private PlayerUIModel playerUIModel;
    public void SetDependencies(IPlayerLineDrawing lineDrawing, IFishingRTPRNGService rtpRNGService, PlayerUIModel playerUIModel)
    {
        this.playerUIModel = playerUIModel;
        playerLineDrawing = lineDrawing;       
        fishingRTPRNGService = rtpRNGService;        
    }
    [Range(3, 8)]
    [Tooltip("The movement speed of the player")]
    public float moveSpeed = 5.0f;
    public LineRenderer lineRenderer;//reference required for networking   

    // Variables to hold synchronized position and line end position
    private Vector3 networkPosition;
    private Vector3 lineEndPosition;//used for networking update
    

    private bool isHooking = false;    
    private List<GameObject> fishesInRange = new List<GameObject>();

    public delegate void AttemptAddedHandler(int fishRareness, string text);
    public static event AttemptAddedHandler OnAttemptAdded;

    private void Update()
    {
        if (photonView.IsMine)
        {
            MovePlayer();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isHooking)
        {
            TryHookFish();
        }

        if (isHooking)
        {
            ReelInHookedFish();
        }
    }

    void MovePlayer()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(h, v, 0) * moveSpeed * Time.deltaTime);

        if (h != 0 || v != 0)
        {
            playerLineDrawing.HideLine();
        }
    }
       
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(lineEndPosition); 
        }
        else
        {
            //network player, receive data
            networkPosition = (Vector3)stream.ReceiveNext();
            lineEndPosition = (Vector3)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            fishesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            fishesInRange.Remove(other.gameObject);
        }
    }
    void TryHookFish()
    {
        foreach (var fishCollider in fishesInRange)
        {            
            FishMovement fish = fishCollider.GetComponent<FishMovement>();            
            if (fish != null )
            {
                playerUIModel.AttemptsCount++;
                //if (Random.value > chanceToHookFish) // 50% chance to hook
                //if (fisingRTPService.TryFishCollecting()) // using Return to player service - Task 2

                //using return to player with custom RNG/game volatility based on fish rareness
                if (fishingRTPRNGService.TryFishCollecting(fish.fishRareness))
                {
                    OnAttemptAdded?.Invoke(fish.fishRareness, "Hooked"); 
                    hookedFish = fish;
                    isHooking = true;
                    fish.HookFish(); 
                    break;
                }
                else
                {
                    OnAttemptAdded?.Invoke(fish.fishRareness, "Missed"); 
                }
            }
        }
    }

    void ReelInHookedFish()
    {
        playerLineDrawing?.HideLine();
        isHooking = false;
        playerUIModel.CollectedFish++;

        if (PhotonNetwork.IsMasterClient)
        {
            //masterClient can disable directly
            hookedFish?.GetComponent<PhotonView>()?.RPC("DisableFish", RpcTarget.All);
        }
        else
        {
            //Non-MasterClient sends a request to the MasterClient
            hookedFish?.GetComponent<PhotonView>()?.RPC("RequestDisableFish", RpcTarget.MasterClient);
        }
        hookedFish = null;
    }
}