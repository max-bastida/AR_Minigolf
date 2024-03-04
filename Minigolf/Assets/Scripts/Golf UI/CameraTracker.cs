using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
  [SerializeField] private new Transform camera;

  public void Initialize(Transform arCamera)
  {
    camera = arCamera;
  }

  private void Update()
  {
    transform.LookAt(camera);
  }
}