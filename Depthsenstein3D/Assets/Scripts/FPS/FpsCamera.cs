using UnityEngine;

public class FpsCamera : MonoBehaviour
{
    private Camera cam;
    private float refreshRate = 8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
        Invoke("Render", 1.0f / refreshRate);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Render() {
        cam.Render();
        Invoke("Render", 1.0f / refreshRate);
    }
}
