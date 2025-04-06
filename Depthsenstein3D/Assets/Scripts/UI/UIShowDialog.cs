using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIShowDialog : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI txtMessage;

    private bool isShown = false;
    private bool isHiding = false;

    private UnityAction showCallback;
    private UnityAction hideCallback;

    public void Hide()
    {
        if (!isShown || isHiding) {
            return;
        }
        isHiding = true;
        animator.Play("uiShowDialogHide");
    }

    public void HideFinished()
    {
        isShown = false;
        isHiding = false;
        hideCallback();
        Destroy(gameObject);
    }

    public void Show(string message, UnityAction showCallback, UnityAction hideCallback)
    {
        if (isShown) {
            return;
        }
        txtMessage.text = message;
        this.showCallback = showCallback;
        this.hideCallback = hideCallback;
        animator.Play("uiShowDialogShow");
    }

    public void ShowFinished() {
        isShown = true;
        showCallback();
    }

    void Update() {
        if (!isHiding && isShown && Input.GetKeyDown(KeyCode.Space)) {
            Hide();
        }
    }
}
