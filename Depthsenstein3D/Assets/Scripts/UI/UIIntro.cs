using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIIntro : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI txtMessage;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    [TextArea]
    private List<string> firstMessages = new List<string>();
    [SerializeField]
    [TextArea]
    private List<string> secondMessages = new List<string>();

    private List<string> messagesFirst;
    private List<string> messagesSecond;

    private UIIntroState state = UIIntroState.Start;
    private UIIntroFadeState fadeState = UIIntroFadeState.FadingIn;

    private float messageTimer = 0f;
    private float messageInterval = 2f;
    private float intervalTimer = 0f;
    private float messageFadeDuration = 1f;

    [SerializeField]
    private Color clearColor;
    private Color visibleColor = Color.white;

    private Color originColor;
    private Color targetColor = Color.white;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        messagesFirst = new(firstMessages);
        messagesSecond = new(secondMessages);
        messageTimer = messageFadeDuration;
    }

    private void DisableUnderlay() {
        //Disable
        //txtMessage.fontSharedMaterial.DisableKeyword("UNDERLAY_ON");
    }

    private void EnableUnderlay() {
        //Enable
       //txtMessage.fontSharedMaterial.EnableKeyword("UNDERLAY_ON");
    }

    void NextMessage() {
        originColor = clearColor;
        targetColor = visibleColor;
        fadeState = UIIntroFadeState.FadingIn;
        txtMessage.color = originColor;
        DisableUnderlay();
        if (state == UIIntroState.FirstScreen)
        {
            if (messagesFirst.Count > 0)
            {
                txtMessage.text = messagesFirst[0];
                messagesFirst.RemoveAt(0);
            } else {
                state = UIIntroState.Switching;
                animator.Play("uiIntroSwitch");
                Debug.Log("Switch");
            }
        } else if (state == UIIntroState.SecondScreen) {
            if (messagesSecond.Count > 0) {
                txtMessage.text = messagesSecond[0];
                messagesSecond.RemoveAt(0);
            } else {
                state = UIIntroState.Finished;
                animator.Play("uiIntroFinish");
            }
        }
    }

    public void SwitchFinished() {
        state = UIIntroState.SecondScreen;
        NextMessage();
    }

    void Update()
    {
        if (state == UIIntroState.Switching) {
            return;
        }
        if (state == UIIntroState.Start) {
            state = UIIntroState.FirstScreen;
            NextMessage();
        }
        if (state == UIIntroState.Finished) {
            return;
        }
        if (fadeState == UIIntroFadeState.WaitingForInput) {
            if (Input.anyKeyDown) {
                NextMessage();
            }
            return;
        }
        if (Input.anyKeyDown) {
            messageTimer = messageFadeDuration;
        }
        if (fadeState == UIIntroFadeState.Stalled) {
            if (Input.anyKeyDown) {
                intervalTimer += 0.5f;
            }
            intervalTimer += Time.unscaledDeltaTime;
            if (intervalTimer >= messageInterval) {
                fadeState = UIIntroFadeState.WaitingForInput;
                intervalTimer = 0f;
            }
        }
        if (fadeState == UIIntroFadeState.FadingIn) {
            messageTimer += Time.unscaledDeltaTime;
            txtMessage.color = Color.Lerp(originColor, targetColor, messageTimer / messageFadeDuration);
            if (messageTimer >= messageFadeDuration) {
                messageTimer = 0f;
                txtMessage.color = targetColor;
                EnableUnderlay();
                fadeState = UIIntroFadeState.Stalled;
                /*if (fadeState == UIIntroFadeState.FadingIn) {
                    fadeState = UIIntroFadeState.FadingOut;
                } else {
                    NextMessage();
                }*/
            }
        }
    }

    public void IntroFinished() {
        SceneManager.LoadScene(1);
    }
}

public enum UIIntroFadeState
{
    FadingIn,
    Stalled,
    WaitingForInput
}

public enum UIIntroState {
    Start,
    FirstScreen,
    Switching,
    SecondScreen,
    Finished
}