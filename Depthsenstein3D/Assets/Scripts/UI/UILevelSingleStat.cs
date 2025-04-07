using System;
using TMPro;
using UnityEngine;

public class UILevelSingleStat : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtName;
    [SerializeField]
    private TextMeshProUGUI txtValue;
    public void Initialize(SingleLevelStat stat, bool isTime = false)
    {
        txtName.text = stat.Name;

        if (isTime)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(stat.Value);

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            txtValue.text = $"{elapsedTime}";
        }
        else
        {
            txtValue.text = $"{stat.Value} / {stat.Max}";
        }
    }
}
