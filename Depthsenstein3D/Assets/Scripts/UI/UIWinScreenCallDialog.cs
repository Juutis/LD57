using UnityEngine;

public class UIWinScreenCallDialog : MonoBehaviour
{
    [SerializeField]
    private UIShowDialog dialog;
    public void CallDialog() {
        dialog.ShowWinFinished();
    }
}
