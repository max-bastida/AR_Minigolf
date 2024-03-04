using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGenerator : MonoBehaviour
{
    public TMP_InputField NumPlayers;
    public GameObject PlayerNamer;
    public int numPlayersInt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // want it to create a new player object for each player, i.e., user says there are n players, should create n player objects
    // on button clicked, create NumPlayers player objects

    public void CreatePlayers()
    {
        int.TryParse(NumPlayers.text, out numPlayersInt);

        Player[] playerList = new Player[numPlayersInt];
        GameObject controller = GameObject.Find("Gameplay Controller").gameObject;
        GameController gameController = controller.GetComponent<GameController>();

        for (int i = 0; i < numPlayersInt; i++)
        {
            GameObject namer = CreateNamer(i);
            Player player = new Player(i, namer);
            playerList[i] = player;
            player.Name = "Player " + i;

            Button button = namer.transform.Find("Player Naming Panel/InputField (TMP)/Button").GetComponent<Button>();

            if (namer.gameObject.GetComponent<Canvas>())
            {
                Canvas canvas = namer.gameObject.GetComponent<Canvas>();
                canvas.sortingOrder = (numPlayersInt - i);
            }

            button.onClick.AddListener(delegate { NamePlayer(player); });
            if (i == (numPlayersInt - 1))
            {
                button.onClick.AddListener(ActivatePlaceCardsPanel);
                GameObject canvas = GameObject.Find("Gameplay Canvas").gameObject;
                PlayerScoring scorer = canvas.transform.Find("Scoreboard Panel").GetComponent<PlayerScoring>();
                scorer.playerList = playerList;
                button.onClick.AddListener(scorer.UpdateScoreboardText);
            }
        }
        gameController.Players = playerList;
    }

    public void ActivateGameplayPanel()
    {
        GameObject gameplayCanvas = GameObject.Find("Gameplay Canvas").gameObject;
        GameObject gameplayPanel = gameplayCanvas.transform.Find("Scoreboard Panel").gameObject;
        GameObject turnsPanel = gameplayCanvas.transform.Find("Turns Panel").gameObject;
        gameplayPanel.SetActive(true);
        turnsPanel.SetActive(true);
        GameObject instructionsCanvas = GameObject.Find("Instructions Canvas").gameObject;
        GameObject resetButton = instructionsCanvas.transform.Find("Instructions Panel/Reset").gameObject;
        resetButton.SetActive(true);
    }

    public void ActivatePlaceCardsPanel()
    {
        GameObject cardsCanvas = GameObject.Find("Place Cards Canvas").gameObject;
        GameObject cardsPanel = cardsCanvas.transform.Find("Place Cards Panel").gameObject;
        cardsPanel.SetActive(true);
    }

    public GameObject CreateNamer(int i)
    {
        GameObject namer = GameObject.Instantiate(PlayerNamer);
        namer.gameObject.SetActive(true);
        namer.name = "Player Namer " + i;
        namer.transform.position = Vector3.zero;
        return namer;
    }

    public void NamePlayer(Player player)
    {
        GameObject panel = player.namer.transform.Find("Player Naming Panel").gameObject;
        GameObject inputFieldObj = panel.transform.Find("InputField (TMP)").gameObject;
        TMP_InputField inputField = inputFieldObj.GetComponent<TMP_InputField>();
        string name = inputField.text;
        player.Name = name;

    }
}
