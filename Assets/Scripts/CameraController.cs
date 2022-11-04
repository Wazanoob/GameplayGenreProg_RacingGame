using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //References
    Transform m_player;
    Camera m_camera;
    CarController m_carController;
    Transform m_tilt;

    //Camera Variables
    [Header("Camera")]
    [SerializeField] float cameraHeight;
    [SerializeField] float cameraMaxTilt;
    [Range(0, 4)]
    [SerializeField] float cameraSpeed;
    const float DISTANCE = 6;
    float currentPan, currentTilt = 5;
    const float tilt = 5;

    //Collisions
    [Header("Collisions")]
    [SerializeField] LayerMask collisionMask;
    public bool collisionDebugg;
    float collisionCushion = 0.35f;
    float adjustedDistance;
    Ray camRay;
    RaycastHit camRayHit;

    //SmoothLerp
    public float smoothLerp = 50f;
    public float smoothLerpRotation = 25f;

    private void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_carController = m_player.GetComponent<CarController>();

        m_tilt = transform.Find("Tilt").transform;
        m_camera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;

        transform.position = m_player.position + Vector3.up * cameraHeight;

        m_tilt.eulerAngles = new Vector3(currentTilt, transform.eulerAngles.y, transform.eulerAngles.z);
        m_camera.transform.position += m_tilt.forward * -DISTANCE;
    }

    private void Update()
    {
        CameraCollisions();
    }

    void LateUpdate()
    {
        //Follow player
        Vector3 nextPosition = m_player.transform.position + Vector3.up * cameraHeight;
        transform.position = nextPosition;
        transform.position = Vector3.Lerp(transform.position, nextPosition, Vector3.SqrMagnitude(transform.position - nextPosition) * smoothLerp * Time.deltaTime); //Lerp

        //Rotate around Player
        currentPan = m_player.transform.eulerAngles.y + m_carController.CurrentSteerAngle / 2;
        currentTilt = tilt + m_carController.CurrentSteerAngle / 3;

        Vector3 nextRotation = new Vector3(transform.eulerAngles.x, currentPan, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(nextRotation), Vector3.SqrMagnitude(transform.position - nextRotation) * (smoothLerpRotation) * Time.deltaTime);
        
        m_tilt.eulerAngles = new Vector3(tilt, m_tilt.eulerAngles.y, -currentTilt);

        m_camera.transform.position = transform.position + m_tilt.forward * -adjustedDistance;
    }

    void CameraCollisions()
    {
        float camDistance = DISTANCE + collisionCushion;  //To avoid clips & smooth the camera collisions

        camRay.origin = transform.position;
        camRay.direction = -m_tilt.forward;               //Shoot at the opposit direction

        if (Physics.Raycast(camRay, out camRayHit, camDistance, collisionMask))
        {
            adjustedDistance = Vector3.Distance(camRay.origin, camRayHit.point) - collisionCushion;
        }
        else
        {
            adjustedDistance = DISTANCE;
        }

        if (collisionDebugg)
        {
            Debug.DrawLine(camRay.origin, camRay.origin + camRay.direction * camDistance, Color.cyan);
        }
    }
}
