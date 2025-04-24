using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float acceleration = 30f;
    public float maxSpeed = 50f;
    public float steering = 5f;
    public float decelerationForce = 10f;
    public float brakingForce = 35f;

    private float slowdown;
    private float currentSpeed;

    public bool isPlayerControlled = false;

    public float verticalInput;
    public float horizontalInput;
    public float stopInput;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isPlayerControlled)
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");

            // Space bar for braking
            if (Input.GetKey(KeyCode.Space))
            {
                stopInput = Mathf.Lerp(stopInput, 1, Time.deltaTime * 5f); // Smoothing
            }
            else
            {
                stopInput = Mathf.Lerp(stopInput, 0, Time.deltaTime * 5f); // Smoothing
            }
        }

        slowdown = Mathf.Lerp(decelerationForce, brakingForce, stopInput);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        MoveCar();
        Decelerate();
    }

    void HandleMovement()
    {
        if (Mathf.Abs(currentSpeed) < maxSpeed)
        {
            currentSpeed += verticalInput * acceleration * Time.fixedDeltaTime;
        }
    }

    void HandleSteering()
    {
        float steeringAmount = horizontalInput * steering * currentSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, 0, -steeringAmount);
    }

    void Decelerate()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= slowdown * Time.fixedDeltaTime;
        }
        else if (currentSpeed < 0)
        {
            currentSpeed += slowdown * Time.fixedDeltaTime;
        }
    }

    void MoveCar()
    {
        rb.linearVelocity = currentSpeed * transform.up;

        // Stop the car when velocity is very low
        if (rb.linearVelocity.magnitude <= 0.1f)
        {
            currentSpeed = 0;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
