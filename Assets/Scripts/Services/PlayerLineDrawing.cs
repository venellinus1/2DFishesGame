
using Photon.Pun;
using UnityEngine;

namespace FishingGame.Services
{


    [RequireComponent(typeof(LineRenderer))]//, typeof(EdgeCollider2D)
    public class PlayerLineDrawing : MonoBehaviourPun, IPunObservable, IPlayerLineDrawing
    {
        public LineRenderer lineRenderer;
        public BoxCollider2D fishDetectionCollider;//assign in inspector
        public Transform fishHookArea;//assign in inspector

        //this could be a Scriptable object settings applied to each iplayerdrawing
        //Range is another approach for limiting the values compared to FishingRTPRNGService successRate
        [Range(4, 8)]
        [Tooltip("The lenght of the fishing line")]
        public float maxLength = 5.0f;

        private Vector2 startPosition;
        private Vector2 endPosition;
        private bool isDrawing = false;

        void Awake()
        {
            SetFishDetection(false);
            lineRenderer = GetComponent<LineRenderer>();           
        }
        
        void Update()
        {
            if (photonView.IsMine)
            {
                HandleDrawingInput();
            }
        }
        public Vector2 GetEndPosition()
        {
            return endPosition;
        }
        private void HandleDrawingInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = transform.position;//line starts from the player's position
                isDrawing = true;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPosition);
                UpdateLineRendererAndCollider(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0) && isDrawing)
            {
                //while drawing update the line end position
                UpdateLineRendererAndCollider(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0) && isDrawing)
            {
                //stop drawing when mouse button is released
                isDrawing = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                //if the right mouse button is clicked, hide the line
                HideLine();
            }
        }
        public void HideLine()
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
                isDrawing = false;
                SetFishDetection(false);
            }
        }
        private void UpdateLineRendererAndCollider(Vector3 screenPosition)
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 direction = (worldPosition - startPosition).normalized;
            float distance = Vector2.Distance(worldPosition, startPosition);
            endPosition = distance > maxLength ? startPosition + direction * maxLength : worldPosition;

            // Update LineRenderer
            if (lineRenderer.positionCount > 1)
                lineRenderer.SetPosition(1, endPosition);

            UpdateFishDetectionCollider(endPosition);

        }
        private void UpdateFishDetectionCollider(Vector2 lineEndPoint)
        {
            //enable the collider and position it at the end of the line
            SetFishDetection(true);
            fishDetectionCollider.transform.position = lineEndPoint;
        }

        private void SetFishDetection(bool state)
        {
            fishDetectionCollider.enabled = state;
            fishHookArea.gameObject.SetActive(state);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(isDrawing);
                stream.SendNext(startPosition);
                stream.SendNext(endPosition);
                stream.SendNext(lineRenderer.positionCount);
            }
            else
            {
                isDrawing = (bool)stream.ReceiveNext();
                startPosition = (Vector2)stream.ReceiveNext();
                endPosition = (Vector2)stream.ReceiveNext();
                lineRenderer.positionCount = (int)stream.ReceiveNext();
                //update the line renderer based on the received data
                if (isDrawing)
                {
                    /////lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, startPosition);
                    lineRenderer.SetPosition(1, endPosition);
                }
            }
        }
    }
}