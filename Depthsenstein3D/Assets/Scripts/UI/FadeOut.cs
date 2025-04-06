using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField]
    private Image image;

    private bool isFading = false;

    private float startTime = 0;
    private float fadeTime = 0.5f;

    private Color black;
    private Color transparent;

    private Color start;
    private Color end;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        black = Color.black;
        transparent = new Color(black.r, black.g, black.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            float t = (Time.time - startTime) / fadeTime;
            image.color = Color.Lerp(start, end, t);

            if (t >= 1)
            {
                isFading = false;
                image.color = end;
            }
        }
    }

    public void FadeOut()
    {
        startTime = Time.time;
        start = transparent;
        end = black;
        isFading = true;
    }

    public void FadeIn()
    {
        startTime = Time.time;
        start = black;
        end = transparent;
        isFading = true;
    }
}
