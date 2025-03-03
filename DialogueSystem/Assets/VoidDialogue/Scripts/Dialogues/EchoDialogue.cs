using System.Collections.Generic;

public static class EchoDialogue
{
    public static DialogueSequence GetDialogue()
    {
        DialogueSequence echoSequence = new()
        {
            dialogues = new List<ChatData>()
        };

        var Question = new ChatData("Do you think that I look cool?", "Echo");
        Question.Options.AddOption("Correct", "Yeaa <3");
        Question.Options.AddOption("Wrong", "Hell naw");
        Question.Options.AddOption("Whatever", "Idk");
        Question.OnComplete = () => DialogueManager.Instance.CheckAnswer("Echo", "Echo_LooksCool", new Dictionary<string, string>
        {
            { "Correct", "Yayyy thx bud/OnClick/" },
            { "Wrong", "Fuck u dude/OnClick/" },
            { "Whatever", "Aww c'mon bro/OnClick/" }
        });

        echoSequence.dialogues.Add(new ChatData("/Resize:up/Sup, /Resize:reset/my name is Echo./OnClick/", "Echo"));
        echoSequence.dialogues.Add(new ChatData("I have nuclear butt disease/OnClick/", "Echo"));
        echoSequence.dialogues.Add(new ChatData("It hurts /SetColor:Red//Resize:99/A LOT/SetColor:White//Resize:reset//OnClick/", "Echo"));
        echoSequence.dialogues.Add(new ChatData("/AdjustSpeed:0.1/I hope this suffering ends soon... /AdjustSpeed:reset//OnClick/", "Echo"));
        echoSequence.dialogues.Add(new ChatData("/AdjustSpeed:0.08/I really want it to stop.../OnClick/", "Echo", null, false));
        echoSequence.dialogues.Add(new ChatData("/AdjustSpeed:0.035/Aight, i'll stop talking about it. /PlayAudio:Ananas/ /AdjustSpeed:reset//OnClick/", "Echo"));
        echoSequence.dialogues.Add(new ChatData("I can turn into a duck btw... look /ChangeMood:Duck//OnClick/", "Echo"));
        echoSequence.dialogues.Add(Question);
        echoSequence.dialogues.Add(new ChatData("/ChangeMood:Joinha/uh... /Delay:2.5/it was nice meeting u ig/OnClick/", "Echo", null, false));
        echoSequence.dialogues.Add(new ChatData("/ChangeMood:Joinha/Aight. Bye!!/OnClick//Terminate/", "Echo"));

        return echoSequence;
    }
}