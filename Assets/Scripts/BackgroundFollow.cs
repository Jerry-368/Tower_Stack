using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = cameraTransform.position.y;
        transform.position = pos;
    }
}