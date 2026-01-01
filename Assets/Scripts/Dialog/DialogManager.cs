using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//ai: how to read dialog from external document
public class DialogueManager : MonoBehaviour
{
    //ai: How to make a struct combine with configuration
    [System.Serializable]
    public struct DialogueNode
    {
        public string id;
        public string text;
        public string next;

        public int requireCoins;
        public int consumeCoins;
        public string reward;
    }
    //ai
    [System.Serializable]
    public struct DialogueData
    {
        public string start;
        public DialogueNode[] nodes;
    }

    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public GameObject NPCNameText;
    public GameObject topBar;
    public GameObject bottomBar;
    public GameObject spaceHint;

    [Header("Pause")]
    public PauseMenuUGUI pauseMenu;

    [Header("Typing Effect")]
    public float typeSpeed = 0.04f;

    private Dictionary<string, DialogueNode> nodeMap;
    private string currentId;
    private bool isPlaying;
    private Coroutine typingCoroutine;
    private bool isTyping;
    private string currentSentence;

    [Header("Audio")]
    public AudioClip dialogueVoiceClip;
    private AudioSource audioSource;

    [Header("Config")]
    public string configName = "dialog_npc";

    [Header("Player Coin UI")]
    public TextMeshProUGUI coinText;
    public bool isFinalBoss=false;

    void Start()
    {
        topBar.SetActive(false);
        bottomBar.SetActive(false);
        NPCNameText.SetActive(false);
        spaceHint.SetActive(false);
        dialogueText.text = "";

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isPlaying && isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            if (audioSource.isPlaying)
                audioSource.Stop();

            dialogueText.text = currentSentence;
            isTyping = false;
        }
    }

    public void StartDialogue()
    {
        if (isPlaying) return;
        isPlaying = true;
        LoadDialogue();
        topBar.SetActive(true);
        bottomBar.SetActive(true);
        NPCNameText.SetActive(true);
        spaceHint.SetActive(true);
        pauseMenu.PauseFromDialogue();
        StartCoroutine(PlayDialogue());
    }

    void LoadDialogue()
    {
        TextAsset json = Resources.Load<TextAsset>(configName);
        if (json == null)
        {
            Debug.LogError("json not found in Resources!");
            return;
        }

        DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);
        nodeMap = new Dictionary<string, DialogueNode>();
        foreach (var node in data.nodes)
        {
            nodeMap[node.id] = node;
        }

        currentId = data.start;
    }
    //ai: how to play dialog basedon linked node
    IEnumerator PlayDialogue()
    {
        while (!string.IsNullOrEmpty(currentId))
        {
            if (currentId == "ENDING")
        {
            StartCoroutine(PlayEndingSequence());
            yield break;  
        }
            DialogueNode node = nodeMap[currentId];
            Debug.Log("currentID = " + currentId);

            
            if (node.text.StartsWith("#CHECK_COINS_"))
            {
                yield return HandleCheckCoins(node);
                continue;
            }

           
            if (node.consumeCoins > 0)
            {
                int coins = GetPlayerCoins();
                coins -= node.consumeCoins;
                coinText.text = coins.ToString();
            }

            if (!string.IsNullOrEmpty(node.reward))
            {
                Debug.Log("Player received reward: " + node.reward);
            }

            
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeSentence(node.text));

            yield return new WaitUntil(() => isTyping == false);

            yield return new WaitForSecondsRealtime(1f);

            currentId = node.next;
        }

        EndDialogue();
    }
    IEnumerator PlayEndingSequence()
{
    pauseMenu.ResumeFromDialogue();   
    isPlaying = false;
    yield return new WaitForSeconds(1f);
    SceneManager.LoadScene("End");   
}
    IEnumerator HandleCheckCoins(DialogueNode node)
    {
        int required = node.requireCoins;
        int playerCoins = GetPlayerCoins();
        if (playerCoins >= required)
            currentId = node.next;
        else
            currentId = "notEnoughCoins";
        yield break;
    }
    //ai: how to implement the typing effect
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        currentSentence = sentence;
        dialogueText.text = "";
        if (dialogueVoiceClip != null)
        {
            float typingDuration = sentence.Length * typeSpeed;
            float clipDuration = dialogueVoiceClip.length;
            audioSource.clip = dialogueVoiceClip;
            audioSource.time = 0f;
            audioSource.Play();
            if (clipDuration > typingDuration)
            {
                Invoke(nameof(StopDialogueAudio), typingDuration);
            }
        }

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;
    }

    void StopDialogueAudio()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
    
    //ai: how to deal with the end node
    void EndDialogue()
    {
        dialogueText.text = "";
        NPCNameText.SetActive(false);
        topBar.SetActive(false);
        bottomBar.SetActive(false);
        spaceHint.SetActive(false);
        pauseMenu.ResumeFromDialogue();
        isPlaying = false;
        if (isFinalBoss)
        {
            SceneManager.LoadScene("End");  
        }
         
    }
    //ai: how to get the coin count from a UI text
    int GetPlayerCoins()
    {
        if (coinText == null) return 0;
        int coins = 0;
        int.TryParse(coinText.text, out coins);
        return coins;
    }
}
