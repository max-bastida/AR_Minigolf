using GolfPhysics;
using TMPro;
using UnityEngine;

public class InitializeGolfBall : MonoBehaviour
{
  [SerializeField] private GameObject golfBall;
  [SerializeField] private GameObject cameraFollowingCanvas;
  [SerializeField] private GameObject nameUi;

  public void Initialize(float resetThreshold, string playerName, Camera arCamera, Player player)
  {
    Color color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
    golfBall.GetComponent<GolfBallController>().Initialize(resetThreshold, color, arCamera, player);
    cameraFollowingCanvas.GetComponent<CameraTracker>()
      .Initialize(arCamera.GetComponent<Transform>());
    var nameText = nameUi.GetComponent<TextMeshProUGUI>();
    nameText.color = color;
    nameText.SetText(playerName);
  }
}