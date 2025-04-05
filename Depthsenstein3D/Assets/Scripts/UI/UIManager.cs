using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{

    public static UIManager main;
    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private UIShowDialog uiShowDialogPrefab;
    [SerializeField]
    private Transform uiShowDialogContainer;

    public void ShowMessage(string message, UnityAction showCallback, UnityAction hideCallback) {
        UIShowDialog uiShowDialog = Instantiate(uiShowDialogPrefab, uiShowDialogContainer);
        uiShowDialog.Show(message, showCallback, hideCallback);
    }

    public void ShowMessage(string message)
    {
        Time.timeScale = 0f;
        UIShowDialog uiShowDialog = Instantiate(uiShowDialogPrefab, uiShowDialogContainer);
        uiShowDialog.Show(message, delegate {}, delegate {
            Time.timeScale = 1f;
        });
    }
}
