using UnityEngine;
using UnityEngine.UI;

public class MuteToggle : MonoBehaviour
{
    public Image iconImage;
    public Sprite soundOn;
    public Sprite soundOff;

    private bool muted = false;

    public void ToggleMute()
    {
        muted = !muted;

        AudioListener.volume = muted ? 0 : 1;

        iconImage.sprite = muted ? soundOff : soundOn;
    }
}