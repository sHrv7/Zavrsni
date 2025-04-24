using UnityEngine;

public class Car : MonoBehaviour
{
    public float score;

    [Header("Brain settings")]
    public float maxEvolveValue;
    public int[] neuronsPerRow = { 2, 5, 7, 2 };
    public Network brain;

    [Header("Sensors")]
    public int numberOfRays;
    public float rayLength;
    private Ray2D[] rays;
    public LayerMask visibleLayers;


    [Header("Score")]
    public float wastedTimeMultiplier;
    public float movementEncourangementMultiplier;
    public int goalScore;
    public int crashPunishment;

    public Rigidbody2D rb;
    private CarController car;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        car = GetComponent<CarController>();
        rays = new Ray2D[numberOfRays];
    }

    public void Init()
    {
        brain = new Network(neuronsPerRow, maxEvolveValue);
    }

    void Update()
    {
        CalculateRays();

        FillInputs();

        brain.Step();

        GetOutputs();
    }

    void FixedUpdate()
    {
        score += car.verticalInput * Time.deltaTime * movementEncourangementMultiplier;
        //score -= Time.deltaTime * wastedTimeMultiplier;
    }


    void CalculateRays()
    {
        float angle = 180f / numberOfRays;
        for (int i = 0; i < numberOfRays; i++)
        {
            rays[i] = new Ray2D(transform.position, Quaternion.Euler(0, 0, angle * (i + 0.5f)) * transform.right);
        }
    }

    void FillInputs()
    {
        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rays[i].origin, rays[i].direction, rayLength, visibleLayers);
            if (hit.transform == null)
            {
                brain.SetInput(i, -1);
                brain.SetInput(rays.Length + i, -1);
                Debug.DrawRay(rays[i].origin, rays[i].direction * rayLength, Color.gray);
            }
            else
            {
                brain.SetInput(i, hit.distance / rayLength);
                if (hit.transform.CompareTag("Goal"))
                {
                    brain.SetInput(rays.Length + i, 1);
                    Debug.DrawLine(rays[i].origin, hit.point, Color.green);
                }
                else
                {
                    brain.SetInput(rays.Length + i, 0);
                    Debug.DrawLine(rays[i].origin, hit.point, Color.red);
                }

            }
        }
    }


    void GetOutputs()
    {
        car.verticalInput = brain.GetOutput(0);
        car.horizontalInput = brain.GetOutput(1);
        car.stopInput = brain.GetOutput(2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        score -= crashPunishment;
    }
}
