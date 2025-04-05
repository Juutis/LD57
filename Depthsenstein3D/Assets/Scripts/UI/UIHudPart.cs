using TMPro;
using UnityEngine;

public class UIHudPart : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtTitle;
    [SerializeField]
    private TextMeshProUGUI txtValue;

    public void Initialize(string title, string value)
    {
        txtTitle.text = title;
        txtValue.text = value;
    }

    public void SetValue(string value) {
        txtValue.text = value;
    }
}
