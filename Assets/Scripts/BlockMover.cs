using UnityEngine;

public class BlockMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveLimit = 2.8f;

    private Transform blockTransform;
    private float moveDirection = 1f;
    private bool isMoving = true;

    void Awake()
    {
        blockTransform = transform;
    }

    void Update()
    {
        if (!isMoving) return;

        MoveBlock();
        CheckBounds();
    }

    void MoveBlock()
    {
        float speed = moveSpeed * Time.deltaTime;

        blockTransform.Translate(Vector3.right * moveDirection * speed);
    }

    void CheckBounds()
    {
        if (blockTransform.position.x > moveLimit)
        {
            moveDirection = -1f;
        }

        if (blockTransform.position.x < -moveLimit)
        {
            moveDirection = 1f;
        }
    }

    public void StopBlock()
    {
        isMoving = false;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}