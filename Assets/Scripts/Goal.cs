using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    List<Car> visited = new List<Car>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Car"))
        {
            Car c = collision.GetComponent<Car>();

            if (!visited.Contains(c))
            {
                visited.Add(c);
                collision.GetComponent<Car>().score += c.goalScore;
            }
        }
    }
}
