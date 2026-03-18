using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButtonFloat : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;

    [SerializeField] private GameManager gameManager;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * 0.9f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale;

        gameManager.StartGame();
    }
}