using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

[RequireComponent(typeof(Collider2D))]
public class FishMovement : MonoBehaviourPun, IPunObservable
{
    [Range(1, 5)]
    [Tooltip("The movement speed of the fish")]
    public float speed = 2.0f;//set in inspector
    [Tooltip("The pond bounds limiting the fish movement")]
    public Collider2D pondBounds;//set in inspector
    [Range(1, 5)]
    [Tooltip("How rare is the fish - lower values = low rareness")]
    public int fishRareness;//set in inspector 

    private Vector2 targetPosition;//network syncd
    private bool isActive = true;//network syncd
    private bool isHooked = false;//network syncd

    private Bounds hookBounds;
    private Transform hook;
    private Bounds targetBounds;//meant to hold the bounds to limit the fish movement - can be changed at runtime eg with bounds of the hook to make fish going round a hook...

   

    private void Start()
    {
        if (photonView.IsMine)
        {            
            targetBounds = pondBounds.bounds;
            SetNewRandomPositionAroundBounds(targetBounds);
        }
    }
  
    private void Update()
    {
        if (photonView.IsMine)
        {
            
            MoveToPosition();
            if ((Vector2)transform.position == targetPosition)
            {
                SetNewRandomPositionAroundBounds(targetBounds);
            }
        }
    }

    [PunRPC]
    public void DisableFish()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
    void MoveToPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    void SetNewRandomPositionAroundBounds(Bounds targetBounds)
    {
        //only the owner sets new positions to prevent conflicts
        if (!photonView.IsMine) return;

        Vector2 potentialPosition;
        do
        {
            float xPosition = Random.Range(targetBounds.min.x, targetBounds.max.x);
            float yPosition = Random.Range(targetBounds.min.y, targetBounds.max.y);
            potentialPosition = new Vector2(xPosition, yPosition);
        }
        while (!pondBounds.OverlapPoint(potentialPosition));

        targetPosition = potentialPosition;
    }    

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //send the active state and the target position of the fish
            stream.SendNext(gameObject.activeSelf);
            stream.SendNext(targetPosition);
            
        }
        else
        {
            //receive the active state and the target position of the fish
            bool receivedIsActive = (bool)stream.ReceiveNext();
            targetPosition = (Vector2)stream.ReceiveNext();
            
            if (!isActive)
            {
                gameObject.SetActive(false);
            }
        }
    }
    [PunRPC]
    public void RequestDisableFish()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.photonView.RPC("DisableFish", RpcTarget.AllBuffered);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FishingLine"))
        {
            hook = other.gameObject.transform;
        }
    }

    
    public void HookFish()
    {
        Debug.Log("HookedFish!");
        isHooked = true;
    }
}
