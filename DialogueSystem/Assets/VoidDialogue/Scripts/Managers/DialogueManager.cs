using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueSequence
{
    public List<ChatData> dialogues;
}
public class DialogueManager : PersistentMonoBehaviourSingleton<DialogueManager>
{
    public DialogueEngine dialogueEngine;

    private Dictionary<string, DialogueSequence> dialogueMap = new();

    protected override void Awake()
    {
        base.Awake();
        LoadDialogues();
    }

    private void LoadDialogues()
    {
        dialogueMap = new Dictionary<string, DialogueSequence>();
        #region EchoDialogue
        dialogueMap.Add("Echo", EchoDialogue.GetDialogue());
        #endregion
    }

    public void StartDialogue(string npcId)
    {
        if (dialogueMap.TryGetValue(npcId, out DialogueSequence sequence))
        {
            dialogueEngine.StartDialogueSequence(sequence.dialogues);
            return;
        }

        Debug.LogWarning($"No dialogue found ({npcId})");
    }
    public void CheckAnswer(string npc_name, string questionId, Dictionary<string, string> responses)
    {
        if (dialogueEngine.conversationData.FullMessage == null)
            return;

        string selectedOption = dialogueEngine.conversationData.FullMessage;

        if (responses.TryGetValue(selectedOption, out string response))
        {
            List<ChatData> dialogueTexts = new() { new ChatData(response, npc_name) };
            dialogueEngine.StartDialogueSequence(dialogueTexts);
        }
        else
            Debug.LogWarning($"Invalid Option '{questionId}': {selectedOption}");
    }
}
