using System.Collections;
using System.Collections.Generic;
using GolfPhysics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBall : MonoBehaviour
{
  [SerializeField] private InputAction spawnAction;
  [SerializeField] private Camera arCamera;
  [SerializeField] private GameObject golfBallPrefab;

  private void Awake()
  {
    spawnAction.Enable();
    spawnAction.performed += _ =>
    {
      var golfBall = Instantiate(golfBallPrefab, Vector3.zero, Quaternion.identity);
      golfBall.GetComponent<InitializeGolfBall>().Initialize(-2, "Test Name", arCamera, new Player(0, new GameObject()));
    };
  }
}