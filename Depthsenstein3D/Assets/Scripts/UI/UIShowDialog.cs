using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIShowDialog : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Animator winscreenAnimator;

    [SerializeField]
    private TextMeshProUGUI txtMessage;
    [SerializeField]
    private TextMeshProUGUI txtButton;

    private bool isShown = false;
    private bool isHiding = false;
    private bool isWin = false;

    private UnityAction showCallback;
    private UnityAction hideCallback;

    [SerializeField]
    private GameObject deathScreen;
    [SerializeField]
    private GameObject paper;
    [SerializeField]
    private GameObject winScreen;
    [SerializeField]
    private GameObject containerContent;

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

    public void ShowWin() {
        isWin = true;
        animator.enabled = false;
        deathScreen.SetActive(false);
        winScreen.SetActive(true);
        containerContent.SetActive(true);
        paper.SetActive(false);
        winscreenAnimator.Play("winScreenShow");
    }

    public void ShowDeath(UnityAction hideCallback) {
        if (isShown)
        {
            return;
        }
        deathScreen.SetActive(true);
        txtMessage.text = "You took too many hits and died.";
        txtButton.text = "RESTART LEVEL (SPACE)";
        this.showCallback = delegate {

        };
        this.hideCallback = hideCallback;
        animator.Play("uiShowDialogShow");
    }

    public void Show(string message, UnityAction showCallback, UnityAction hideCallback)
    {
        if (isShown) {
            return;
        }
        deathScreen.SetActive(false);
        txtMessage.text = message;
        txtButton.text = "OK (SPACE)";
        this.showCallback = showCallback;
        this.hideCallback = hideCallback;
        animator.Play("uiShowDialogShow");
    }

    public void ShowFinished() {
        isShown = true;
        showCallback();
    }

    void Update() {
        if (isWin) {
            return;
        }
        if (!isHiding && isShown && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))) {
            Hide();
        }
    }
}
