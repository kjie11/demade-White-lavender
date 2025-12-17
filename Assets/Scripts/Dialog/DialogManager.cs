
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueNode
    {
        public string id;
        public string text;
        public string next;
    }

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
    public AudioClip dialogueVoiceClip;   // 胡言乱语 / NPC说话音效
    private AudioSource audioSource;

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
        //press Space to skip typing current sentense
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
        TextAsset json = Resources.Load<TextAsset>("dialog_npc");
        if (json == null)
        {
            Debug.LogError("dialog_npc.json not found in Resources!");
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

    
    IEnumerator PlayDialogue()
    {
        while (currentId != null&&currentId!="")
        {
            Debug.Log("currentID"+currentId);
            DialogueNode node = nodeMap[currentId];

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            typingCoroutine = StartCoroutine(TypeSentence(node.text));

            // waiting for finishing typing
            yield return new WaitUntil(() => isTyping == false);

            // 句末停顿
            yield return new WaitForSecondsRealtime(1f);

            currentId = node.next;
        }

        EndDialogue();
    }

    
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

        // 如果音效比打字长，定时停止
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
    void EndDialogue()
    {
        Debug.Log("end dialog");
        dialogueText.text = "";
        NPCNameText.SetActive(false);
        topBar.SetActive(false);
        bottomBar.SetActive(false);
        spaceHint.SetActive(false);

        pauseMenu.ResumeFromDialogue();
        isPlaying = false;
    }
}
