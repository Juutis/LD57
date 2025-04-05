using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoreMessageConfigScriptableObject", menuName = "Scriptable Objects/LoreMessageConfigScriptableObject")]
public class LoreMessageConfigScriptableObject : ScriptableObject
{

    [SerializeField]
    private List<LevelLoreMessage> loreMessages = new();

    public List<LevelLoreMessage> LoreMessages { get {return new (loreMessages);}}
}

[System.Serializable]
public class LevelLoreMessage {
    public int Level;
    [TextArea(20, 40)]
    public string Message;
}
