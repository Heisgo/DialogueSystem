using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.VersionControl;
using UnityEngine;

public class Example : MonoBehaviour
{
    public DialogueEngine DialogueEngine;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !DialogueEngine.uiPanel.activeInHierarchy)
            DialogueManager.Instance.StartDialogue("Echo");
    }
}
