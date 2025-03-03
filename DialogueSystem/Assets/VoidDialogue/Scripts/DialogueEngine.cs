using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueEngine : MonoBehaviour
{
    #region Configuração de UI e Áudio
    [HeaderImproved("UI Components", 16, "#16fc63", "#101010")]
    public GameObject uiPanel;
    public TextMeshProUGUI uiDisplayText;
    public TextMeshProUGUI speakerNameText;
    public GameObject optionsHolder;
    public GameObject optionPrefab;

    [HeaderImproved("Character and Audio Components",16, "#16fc63", "#101010")]
    public GameObject charactersHolder;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [HeaderImproved("Parameters", 16, "#16fc63", "#101010")]
    public float baseDelay = 0.05f;
    #endregion

    [HideInInspector]public ChatData conversationData;
    #region Internal Vars
    private Character currentCharacter;
    private float currentDelay;
    private float savedDelay;
    private Coroutine conversationCoroutine;
    private Coroutine typingCoroutine;
    private ChatState engineState;
    #endregion

    #region Start and Terminate Methods
    public void StartDialogue(ChatData data)
    {
        conversationData = data;
        conversationData.FullMessage = "";
        currentCharacter = LocateCharacter(data.SpeakerName);
        if (conversationData.SpeakerName != null)
            speakerNameText.text = conversationData.SpeakerName;
        ActivateCharacter(currentCharacter);
        conversationCoroutine = StartCoroutine(RunDialogue());
    }

    public void StartDialogueSequence(List<ChatData> dataList) => StartCoroutine(RunDialogueSequence(dataList));

    public void OnUserInteraction()
    {
        if (engineState == ChatState.Active)
            StartCoroutine(ForceCompleteTyping());
        else if (engineState == ChatState.Paused && conversationData.Options.Count == 0)
            TerminateDialogue();
    }

    public void SelectOption(int index)
    {
        var selectedOption = conversationData.Options.GetOption(index);
        conversationData.FullMessage = selectedOption.Key;
        TerminateDialogue();
    }

    private void TerminateDialogue()
    {
        if (conversationCoroutine != null)
            StopCoroutine(conversationCoroutine);
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        uiPanel.SetActive(false);
        charactersHolder.SetActive(false);
        optionsHolder.SetActive(false);
        engineState = ChatState.Inactive;

        conversationData.OnComplete?.Invoke();
        conversationData.OnComplete = null;
    }
    #endregion

    #region Main Coroutines
    private IEnumerator RunDialogueSequence(List<ChatData> dataList)
    {
        engineState = ChatState.Active;
        foreach (ChatData data in dataList)
        {
            StartDialogue(data);
            SetupOptions();
            while (engineState != ChatState.Paused)
                yield return null;
        }
    }

    private IEnumerator RunDialogue()
    {
        InitializeUI();
        engineState = ChatState.Active;
        foreach (var action in conversationData.Actions)
        {
            yield return ProcessAction(action);
        }
        if(conversationData.Options.Count == 0)
            engineState = ChatState.Paused;
    }

    private IEnumerator ProcessAction(ChatAction action)
    {
        switch (action.type)
        {
            case ActionType.Display:
                typingCoroutine = StartCoroutine(TypeText(action.parameter));
                yield return typingCoroutine;
                break;
            case ActionType.SetColor:
                conversationData.Formatting.Color = action.parameter;
                break;
            case ActionType.ChangeMood:
                UpdateCharacterExpression(action.parameter);
                break;
            case ActionType.Resize:
                conversationData.Formatting.ChangeSize(action.parameter);
                break;
            case ActionType.PlayAudio:
                PlayVoiceClip(action.parameter);
                break;
            case ActionType.AdjustSpeed:
                AdjustDelay(action.parameter);
                break;
            case ActionType.OnClick:
                yield return WaitForUserClick();
                break;
            case ActionType.Terminate:
                TerminateDialogue();
                yield break;
            case ActionType.Delay:
                yield return new WaitForSeconds(float.Parse(action.parameter));
                break;
        }
    }

    private IEnumerator TypeText(string textContent)
    {
        conversationData.FullMessage += conversationData.Formatting.OpeningTag;
        for (int i = 0; i < textContent.Length; i++)
        {
            conversationData.FullMessage += textContent[i];
            uiDisplayText.text = conversationData.FullMessage + conversationData.Formatting.ClosingTag;
            if (textContent[i] != ' ')
                PlaySFX();
            if (currentDelay > 0)
                yield return new WaitForSeconds(currentDelay);
        }
        conversationData.FullMessage += conversationData.Formatting.ClosingTag;
    }

    private IEnumerator WaitForUserClick()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;
        currentDelay = savedDelay;
    }

    private IEnumerator ForceCompleteTyping()
    {
        if (conversationData.SkipAllowed)
        {
            currentDelay = 0;
            while (engineState != ChatState.Paused)
                yield return null;
            currentDelay = baseDelay;
        }
    }
    #endregion

    #region Helpers

    private void InitializeUI()
    {
        currentDelay = baseDelay;
        savedDelay = baseDelay;
        uiDisplayText.text = "";
        uiPanel.SetActive(true);

        if (currentCharacter != null)
        {
            ActivateCharacter(currentCharacter);
            charactersHolder.SetActive(true);
        }
        else
        {
            charactersHolder.SetActive(false);
        }
    }

    private Character LocateCharacter(string characterName)
    {
        if (!string.IsNullOrEmpty(characterName))
        {
            Transform t = charactersHolder.transform.Find(characterName);
            if (t != null)
                return t.GetComponent<Character>();
        }
        return null;
    }

    private void ActivateCharacter(Character character)
    {
        foreach (Transform child in charactersHolder.transform)
            child.gameObject.SetActive(false);
        if (character != null)
            character.gameObject.SetActive(true);
    }

    private void SetupOptions()
    {
        ClearOptions();
        if (conversationData.Options.Count > 0)
        {
            optionsHolder.SetActive(true);
            for (int i = 0; i < conversationData.Options.Count; i++)
            {
                CreateOptionItem(i);
            }
        }
        else
        {
            optionsHolder.SetActive(false);
        }
    }

    private void ClearOptions()
    {
        for (int i = 1; i < optionsHolder.transform.childCount; i++)
            Destroy(optionsHolder.transform.GetChild(i).gameObject);
    }

    private void CreateOptionItem(int index)
    {
        GameObject newOption = Instantiate(optionPrefab, optionsHolder.transform);
        newOption.GetComponentInChildren<TextMeshProUGUI>().text = conversationData.Options.GetOption(index).Value;
        newOption.GetComponent<Button>().onClick.AddListener(() => SelectOption(index));
        newOption.SetActive(true);
    }

    private void UpdateCharacterExpression(string expression)
    {
        currentCharacter.GetComponent<Image>().sprite = currentCharacter.Mood.MoodMapping[expression];
    }

    private void AdjustDelay(string instruction)
    {
        switch (instruction)
        {
            case "up":
                currentDelay = Mathf.Max(currentDelay - 0.25f, 0.01f);
                break;
            case "down":
                currentDelay += 0.25f;
                break;
            case "reset":
                currentDelay = baseDelay;
                break;
            default:
                currentDelay = float.Parse(instruction);
                break;
        }
        savedDelay = currentDelay;
    }

    private void PlaySFX()
    {
        if (currentCharacter != null)
        {
            sfxSource.clip = currentCharacter.ChatSE[UnityEngine.Random.Range(0, currentCharacter.ChatSE.Length)];
            sfxSource.Play();
        }
    }

    private void PlayVoiceClip(string clipName)
    {
        if (currentCharacter != null)
        {
            AudioClip clip = Array.Find(currentCharacter.CallSE, c => c.name == clipName);
            voiceSource.clip = clip;
            voiceSource.Play();
        }
    }
    #endregion
}

