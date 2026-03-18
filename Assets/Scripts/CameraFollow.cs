using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offsetY = 6f;
    [SerializeField] private float smoothSpeed = 3f;

    private float highestPoint;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        highestPoint = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetHeight = target.position.y + offsetY;

        if (targetHeight > highestPoint)
        {
            highestPoint = targetHeight;
        }

        Vector3 newPosition = new Vector3(
            transform.position.x,
            Mathf.Lerp(transform.position.y, highestPoint, smoothSpeed * Time.deltaTime),
            transform.position.z
        );

        transform.position = newPosition;
    }

    public void ResetCamera()
    {
        transform.position = startPos;
        highestPoint = startPos.y;
    }
}