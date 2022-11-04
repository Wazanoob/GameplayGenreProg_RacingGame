using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;

    private bool m_isBreaking;

    [Header("Wheels control")]
    [SerializeField] private float m_motorForce;
    [SerializeField] private float m_breakForce;
    private float m_currentBreakForce;

    [SerializeField] private WheelCollider m_frontLeftWheelCollider;
    [SerializeField] private WheelCollider m_frontRightWheelCollider;
    [SerializeField] private WheelCollider m_rearLeftWheelCollider;
    [SerializeField] private WheelCollider m_rearRightWheelCollider;

    [SerializeField] private Transform m_frontLeftWheelTransform;
    [SerializeField] private Transform m_frontRightWheelTransform;
    [SerializeField] private Transform m_rearLeftWheelTransform;
    [SerializeField] private Transform m_rearRightWheelTransform;

    [Header("Steering")]
    [SerializeField] private float m_maxSteerAngle;

    [Header("Taillight")]
    [SerializeField] private Animator m_taillight;


    private float m_currentSteerAngle;
    public float CurrentSteerAngle { get { return m_currentSteerAngle; } set { CurrentSteerAngle = m_currentSteerAngle; } }


    [SerializeField] private float m_downPressure;

    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += Vector3.down;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //TurboTroll
            float turboBoost = 10000;
            m_motorForce = 9000 * turboBoost;

            WheelFrictionCurve turboCurve = new WheelFrictionCurve();
            turboCurve.extremumSlip = 2 * turboBoost;
            turboCurve.extremumValue = 5 * turboBoost;
            turboCurve.asymptoteSlip = 5 * turboBoost;
            turboCurve.asymptoteValue = 2 * turboBoost;
            turboCurve.stiffness = 1;


            m_frontLeftWheelCollider.forwardFriction = turboCurve;
            m_frontRightWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;
        }
        else
        {
            m_motorForce = 9000;

            //TurboTroll
            WheelFrictionCurve turboCurve = new WheelFrictionCurve();
            turboCurve.extremumSlip = 2;
            turboCurve.extremumValue = 5;
            turboCurve.asymptoteSlip = 5;
            turboCurve.asymptoteValue = 2;
            turboCurve.stiffness = 1;

            m_frontLeftWheelCollider.forwardFriction = turboCurve;
            m_frontRightWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;
            m_rearLeftWheelCollider.forwardFriction = turboCurve;
        }

        m_isBreaking = Input.GetKey(KeyCode.Space);
    }

    void HandleMotor()
    {
        //Apply force just to the front wheels
        m_frontLeftWheelCollider.motorTorque = m_verticalInput * m_motorForce;
        m_frontRightWheelCollider.motorTorque = m_verticalInput * m_motorForce;

        if (m_isBreaking)
        {
            m_taillight.SetBool("isBreaking", true);
            m_currentBreakForce = m_breakForce;
        }else if(m_verticalInput != 0)
        {
            m_taillight.SetBool("isBreaking", false);
            m_currentBreakForce = 0.0f;
        }else if (m_verticalInput == 0)
        {
            m_taillight.SetBool("isBreaking", false);
            m_currentBreakForce = m_breakForce / 10;
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
