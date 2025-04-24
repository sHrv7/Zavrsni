using System.Collections.Generic;
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
    private float prevGenerationAverageScore = 0;
    private float prevGenerationHighScore = 0;

    public Text generationText;
    public Text prevGenerationAverageScoreText;
    public Text prevGenerationHighScoreText;
    public Slider timeScaleSlider;
    public Text timeScaleText;

    void Start()
    {
        for (int i = 0; i < carCount + amountOfNewCarsAdded; i++)
        {
            Car c = Instantiate(car, transform.position, Quaternion.identity, transform).GetComponent<Car>();
            c.Init();
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

            int count = cars.Count;
            prevGenerationAverageScore = 0;
            prevGenerationHighScore = 0;

            for (int i = cars.Count - 1; i >= 0; i--)
            {
                prevGenerationAverageScore += cars[i].score;

                if (cars[i].score > prevGenerationHighScore)
                {
                    prevGenerationHighScore = cars[i].score;
                }

                if (i < numberOfParents)
                {
                    if (cars[i].score <= 0)
                    {
                        Destroy(cars[i].gameObject);
                        cars.RemoveAt(i);

                        for (int j = 0; j < numberOfChildren; j++)
                        {
                            Car c = Instantiate(car, transform.position, Quaternion.identity, transform).GetComponent<Car>();
                            c.Init();
                            cars.Add(c);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < numberOfChildren; j++)
                        {
                            Car c = Instantiate(car, transform.position, Quaternion.identity, transform).GetComponent<Car>();
                            c.brain = new Network(cars[i].brain);

                            c.brain.Evolve(chanceToMutate);

                            cars.Add(c);
                        }
                        Destroy(cars[i].gameObject);
                        cars.RemoveAt(i);
                    }
                }
                else
                {
                    Destroy(cars[i].gameObject);
                    cars.RemoveAt(i);
                }
            }

            for (int i = 0; i < amountOfNewCarsAdded; i++)
            {
                Car c = Instantiate(car, transform.position, Quaternion.identity, transform).GetComponent<Car>();
                c.Init();
                cars.Add(c);
            }

            prevGenerationAverageScore /= count;

            prevGenerationAverageScoreText.text = "Previous generation average score: " + prevGenerationAverageScore;
            prevGenerationHighScoreText.text = "Previous generation high score: " + prevGenerationHighScore;

        }
    }

    public void ChangeTimeScale()
    {
        Time.timeScale = timeScaleSlider.value;
        Time.fixedDeltaTime = 0.02f / Time.timeScale;

        timeScaleText.text = "Speed multiplier: " + timeScaleSlider.value;
    }
}
