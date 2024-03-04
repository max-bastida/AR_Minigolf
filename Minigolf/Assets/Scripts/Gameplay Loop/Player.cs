using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name {get;set;}
    public int ID {get;}
    public GameObject Ball {get;set;}
    public int Score {get;set;} = 0;
    public int TotalScore {get;set;} = 0;
    public GameObject namer {get;}
    public bool finishedCourse = false;
    public bool isTurn = false;

    public Player (int ID, GameObject namer){
        this.ID = ID;
        this.namer = namer;
        //create ball
    }

    public void createBall(float lowestTerrainPoint, GameObject ball, Camera arCamera){
        Ball = ball;
        Ball.GetComponent<InitializeGolfBall>().Initialize(lowestTerrainPoint, Name, arCamera, this);
        Ball.SetActive(false);
    }

    public void EndTurn(){
        if (isTurn)
        {
            Score += 1;
        }
        isTurn = false;
    }
    
    public void completeCourse(){
        Ball.SetActive(false);
        finishedCourse = true;
        isTurn = false;
        //Score += 1;
        if (Score == 1){
            Debug.Log("Hole in one!");
        }
    }

}
