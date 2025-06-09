using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel, charImages, dialogueUI;
    [SerializeField] TextMeshProUGUI dialogueText;
    internal Story currentStory;
    public bool dialogueIsPlaying { get; private set; }

    static DialogueManager instance;

    [SerializeField] GameObject[] choices;
    TextMeshProUGUI[] choicesText;

    [SerializeField] Image leftCharImg, rightCharImg;

    public DialogueTrigger dT;


    void Awake()
    {
        if (instance != null) Debug.LogWarning("More than one dialogue manager");
        instance = this;
    }

    void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        charImages.SetActive(false);
        dialogueUI.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    void Update()
    {
        if (!dialogueIsPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ContinueStory();
            leftCharImg.sprite = dT.charSprites[(int)currentStory.variablesState["step"]].characterSprites[0];
            rightCharImg.sprite = dT.charSprites[(int)currentStory.variablesState["step"]].characterSprites[1];
        }
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }
    
    // ----------------------------- Enter And Exit Dialogue Mode ----------------------------------
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        charImages.SetActive(true);
        dialogueUI.SetActive(true);

        leftCharImg.sprite = dT.charSprites[(int)currentStory.variablesState["step"]].characterSprites[0];
        rightCharImg.sprite = dT.charSprites[(int)currentStory.variablesState["step"]].characterSprites[1];

        ContinueStory();
    }

    IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(.2f);
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        charImages.SetActive(false);
        dialogueText.text = "";
    }

    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
        }
        else StartCoroutine(ExitDialogueMode());
    }
    
    // --------------------------------------- Choices --------------------------------------------
    void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length) Debug.LogError("Mas opciones de las que entran en UI : "
            + currentChoices.Count);

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}
