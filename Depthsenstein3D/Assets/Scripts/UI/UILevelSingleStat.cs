using TMPro;
using UnityEngine;

public class UILevelSingleStat : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtName;
    [SerializeField]
    private TextMeshProUGUI txtValue;
    public void Initialize(SingleLevelStat stat) {
        txtName.text = stat.Name;
        txtValue.text = $"{stat.Value} / {stat.Max}";
    }
}
