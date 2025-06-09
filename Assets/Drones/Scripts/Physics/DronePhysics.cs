using UnityEngine;

public class DronePhysics : MonoBehaviour
{
    public float mass = 1.0f; // 1Kg
    public float thrustCoefficient = 1.0f; // (thrust coefficient) T_i = k_T * (w_i)^2 : N/(rad/s)^2 
    public float dragCoefficient = 0.1f; // D = -k_D * v : N*s/m The faster the drone moves the more drag
    private Vector3 velocity = Vector3.zero; //(x,y,z) m/s
    private Vector3 angularVelocity = Vector3.zero; // (x,y,z) roll, pitch, yaw
    private Quaternion rotation; // Unitys preffered way to represent rotation, TODO: implement own rotation matrix
    private Vector3 position;

    void Start()
    {
        rotation = transform.rotation; //copy unitys current rotation into internal variable
        position = transform.position; 

        // Ensure the drone is not moving or rotatating on start TODO: update manually each frame
        velocity = Vector3.zero;       
        angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        // --- Thrust force ---
        // For now, constant thrust (rotorspeed) TODO: variable thrust

        // T = k_T * w^2 = 9.81 -> thrust must be 9.81 N to hover
        // => k_t = 1 then w = 3.13 rad/s => slowly sinks due to drag 
        float simulatedRotorSpeed = 3.13f;
        float thrust = thrustCoefficient * simulatedRotorSpeed * simulatedRotorSpeed;

        // Thrust always points up in the drone's local Z axis → convert to world space
        Vector3 thrustForce = transform.up * thrust;

        // --- Gravity and drag ---
        Vector3 gravity = new Vector3(0, -9.81f, 0);    // Gravity force
        Vector3 drag = -dragCoefficient * velocity;     // Drag force
   
        // --- Total acceleration ---
        Vector3 acceleration = (gravity * mass + thrustForce + drag) / mass;

        // --- Integrate motion ---
        velocity += acceleration * Time.fixedDeltaTime; // Integrate velocity
        position += velocity * Time.fixedDeltaTime;     // Integrate position

        // --- Update Transform ---
        transform.position = position;
        transform.rotation = rotation;
        }
}
