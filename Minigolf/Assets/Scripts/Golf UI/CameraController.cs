using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Transform targetTransform;
    private float yDistance;
    private float zDistance;

    private void Start()
    {
        targetTransform = target.transform;
        var currentPos = transform.position;
        yDistance = currentPos.y - targetTransform.position.y;
        zDistance = currentPos.z - targetTransform.position.z;
    }

    private void LateUpdate()
    {
        // TODO: Camera will be tied to the phone screen
        var currentTargetPos = targetTransform.position;

        transform.position = new Vector3(
            currentTargetPos.x,
            currentTargetPos.y + yDistance,
            currentTargetPos.z + zDistance
        );
    }
}