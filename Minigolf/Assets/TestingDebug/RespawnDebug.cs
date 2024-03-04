using GolfPhysics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetBall : MonoBehaviour
{
  [SerializeField] private InputAction resetAction;

  private void Awake()
  {
    resetAction.Enable();
    resetAction.performed += _ => GetComponent<GolfBallController>().ResetBall();
  }
}