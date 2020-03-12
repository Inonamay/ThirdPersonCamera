using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] float cameraSensitivity = 10f;
    [SerializeField] float maxRotateDegrees = 70;
    [SerializeField] float minRotateDegrees = -45;
    [SerializeField] float zoominAmount = 0.5f;
    [SerializeField] float cameraSpeed = 10;
    [SerializeField] float minDistanceToPlayerAllowed = 2;
    [SerializeField] LayerMask BlockingCameraLayers;
    public bool invertedControls;
    public bool cameraSmoothing = true;
    float cameraOffset;
    float cameraRotationLeftRight;
    float cameraRotationUpDown;
    float previousFrameCameraRotationUpDown;
    [Header("Camera and winch")]
    [SerializeField]
    GameObject playerCamera;
    [SerializeField]
    Transform cameraPivotPoint;
    bool zoomedIn = false;
    int invertedDirection = 1;
    const string MouseAxisX = "Mouse X";
    const string MouseAxisY = "Mouse Y";
    Vector3 originalPosition;
    public bool InvertControls
    {
        get{
            return invertedControls;
        }
        set
        {
            if (value)
            {
                invertedDirection = -1;
            }
            else
            {
                invertedDirection = 1;
            }
            invertedControls = value;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        //Finds pivotpoint and camera
        if (!ValidatePivotPoint())
        {
            print("Error, no component named CameraPivot found!");
            Destroy(this);
        }
        if (!ValidatePlayerCamera())
        {
            Debug.Log("Error, no camera found");
            Destroy(this);
        }
        InvertControls = invertedControls;
        originalPosition = playerCamera.transform.localPosition;
        cameraOffset = -playerCamera.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(MouseAxisX) != 0 || Input.GetAxis(MouseAxisY) != 0)
        { RotateCamera(); }
        MoveCamera();
        //I had already bound my scroll wheel to something else in this project so i bound the zoom to this instead
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!zoomedIn)
            {
                originalPosition.z -= zoominAmount;
                zoomedIn = true;
            }
            else
            {
                originalPosition.z += zoominAmount;
                zoomedIn = false;
            }
        }
    }
    void RotateCamera()
    {
        cameraRotationLeftRight += cameraSensitivity * Input.GetAxis(MouseAxisX) * Time.deltaTime * invertedDirection;
        cameraRotationUpDown -= cameraSensitivity * Input.GetAxis(MouseAxisY) * Time.deltaTime * invertedDirection;
        cameraRotationUpDown = Mathf.Clamp(cameraRotationUpDown, minRotateDegrees, maxRotateDegrees);

        //sets the rotation of the player and the pivotpoint
        if (cameraSmoothing)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, cameraRotationLeftRight, 0), Time.deltaTime * cameraSpeed);
            cameraPivotPoint.localRotation = Quaternion.Lerp(cameraPivotPoint.localRotation, Quaternion.Euler(cameraRotationUpDown, 0, 0), Time.deltaTime * cameraSpeed);

        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, cameraRotationLeftRight, 0);
            cameraPivotPoint.localRotation = Quaternion.Euler(cameraRotationUpDown, 0, 0);

        }

    }
    void MoveCamera()
    {
        RaycastHit hit = new RaycastHit();
        float Distance;
        //Checks if there is and obstacle behind the player
        if (RaycastFromPlayer(cameraPivotPoint.transform.forward * -1, ref hit))
        {
            if(hit.transform.gameObject != gameObject)
            {
                Distance = Vector3.Distance(playerCamera.transform.localPosition, hit.point);
                float distanceToPlayer = Vector3.Distance(hit.point, transform.position);
                if(distanceToPlayer > minDistanceToPlayerAllowed)
                {
                    //If there is space for the camera it moves to that spot note that changing the *5 to anything else 
                    //makes the camera freak out and go flying off in directions it shouldn't
                    playerCamera.transform.position =
                     Vector3.Lerp(playerCamera.transform.localPosition, hit.point +
                     (transform.position - playerCamera.transform.position).normalized * 0.75f, Time.deltaTime * Distance * 5);
                }
                else
                {
                    //This entire block is for the edge case of standing at a corner and there not being enough space for the camera to fit
                    //Instead it test to see if it can move to the players shoulder it throws out 4 raycasts to the left and right to see if there is available space
                    Vector3 point = originalPosition + playerCamera.transform.forward;
                    if (RaycastFromPlayer(cameraPivotPoint.transform.forward * -1 + cameraPivotPoint.transform.right * 0.5f, ref hit) ||
                        RaycastFromPlayer(cameraPivotPoint.transform.forward * -1 + cameraPivotPoint.transform.right * 0.75f, ref hit))
                    {
                        if (!RaycastFromPlayer(cameraPivotPoint.transform.forward * -1 + cameraPivotPoint.transform.right * -0.5f, ref hit) ||
                           !RaycastFromPlayer(cameraPivotPoint.transform.forward * -1 + cameraPivotPoint.transform.right * -0.75f, ref hit))
                        {
                            Distance = Vector3.Distance(playerCamera.transform.localPosition + cameraPivotPoint.transform.right, point);

                            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, (point + cameraPivotPoint.transform.right).normalized,
                            Time.deltaTime * Distance * 2);
                        }
                    }
                    else
                    {
                        Distance = Vector3.Distance(playerCamera.transform.localPosition, point + cameraPivotPoint.transform.right * -1);
                        playerCamera.transform.localPosition =
                        Vector3.Lerp(playerCamera.transform.localPosition, (point + cameraPivotPoint.transform.right * -1).normalized,
                        Time.deltaTime * Distance * 2);
                    }
                }

            }
        }
        else
        {
            //Moves the camera back to its original position
            Distance = Vector3.Distance(playerCamera.transform.localPosition, originalPosition);
            playerCamera.transform.localPosition =
            Vector3.Lerp(playerCamera.transform.localPosition, originalPosition, Time.deltaTime * Distance * 5);
            
        }
    }
    bool RaycastFromPlayer(Vector3 Direction, ref RaycastHit Hit)
    {
        Debug.DrawRay(transform.position, Direction);
        return Physics.Raycast(transform.position, Direction, out Hit, cameraOffset + 2, BlockingCameraLayers);
    }
    bool ValidatePivotPoint()
    {
        if (!cameraPivotPoint)
        {
            cameraPivotPoint = transform.Find("CameraPivot");
            if (!cameraPivotPoint)
            {
                
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
    bool ValidatePlayerCamera()
    {
        if (!playerCamera)
        {
            playerCamera = cameraPivotPoint.transform.Find("PlayerCamera").gameObject;
            if (playerCamera == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
}
