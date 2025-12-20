
// using UnityEngine;
// using TMPro;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;

// public class DialogueManager : MonoBehaviour
// {
//     [System.Serializable]
//     public struct DialogueNode
//     {
//         public string id;
//         public string text;
//         public string next;

//         public string choiceA;
//         public string choiceB;

//         public int requireCoins;
//         public int consumeCoins;
//         public string reward;

//     }

//     [System.Serializable]
//     public struct DialogueData
//     {
//         public string start;
//         public DialogueNode[] nodes;
//     }

//     [Header("UI")]
//     public TextMeshProUGUI dialogueText;
//     public GameObject NPCNameText;
//     public GameObject topBar;
//     public GameObject bottomBar;
//     public GameObject spaceHint;

//     [Header("Pause")]
//     public PauseMenuUGUI pauseMenu;

//     [Header("Typing Effect")]
//     public float typeSpeed = 0.04f;

//     [Header("Dialog Choice")]
//     public GameObject choiceUI;
//     public TMPro.TextMeshProUGUI choiceAText;
//     public TMPro.TextMeshProUGUI choiceBText;
//     public UnityEngine.UI.Button choiceButtonA;
//     public UnityEngine.UI.Button choiceButtonB;


//     private Dictionary<string, DialogueNode> nodeMap;
//     private string currentId;
//     private bool isPlaying;

//     private Coroutine typingCoroutine;
//     private bool isTyping;
//     private string currentSentence;

//     [Header("Audio")]
//     public AudioClip dialogueVoiceClip;   // 胡言乱语 / NPC说话音效
//     private AudioSource audioSource;

//     [Header("Config")]
//     public string configName = "dialog_npc";

//     [Header("Player Coin UI")]
//     public TMPro.TextMeshProUGUI coinText; //refer to coin ui to get coin amount


//     void Start()
//     {
//         topBar.SetActive(false);
//         bottomBar.SetActive(false);
//         NPCNameText.SetActive(false);
//         spaceHint.SetActive(false);
//         dialogueText.text = "";
//         // choiceUI.SetActive(false);
//          choiceButtonA.gameObject.SetActive(false);
//          choiceButtonB.gameObject.SetActive(false);



//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null)
//             audioSource = gameObject.AddComponent<AudioSource>();

//         audioSource.playOnAwake = false;
//     }

//     void Update()
//     {
//         //press Space to skip typing current sentense
//         if (isPlaying && isTyping && Input.GetKeyDown(KeyCode.Space))
//         {
//             if (typingCoroutine != null)
//             {
//                 StopCoroutine(typingCoroutine);
//                 typingCoroutine = null;
//             }
//             if (audioSource.isPlaying)
//                 audioSource.Stop();


//             dialogueText.text = currentSentence;
//             isTyping = false;
//         }

//     }


//     public void StartDialogue()
//     {
//         if (isPlaying) return;
//         isPlaying = true;

//         LoadDialogue();

//         topBar.SetActive(true);
//         bottomBar.SetActive(true);
//         NPCNameText.SetActive(true);
//         spaceHint.SetActive(true);

//         pauseMenu.PauseFromDialogue();

//         StartCoroutine(PlayDialogue());
//     }


//     void LoadDialogue()
//     {
//         TextAsset json = Resources.Load<TextAsset>(configName);
//         if (json == null)
//         {
//             Debug.LogError("json not found in Resources!");
//             return;
//         }

//         DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);

//         nodeMap = new Dictionary<string, DialogueNode>();
//         foreach (var node in data.nodes)
//         {
//             nodeMap[node.id] = node;
//         }

//         currentId = data.start;
//     }


//     IEnumerator PlayDialogue()
//     {
//         while (currentId != null && currentId != "")
//         {
//             Debug.Log("currentID" + currentId);
//             DialogueNode node = nodeMap[currentId];

//             if (node.text.StartsWith("#CHECK_COINS_"))
//             {
//                 yield return HandleCheckCoins(node);
//                 continue;
//             }

//             // 2️⃣ 检查是否为选择节点
//             if (!string.IsNullOrEmpty(node.choiceA))
//             {
//                 yield return HandleChoiceNode(node);
//                 continue;
//             }

//             if (typingCoroutine != null)
//             {
//                 StopCoroutine(typingCoroutine);
//                 typingCoroutine = null;
//             }

//             typingCoroutine = StartCoroutine(TypeSentence(node.text));

//             // waiting for finishing typing
//             yield return new WaitUntil(() => isTyping == false);

//             // 句末停顿
//             yield return new WaitForSecondsRealtime(1f);

//             currentId = node.next;
//         }

//         EndDialogue();
//     }

//     IEnumerator HandleCheckCoins(DialogueNode node)
//     {
//         int required = node.requireCoins;
//         int playerCoins = GetPlayerCoins();

//         if (playerCoins >= required)
//             currentId = node.next;
//         else
//             currentId = "notEnoughCoins";

//         yield break;
//     }

//     IEnumerator HandleChoiceNode(DialogueNode node)
//     {
//         choiceUI.SetActive(true);
//         choiceButtonA.gameObject.SetActive(true);
//          choiceButtonB.gameObject.SetActive(true);
//         choiceAText.text = "Give coins";
//         choiceBText.text = "Refuse";

//         bool selected = false;

//         void ChooseA()
//         {
//             currentId = node.choiceA;
//             selected = true;
//         }

//         void ChooseB()
//         {
//             currentId = node.choiceB;
//             selected = true;
//         }

//         // 监听按钮点击
//         choiceButtonA.onClick.AddListener(ChooseA);
//         choiceButtonB.onClick.AddListener(ChooseB);

//         // 等待选择
//         yield return new WaitUntil(() => selected);

//         // 清除
//         choiceButtonA.onClick.RemoveAllListeners();
//         choiceButtonB.onClick.RemoveAllListeners();
//         choiceUI.SetActive(false);
//     }


//     IEnumerator TypeSentence(string sentence)
//     {
//         isTyping = true;
//         currentSentence = sentence;
//         dialogueText.text = "";
//         if (dialogueVoiceClip != null)
//         {
//             float typingDuration = sentence.Length * typeSpeed;
//             float clipDuration = dialogueVoiceClip.length;

//             audioSource.clip = dialogueVoiceClip;
//             audioSource.time = 0f;
//             audioSource.Play();

//             // 如果音效比打字长，定时停止
//             if (clipDuration > typingDuration)
//             {
//                 Invoke(nameof(StopDialogueAudio), typingDuration);
//             }
//         }

//         foreach (char letter in sentence)
//         {
//             dialogueText.text += letter;
//             yield return new WaitForSecondsRealtime(typeSpeed);
//         }

//         isTyping = false;
//     }

//     void StopDialogueAudio()
//     {
//         if (audioSource.isPlaying)
//             audioSource.Stop();
//     }
//     void EndDialogue()
//     {
//         Debug.Log("end dialog");
//         dialogueText.text = "";
//         NPCNameText.SetActive(false);
//         topBar.SetActive(false);
//         bottomBar.SetActive(false);
//         spaceHint.SetActive(false);

//         pauseMenu.ResumeFromDialogue();
//         isPlaying = false;
//     }


//     int GetPlayerCoins()
//     {
//         if (coinText == null) return 0;

//         int coins = 0;
//         int.TryParse(coinText.text, out coins);
//         return coins;
//     }
// }


using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
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

    IEnumerator PlayDialogue()
    {
        while (!string.IsNullOrEmpty(currentId))
        {
            DialogueNode node = nodeMap[currentId];
            Debug.Log("currentID = " + currentId);

            // ----------- CHECK COINS ----------- 
            if (node.text.StartsWith("#CHECK_COINS_"))
            {
                yield return HandleCheckCoins(node);
                continue;
            }

            // ----------- REWARD & CONSUME ----------- 
            if (node.consumeCoins > 0)
            {
                int coins = GetPlayerCoins();
                coins -= node.consumeCoins;
                coinText.text = coins.ToString();
            }

            if (!string.IsNullOrEmpty(node.reward))
            {
                Debug.Log("Player received reward: " + node.reward);
                // TODO: add to inventory if needed
            }

            // --------------------------------
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

    void EndDialogue()
    {
        dialogueText.text = "";
        NPCNameText.SetActive(false);
        topBar.SetActive(false);
        bottomBar.SetActive(false);
        spaceHint.SetActive(false);

        pauseMenu.ResumeFromDialogue();
        isPlaying = false;
    }

    int GetPlayerCoins()
    {
        if (coinText == null) return 0;

        int coins = 0;
        int.TryParse(coinText.text, out coins);
        return coins;
    }
}
