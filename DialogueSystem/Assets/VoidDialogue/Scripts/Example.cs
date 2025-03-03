using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.VersionControl;
using UnityEngine;

public class Example : MonoBehaviour
{
    public DialogueEngine DialogueEngine;
    private List<ChatData> dialogueTexts;
    void Awake()
    {
        var Question = new ChatData("Do you think that I look cool?", "Echo");
        Question.Options.AddOption("Correct", "Yeaa <3");
        Question.Options.AddOption("Wrong", "Hell naw");
        Question.Options.AddOption("Whatever", "Idk");
        Question.OnComplete = () => Check_Correct();
        dialogueTexts = new List<ChatData>
        {
            new("/Resize:up/Sup, /Resize:reset/my name is Echo./OnClick/", "Echo"),
            new("I have nuclear butt disease/OnClick/", "Echo"),
            new("It hurts /SetColor:Red//Resize:99/A LOT/OnClick/", "Echo"),
            new("/AdjustSpeed:0.1/I hope this suffering ends soon... /AdjustSpeed:reset//OnClick/", "Echo"),
            new("/AdjustSpeed:0.08/I really want it to stop.../OnClick/", "Echo", null, false),
            new("/AdjustSpeed:0.035/Aight, i'll stop talking about it. /PlayAudio:Ananas/ /AdjustSpeed:reset//OnClick/", "Echo"),
            new("I can turn into a duck btw... look /ChangeMood:Duck//OnClick/", "Echo"),
            Question,
            new("/ChangeMood:Joinha/uh... /Delay:2.5/it was nice meeting u ig/OnClick/", "Echo", null, false),
            new("/ChangeMood:Joinha/Aight. Bye!!/OnClick//Terminate/", "Echo"),
        };
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !DialogueEngine.uiPanel.activeInHierarchy)
            DialogueEngine.StartDialogueSequence(dialogueTexts);
    }
    private void Check_Correct()
    {

        if (DialogueEngine.conversationData.FullMessage == "Correct")
        {
            var dialogueTexts = new List<ChatData>
        {
            new("Yayyy thx bud/OnClick/")
        };

            DialogueEngine.StartDialogueSequence(dialogueTexts);
        }
        else if (DialogueEngine.conversationData.FullMessage == "Wrong")
        {
            var dialogueTexts = new List<ChatData>
        {
            new("Fuck u dude/OnClick/")
        };

            DialogueEngine.StartDialogueSequence(dialogueTexts);
        }
        else
        {
            var dialogueTexts = new List<ChatData>
        {
            new("Aww c'mon bro/OnClick/")
        };

            DialogueEngine.StartDialogueSequence(dialogueTexts);
        }
    }
}
