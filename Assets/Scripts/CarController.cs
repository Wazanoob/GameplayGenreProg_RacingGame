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

    [Header("Steering")]
    [SerializeField] private float m_maxSteerAngle;
    private float m_steerAngle;
    private float m_currentSteerAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheel();
    }

    void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");

        m_isBreaking = Input.GetKey(KeyCode.Space);
    }

    void HandleMotor()
    {
        //Apply force just to the front wheels
        m_frontLeftWheelCollider.motorTorque = m_verticalInput * m_motorForce;
        m_frontRightWheelCollider.motorTorque = m_verticalInput * m_motorForce;

        if (m_isBreaking)
        {
            m_currentBreakForce = m_breakForce;
            ApplyBreaking();
        }else
        {
            m_currentBreakForce = 0.0f;
        }
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

    }

    void ApplyBreaking()
    {
        m_frontLeftWheelCollider.brakeTorque = m_currentBreakForce;
        m_frontRightWheelCollider.brakeTorque = m_currentBreakForce;
        m_rearLeftWheelCollider.brakeTorque = m_currentBreakForce;
        m_rearRightWheelCollider.brakeTorque = m_currentBreakForce;
    }
}