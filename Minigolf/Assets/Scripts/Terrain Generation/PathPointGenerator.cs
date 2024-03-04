using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject[] allCards;

    GameObject[] objectsList;
    List<Vector3> pointsList;
    //public TerrainGeneration terrainGenerator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToList()
    {
        objectsList = GameObject.FindGameObjectsWithTag("Point");
        pointsList = new List<Vector3>(objectsList.Length);
        bool start = false;
        bool end = false;
        bool doubledUp = false;
        int number = 0;
        foreach (GameObject card in allCards)
        {
            if (objectsList.Contains(card))
            {
                pointsList.Add(card.transform.position);
                if (card.name == "Start")
                {
                    start = true;
                    Debug.Log("Start found");
                }
                else if (card.name == "Hole")
                {
                    end = true;
                    Debug.Log("End found");
                }
                else if (card.GetComponent<Numbering>())
                {
                    number = card.GetComponent<Numbering>().number;
                    foreach (GameObject gameObject in objectsList)
                    {
                        if (gameObject.name.EndsWith(number.ToString()) && (gameObject.name != card.name))
                        {
                            doubledUp = true;
                            Debug.Log("Doubled up: " + number.ToString());
                            break;
                        }
                    }
                }
            }
        }
        
        if (start && end && !doubledUp)
        {
            // Pass list to terrain generator
            PlayerGenerator playerGenerator = GameObject.Find("Start Menu Canvas").gameObject.GetComponent<PlayerGenerator>();
            playerGenerator.ActivateGameplayPanel();
            GameObject controller = GameObject.Find("Gameplay Controller").gameObject;
            TerrainGeneration terrainGenerator = controller.GetComponent<TerrainGeneration>();
            terrainGenerator.createCourse(pointsList);
            GameController gameController = controller.GetComponent<GameController>();
            gameController.StartPlayLoop();
            // hide all cube markers
            foreach (GameObject card in objectsList)
            {
                card.transform.Find("Cube").gameObject.SetActive(false);
            }
        }
        else
        {
            if (!start)
            {
                
                Debug.Log("No start card found");
            }
            if (!end)
            {
                Debug.Log("No hole card found");
            }

            // Display UI asking to include the missing tile(s) or remove excess tile(s)
            GameObject canvas = GameObject.Find("Error Canvas").gameObject;
            GameObject panel = canvas.transform.Find("Missing Panel").gameObject;
            panel.SetActive(true);

            // Reset vuforia tracking
            //DeInitialiseVuforia();
            //Button button = panel.gameObject.transform.Find("Next").GetComponent<Button>();
            //button.onClick.AddListener(InitialiseVuforia);

            // Don't pass list to terrain generator

        }

    }

}
