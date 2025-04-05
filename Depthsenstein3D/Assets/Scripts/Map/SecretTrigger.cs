using UnityEngine;

public class SecretTrigger : MonoBehaviour
{

    private int secretId;

    private bool hasTriggered = false;
    public void Initialize(int secretId)
    {
        this.secretId = secretId;
    }

    public void Trigger() {
        if (hasTriggered) {
            return;
        }

        Debug.Log($"Secret id {secretId} triggered!");
        MapGenerator.main.TriggerSecret(secretId);

        hasTriggered = true;
    }

    void Update()
    {
        // uncomment to test
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            Trigger();
        }*/
    }

}
