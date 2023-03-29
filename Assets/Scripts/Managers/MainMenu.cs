using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public ImageArray back, title1, title2;

    private void Start()
    {
        back.SetRandom(); 
        if (Random.value < 0.2f)
        {
            title1.SetRandom();
            title2.SetRandom();
        }
        GameManager.paused = false;
    }

    public void PlaySound(AudioClip clip)
    {
        AudioManager.Play(new AudioA(clip));
    }

    public void Quit() => Application.Quit();
}

[System.Serializable]
public class ImageArray
{
    public Image image;
    public Sprite[] sprites;

    public void SetRandom()
    {
        image.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}