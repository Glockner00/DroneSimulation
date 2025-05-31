using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour

{

    public float speed = 5f;
    private Vector3 velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Drone initialized!");
    }
    

    // Update is called once per frame

    void Update()
    {
        // Enkel simulerad riktning (t.ex. diagonalt framåt)
        Vector3 direction = new Vector3(1, 0, 1).normalized;
        Vector3 desiredVelocity = direction * speed;

        // Fysikinspirerad styrning (acceleration mot desired)
        Vector3 steering = desiredVelocity - velocity;
        velocity += steering * Time.deltaTime;

        // Flytta objektet
        transform.position += velocity * Time.deltaTime;

        // Rikta drönaren i rörelseriktningen
        if (velocity.magnitude > 0.1f)
            transform.forward = velocity.normalized;
    }
}
