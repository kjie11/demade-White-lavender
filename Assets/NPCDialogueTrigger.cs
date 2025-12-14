using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    private bool playerNearby;
    public GameObject hint;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hint.SetActive(true);
            playerNearby = true;
        }
            
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hint.SetActive(false);
            playerNearby = false;
        }
            
    }
    void Start()
    {
        hint.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            dialogueManager.StartDialogue();
        }
    }
}

