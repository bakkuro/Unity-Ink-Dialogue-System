using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] GameObject cue;
    bool playerIn;
    [SerializeField] TextAsset inkJSON;
    [SerializeField] public CharSpritesList[] charSprites;

    [System.Serializable] public class CharSpritesList { public Sprite[] characterSprites; }

    void Awake()
    {
        playerIn = false;
        cue.SetActive(false);
    }

    void Update()
    {
        if (playerIn && InputManager.GetInstance().GetInteractPressed() && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().dT = this;
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
        }
    }
    // --------------------------------- Mouse Triggers -------------------------------------
    private void OnMouseDown()
    {
        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().dT = this;
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
        }
    }
    private void OnMouseEnter()
    {
        cue.SetActive(true);
    }
    private void OnMouseExit()
    {
        cue.SetActive(false);
    }
    // ----------------------------------- Collisions ---------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") playerIn = true; cue.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") playerIn = false; cue.SetActive(false);
    }
}
