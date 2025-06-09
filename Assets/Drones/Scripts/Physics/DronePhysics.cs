using UnityEngine;
/// DronePhysics.cs
/// ----------------------------------------
/// This class implements the core physics model for a single drone.
/// Current features:
/// - Translational motion (gravity, drag, thrust)
/// - Velocity and position integration
///
/// TODOs:
/// - Implement rotational physics: angular velocity + torques
/// - Implement rotation integration (update quaternion)
/// - Add controller layer to map user commands (target velocity / altitude) → to throttle
/// - Eventually support full flight control: yaw, pitch, roll control
/// - Support multiple drones (already compatible with swarm design)
///
/// Notes:
/// - Manual throttle control via W/S is currently hard to fly
/// - Real drones use a controller (e.g. PID) to map desired altitude/velocity to throttle
/// - Physics model is kept theoretically correct → controller will be added on top
/// ----------------------------------------
public class DronePhysics : MonoBehaviour
{
    [Header("Physics Settings")]
    public float mass = 1.0f; // 1Kg
    public float thrustCoefficient = 1.0f; // (thrust coefficient) T_i = k_T * (w_i)^2 : N/(rad/s)^2 
    public float dragCoefficient = 0.1f; // D = -k_D * v : N*s/m The faster the drone moves the more drag

    /* NOTE
    T = k_T * w^2 = 9.81 -> thrust must be 9.81 N to hover
    => k_T = 1 then w = 3.13 rad/s → slowly sinks due to drag
    */
    [Header("Rotor Settings")]
    public float maxRotorSpeed = 10.0f; // rad/s → max rotor speed used when throttle = 1.0

    [Header("Throttle (0 = off, 1 = full)")]
    [Range(0f, 1f)]
    public float throttle = 0.313f; // ≈ hover throttle for maxRotorSpeed = 10

    // -- State Variables
    private Vector3 velocity = Vector3.zero; // (x,y,z) m/s
    private Vector3 angularVelocity = Vector3.zero; // (x,y,z) roll, pitch, yaw (TODO: implement rotational dynamics)
    private Quaternion rotation; // Unity's preferred way to represent rotation (TODO: implement own rotation integration)
    private Vector3 position;

    void Start()
    {
        rotation = transform.rotation; // copy Unity's current rotation into internal variable
        position = transform.position;

        // Ensure the drone is not moving or rotating on start
        velocity = Vector3.zero;
        angularVelocity = Vector3.zero;
    }


    void FixedUpdate()
    {
        // --- Thrust force ---
        velocity = Vector3.zero;
        angularVelocity = Vector3.zero;
        // Compute rotor speed from throttle
        float rotorSpeed = throttle * maxRotorSpeed;

        // Compute thrust from rotor speed
        float thrust = thrustCoefficient * rotorSpeed * rotorSpeed;

        // Thrust always points up in the drone's local Z axis → convert to world space
        Vector3 thrustForce = transform.up * thrust;

        // --- Gravity and drag ---
        Vector3 gravity = new Vector3(0, -9.81f, 0);    // Gravity force
        Vector3 drag = -dragCoefficient * velocity;     // Drag force

        // --- Total acceleration ---
        Vector3 acceleration = (gravity * mass + thrustForce + drag) / mass;

        // --- Integrate translational motion ---
        velocity += acceleration * Time.fixedDeltaTime; // Integrate velocity
        position += velocity * Time.fixedDeltaTime;     // Integrate position

        // --- Update Transform ---
        transform.position = position;
        transform.rotation = rotation;
    }
}