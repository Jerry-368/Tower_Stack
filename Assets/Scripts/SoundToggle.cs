using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    public Image icon;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool muted;

    void Awake()
    {
        muted = PlayerPrefs.GetInt("MUTED", 0) == 1;
        ApplyState();
    }

    public void ToggleSound()
    {
        muted = !muted;

        PlayerPrefs.SetInt("MUTED", muted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyState();
    }

    void ApplyState()
    {
        AudioListener.volume = muted ? 0f : 1f;

        if (icon != null)
        {
            icon.sprite = muted ? soundOffSprite : soundOnSprite;
        }
    }
}