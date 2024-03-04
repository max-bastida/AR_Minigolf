using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GolfPhysics
{
  public class GolfBallController : MonoBehaviour
  {
    [SerializeField] private InputAction drag, screenPos;
    [SerializeField] private new Camera camera;
    [Range(0.5f, 100f)] [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject worldSpaceCanvas;
    [SerializeField] private GameObject powerMeter;
    [SerializeField] private float maxShotPower = 10;
    [SerializeField] private float xMultiplier = 0.9f;
    [SerializeField] private float zMultiplier = 1.1f;
    [Range(0.05f, 2f)] [SerializeField] private float stopVelocityThreshold = 0.7f;

    private Rigidbody rb;
    private Vector3 currentScreenPos;
    private Image powerMeterImage;
    private bool isDragging;
    private Vector3 lastShotPosition;
    private float resetThreshold = -10f;
    private Color color;
    private GameController gameController;
    private Player player;

    public void Initialize(float newResetThreshold, Color ballColor, Camera arCamera, Player player)
    {
      resetThreshold = newResetThreshold;
      color = ballColor;
      camera = arCamera;
      this.player = player;
    }

    private bool IsClicked
    {
      get
      {
        var ray = camera.ScreenPointToRay(currentScreenPos);
        return Physics.Raycast(ray, out var raycastHit) && raycastHit.transform == transform;
      }
    }

    private Vector3 WorldPos
    {
      get
      {
        var z = camera.WorldToScreenPoint(transform.position).z;
        return camera.ScreenToWorldPoint(currentScreenPos + new Vector3(0, 0, z));
      }
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Hole"){
        player.completeCourse();
        gameController.playerScore(player.ID);
      }
    }

    private void Start()
    {
      rb = GetComponent<Rigidbody>();
      powerMeterImage = powerMeter.GetComponent<Image>();
      powerMeterImage.fillAmount = 0;
      GetComponent<MeshRenderer>().material.color = color;
    }

    private void Awake()
    {
      lastShotPosition = transform.position;
      gameController = GameObject.Find("Gameplay Controller").gameObject.GetComponent<GameController>();
      screenPos.performed += context => { currentScreenPos = context.ReadValue<Vector2>(); };
      drag.performed += _ =>
      {
        if (IsClicked && rb.velocity.magnitude < stopVelocityThreshold)
        {
          StartCoroutine(Drag());
        }
      };
      drag.canceled += _ => { isDragging = false; };
    }

    public void EnableInteraction() {
      drag.Enable();
      screenPos.Enable();
    }

    private IEnumerator Drag()
    {
      isDragging = true;

      while (isDragging)
      {
        var vector = CalculateVector();
        powerMeterImage.fillAmount = vector.magnitude / maxShotPower;
        worldSpaceCanvas.transform.rotation =
          Quaternion.Euler(-90, 0, Mathf.Atan2(-vector.z, vector.x) * Mathf.Rad2Deg);
        yield return null;
      }

      yield return new WaitForFixedUpdate();
      powerMeterImage.fillAmount = 0;
      rb.AddForce(CalculateVector() * speed);
      drag.Disable();
      screenPos.Disable();
      yield return new WaitUntil(() => rb.velocity.magnitude < stopVelocityThreshold);
      lastShotPosition = transform.position;
      gameController.EndTurn();
    }

    private Vector3 CalculateVector()
    {
      var startVector = transform.position;
      var originalWorldPos = WorldPos;
      var endVector =
        -(new Vector3(originalWorldPos.x, 0, originalWorldPos.z) -
          startVector);
      endVector = Vector3.Scale(endVector, new Vector3(xMultiplier, 0, zMultiplier));
      return Vector3.ClampMagnitude(endVector, maxShotPower);
    }


    public void ResetBall()
    {
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      transform.position = lastShotPosition;
      player.Score += 1;
      PlayerScoring scoreboard = GameObject.Find("Scoreboard Panel").gameObject.GetComponent<PlayerScoring>();
      scoreboard.UpdateScoreboardText();
    }

    private void FixedUpdate()
    {
      if (transform.position.y <= resetThreshold)
      {
        ResetBall();
      }
    }
  }
}