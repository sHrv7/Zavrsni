using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public GameObject car;

    [Header("Initialization settings")]
    public int carCount;
    private List<Car> cars = new List<Car>();


    [Header("Evolution settings")]
    public float chanceToMutate = 1;

    [Range(1, 50)]
    public int numberOfParents;
    private int numberOfChildren;
    public float generationTime;
    public int amountOfNewCarsAdded;

    private float currentTime = 0;
    private int currentGeneration = 0;
    private float prevGenerationHighScore = 0;

    public Text generationText;
    public Text prevGenerationHighScoreText;

    public bool loadFromFile = false;
    public int saveToFileEvery = 10;


    void Start()
    {
        for (int i = 0; i < carCount + amountOfNewCarsAdded; i++)
        {
            Car c = Instantiate(car, transform.position, Quaternion.identity, transform).GetComponent<Car>();
            c.Init();

            if (loadFromFile)
            {
                c.brain.LoadWeights();
            }

            cars.Add(c);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        currentTime += Time.deltaTime;

        if (currentTime >= generationTime)
        {
            numberOfChildren = carCount / numberOfParents;
            currentTime = 0;
            currentGeneration++;
            generationText.text = "Current generation: " + currentGeneration;
            cars.Sort((car1, car2) => car2.score.CompareTo(car1.score));

            if (currentGeneration % saveToFileEvery == 0)
            {
                cars[0].brain.SaveWeights();
            }

            prevGenerationHighScore = 0;
            if (cars.Count > 0)
            {
                prevGenerationHighScore = cars[0].score;
            }

            List<Car> survivors = cars.Take(numberOfParents).ToList();
            cars.ForEach(c => Destroy(c.gameObject));
            cars.Clear();

            foreach (Car parent in survivors)
            {
                for (int j = 0; j < numberOfChildren; j++)
                {
                    Car c = Instantiate(car, transform.position, Quaternion.identity, transform).GetComponent<Car>();

                    if (parent.score <= 0)
                    {
                        // Bad parent: start from scratch
                        c.Init();
                    }
                    else
                    {
                        // Good parent: clone and evolve
                        c.brain = new Network(parent.brain);
                        if (j != 0)
                            c.brain.Evolve(chanceToMutate);
                    }

                    cars.Add(c);
                }
            }

            survivors.ForEach(c => Destroy(c.gameObject));

            // Add new random cars each generation
            for (int i = 0; i < amountOfNewCarsAdded; i++)
            {
                Car c = Instantiate(car, transform.position, Quaternion.identity, transform).GetComponent<Car>();
                c.Init();
                cars.Add(c);
            }


            prevGenerationHighScoreText.text = "Previous generation high score: " + prevGenerationHighScore;

        }
    }

}
