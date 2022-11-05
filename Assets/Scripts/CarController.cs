using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    //Inputs
    private float m_horizontalInput;
    private float m_verticalInput;

    public Rigidbody rb;
    public bool gamedStarted = false;

    [Header("Audio Source")]
    private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_nitroClip;
    [SerializeField] private AudioClip m_stopNitroClip;

    [Header("Wheels control")]
    [SerializeField] private float m_downPressure;
    [SerializeField] private float m_motorForce;
    [SerializeField] private float m_brakeForce;
    private float m_currentBreakForce;

    [SerializeField] private WheelCollider m_frontLeftWheelCollider;
    [SerializeField] private WheelCollider m_frontRightWheelCollider;
    [SerializeField] private WheelCollider m_rearLeftWheelCollider;
    [SerializeField] private WheelCollider m_rearRightWheelCollider;

    [SerializeField] private Transform m_frontLeftWheelTransform;
    [SerializeField] private Transform m_frontRightWheelTransform;
    [SerializeField] private Transform m_rearLeftWheelTransform;
    [SerializeField] private Transform m_rearRightWheelTransform;

    private bool m_isBraking;

    [Header("Steering")]
    [SerializeField] private float m_maxSteerAngle;

    [Header("Taillight")]
    [SerializeField] private Animator m_taillight;


    private float m_currentSteerAngle;
    public float CurrentSteerAngle { get { return m_currentSteerAngle; } set { CurrentSteerAngle = m_currentSteerAngle; } }

    [Header("Nitro")]
    [SerializeField] private GameObject m_VFXNitro;
    [SerializeField] private GameObject m_VFXSpeed;
    [SerializeField] private float m_consumeNitroSpeed = 0.2f;
    [SerializeField] private float m_refillNitroSpeed = 0.025f;
    private const float COOLDOWNNITRO = 3.0f;
    private float m_cooldown;
    [HideInInspector] public float nitroAmount = 1.0f;
    private bool m_onlyOnceStop = true;
    private bool m_onlyOnceNitro = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += Vector3.down;

        m_audioSource = GetComponent<AudioSource>();

        m_VFXNitro.SetActive(false);
        m_VFXSpeed.SetActive(false);
        m_cooldown = COOLDOWNNITRO;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamedStarted)
        {
            GetInput();
        }
        HandleMotor();
        HandleSteering();
        UpdateWheel();
    }

    private void FixedUpdate()
    {
        Vector3 forceToAdd = new Vector3(0, -rb.velocity.magnitude, 0);
        rb.AddForce(forceToAdd * m_downPressure * Time.fixedDeltaTime);
    }

    void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && nitroAmount > 0)
        {
            if (!m_VFXNitro.activeInHierarchy)
            {
                m_VFXNitro.SetActive(true);
                m_VFXSpeed.SetActive(true);
            }

            if (!m_onlyOnceNitro)
            {
                m_audioSource.Stop();
                m_audioSource.clip = m_nitroClip;
                m_audioSource.Play();

                m_onlyOnceStop = false;
                m_onlyOnceNitro = true;
            }

            //TurboTroll
            WheelFrictionCurve turboCurve = new WheelFrictionCurve();
            turboCurve.extremumSlip = 2;
            turboCurve.extremumValue = 4;
            turboCurve.asymptoteSlip = 4;
            turboCurve.asymptoteValue = 2;
            turboCurve.stiffness = 1;

            m_frontLeftWheelCollider.forwardFriction = turboCurve;
            m_frontRightWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;

            nitroAmount -= m_consumeNitroSpeed * Time.deltaTime;
            nitroAmount = Mathf.Clamp(nitroAmount, 0, 1);
        }
        else
        {
            if (m_VFXNitro.activeInHierarchy)
            {
                m_cooldown = 0;
                m_VFXNitro.SetActive(false);
                m_VFXSpeed.SetActive(false);
            }

            m_cooldown += Time.deltaTime;

            if (m_cooldown >= COOLDOWNNITRO)
            {
                nitroAmount += m_refillNitroSpeed * Time.deltaTime;
                nitroAmount = Mathf.Clamp(nitroAmount, 0, 1);
            }


            if (m_audioSource.isPlaying && !m_onlyOnceStop)
            {
                m_audioSource.Stop();
                m_audioSource.clip = m_stopNitroClip;
                m_audioSource.Play();

                m_onlyOnceNitro = false;
                m_onlyOnceStop = true;
            }

            WheelFrictionCurve turboCurve = new WheelFrictionCurve();
            turboCurve.extremumSlip = 0.6f;
            turboCurve.extremumValue = 1;
            turboCurve.asymptoteSlip = 1.2f;
            turboCurve.asymptoteValue = 0.75f;
            turboCurve.stiffness = 1;

            m_frontLeftWheelCollider.forwardFriction = turboCurve;
            m_frontRightWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;
        }

        m_isBraking = Input.GetKey(KeyCode.Space);
    }

    void HandleMotor()
    {
        //Apply force just to the front wheels
        m_frontLeftWheelCollider.motorTorque = m_verticalInput * m_motorForce;
        m_frontRightWheelCollider.motorTorque = m_verticalInput * m_motorForce;

        if (m_isBraking)
        {
            m_taillight.SetBool("isBreaking", true);
            m_currentBreakForce = m_brakeForce;
        }else if(m_verticalInput != 0)
        {
            m_taillight.SetBool("isBreaking", false);
            m_currentBreakForce = 0.0f;
        }else if (m_verticalInput == 0)
        {
            m_taillight.SetBool("isBreaking", false);
            m_currentBreakForce = m_brakeForce / 10;
        }


        ApplyBreaking(m_currentBreakForce);
    }

    void HandleSteering()
    {
        //Apply rotations (steering angle)
        m_currentSteerAngle = m_maxSteerAngle * m_horizontalInput;
        m_frontLeftWheelCollider.steerAngle = m_currentSteerAngle;
        m_frontRightWheelCollider.steerAngle = m_currentSteerAngle;
    }

    void UpdateWheel()
    {
        //Anim the wheels
        UpdateSingleWheel(m_frontLeftWheelCollider, m_frontLeftWheelTransform);
        UpdateSingleWheel(m_frontRightWheelCollider, m_frontRightWheelTransform);
        UpdateSingleWheel(m_rearLeftWheelCollider, m_rearLeftWheelTransform);
        UpdateSingleWheel(m_rearRightWheelCollider, m_rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;

        //Get the transform of the wheelCollider
        wheelCollider.GetWorldPose(out position, out rotation);

        //Apply it to the wheelTransform
        wheelTransform.rotation = rotation;
        wheelTransform.position = position;
    }

    void ApplyBreaking(float brakeForceP = 0)
    {
        m_frontLeftWheelCollider.brakeTorque = brakeForceP;
        m_frontRightWheelCollider.brakeTorque = brakeForceP;
        m_rearLeftWheelCollider.brakeTorque = brakeForceP;
        m_rearRightWheelCollider.brakeTorque = brakeForceP;
    }
}
