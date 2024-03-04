using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

public class PlayerScoring : MonoBehaviour
{
    // to be updated later when actual scoring is done in the gameplay loop
    public TMP_Text namesText;
    public TMP_Text scoresText;
    public TMP_Text turnText;
    public Player[] playerList;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTurnText(Player CurrentPlayer)
    {
        string playerName = CurrentPlayer.Name;
        turnText.text = playerName + "'s turn";
    }
    
    public void UpdateScoreboardText()
    {
        namesText.text = "";
        scoresText.text = "";
        
        foreach (Player player in playerList)
        {
            string playerName = player.Name;
            int playerScore = player.Score;

            // Add to scoreboard
            namesText.text += playerName + "\n";
            scoresText.text += playerScore.ToString() + "\n";
        }
    }

    public void endRoundScores(){
        // TODO use this to display the scores at the end of a round
        // sort players by score that round
        Player[] bestScoreRound = playerList.OrderBy(c => c.Score).ToArray();
        // sort players by total score
        Player[] bestScoreTotal = playerList.OrderBy(c => c.TotalScore).ToArray();
    }
}
