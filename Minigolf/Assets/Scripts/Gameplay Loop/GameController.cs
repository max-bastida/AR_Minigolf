using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GolfPhysics;
using TMPro;
using System.Linq;
public class GameController : MonoBehaviour
{
    public Player[] Players {get;set;}
    private int currentPlayerIndex;
    public Player CurrentPlayer {get{
        return Players[currentPlayerIndex];
    }}
    public float lowestTerrainPoint {get;set;}
    public Vector3 ballStartPos {get;set;}
    public GameObject ballTemplate;
    public Camera arCamera;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlayLoop(){
        SetupPlayers();
        StartPlayerTurn();
    }
    private void SetupPlayers(){
        foreach (Player player in Players)
        {
            GameObject ball = Instantiate(ballTemplate, ballStartPos, Quaternion.identity);
            player.createBall(lowestTerrainPoint, ball, arCamera);
            player.finishedCourse = false;
            player.Score = 0;
        }
        PlayerScoring scoreboard = GameObject.Find("Scoreboard Panel").gameObject.GetComponent<PlayerScoring>();
        scoreboard.UpdateTurnText(CurrentPlayer);
        scoreboard.UpdateScoreboardText();
        currentPlayerIndex = 0;
    }

    private void StartPlayerTurn(){
        CurrentPlayer.isTurn = true;
        CurrentPlayer.Ball.SetActive(true);
        CurrentPlayer.Ball.transform.Find("Golf Ball").gameObject.GetComponent<GolfBallController>().EnableInteraction();
        PlayerScoring scoreboard = GameObject.Find("Scoreboard Panel").gameObject.GetComponent<PlayerScoring>();
        scoreboard.UpdateTurnText(CurrentPlayer);
    }
    
    public void EndTurn(){
        CurrentPlayer.EndTurn();
        if (Players.All(x => x.finishedCourse)){
            EndRound();
            return;
        }
        currentPlayerIndex += 1;
        if (currentPlayerIndex >= Players.Length) {
            currentPlayerIndex = 0;
        }
        while (CurrentPlayer.finishedCourse){
            currentPlayerIndex += 1;
            if (currentPlayerIndex >= Players.Length) {
                currentPlayerIndex = 0;
            }
        }
        PlayerScoring scoreboard = GameObject.Find("Scoreboard Panel").gameObject.GetComponent<PlayerScoring>();
        scoreboard.UpdateScoreboardText();
        

        StartPlayerTurn();
    }

    public void EndRound() {
        foreach (Player player in Players)
        {
            player.TotalScore += player.Score;
        }
        // TODO display a screen of scores for this round and total scores
        // give players option of starting a new course or finishing
        GameObject course = GameObject.Find("Course");
        DeleteCourse(course);
        SetUpEndUI();
    }

    public void DeleteCourse(GameObject parent)
    {
        // Delete course
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        // Hide flag
        GameObject controller = GameObject.Find("Gameplay Controller").gameObject;
        controller.GetComponent<TerrainGeneration>().flag.SetActive(false);

        // Disable balls
        foreach (Player player in Players)
        {
            player.Ball.SetActive(false);
        }
    }

    public void ShowCubes()
    {
        NewBehaviourScript pointGenerator = transform.GetComponent<NewBehaviourScript>();
        GameObject[] objectsList = pointGenerator.allCards;
        // show all cube markers
        foreach (GameObject card in objectsList)
        {
            card.transform.Find("Cube").gameObject.SetActive(true);
        }
    }

    public void playerScore(int playerID){
        if (playerID == currentPlayerIndex){
            EndTurn();
        }
    }

    public void SetUpEndUI()
    {
        // Activate panel
        GameObject endRoundCanvas = GameObject.Find("End Round Canvas").gameObject;
        GameObject endRoundPanel = endRoundCanvas.transform.Find("End Round Panel").gameObject;
        endRoundPanel.SetActive(true);

        PlayerScoring scoreboard = GameObject.Find("Scoreboard Panel").gameObject.GetComponent<PlayerScoring>();
        Player[] bestScoreRound = scoreboard.playerList.OrderBy(c => c.Score).ToArray();
        Player[] bestScoreTotal = scoreboard.playerList.OrderBy(c => c.TotalScore).ToArray();

        TMP_Text roundText = endRoundPanel.transform.Find("Round Players").gameObject.GetComponent<TMP_Text>();
        TMP_Text roundScoreText = endRoundPanel.transform.Find("Round Scores").gameObject.GetComponent<TMP_Text>();
        TMP_Text totalText = endRoundPanel.transform.Find("Total Players").gameObject.GetComponent<TMP_Text>();
        TMP_Text totalScoreText = endRoundPanel.transform.Find("Total Scores").gameObject.GetComponent<TMP_Text>();

        roundText.text = "";
        totalText.text = "";
        roundScoreText.text = "";
        totalScoreText.text = "";

        for (int i = 0; i < bestScoreRound.Count(); i++)
        {
            roundText.text += bestScoreRound[i].Name + "\n";
            totalText.text += bestScoreTotal[i].Name + "\n";
            roundScoreText.text += bestScoreRound[i].Score + "\n";
            totalScoreText.text += bestScoreTotal[i].TotalScore + "\n";
        }

    }

    public void DeletePlayers()
    {
        GameObject[] playerNamers = GameObject.FindGameObjectsWithTag("Namer");
        foreach (GameObject playerNamer in playerNamers)
        {
            Destroy(playerNamer.gameObject);
        }
    }
}
