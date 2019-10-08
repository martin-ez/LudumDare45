using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Dialogue introDialogue;
    public Dialogue[] tutorialDialogue;
    public Dialogue[] levelDialogue;
    public DialogueManager dialogueManager;
    public Animator titleScreen;
    public Animator exitScreen;
    public Animator finalScreen;

    int level = 0;
    private Level currentLevel;
    private bool dialogueEnded;
    private bool firstItem;
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        dialogueManager.OnDialogueEnd += EndDialogue;
        StartCoroutine(IntroSequence());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) exitScreen.SetBool("isOpen", !exitScreen.GetBool("isOpen"));
    }

    public void Continue()
    {
        exitScreen.SetBool("isOpen", false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        StartCoroutine(TutorialSequence());
    }

    public void LoadNextLevel()
    {
        level++;
        // TODO
    }

    IEnumerator LoadLevel(string level)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        currentLevel = FindObjectOfType<Level>();
        currentLevel.OnLevelComplete += OnLevelCompleted;
    }

    IEnumerator IntroSequence()
    {
        yield return StartCoroutine(LoadLevel("Tutorial"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForDialogue(introDialogue));
        currentLevel.RevealMap();
        player.Appear();
        yield return new WaitForSeconds(6f);
        titleScreen.SetBool("isOpen", true);
    }

    IEnumerator TutorialSequence()
    {
        titleScreen.SetBool("isOpen", false);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[0]));
        currentLevel.StartGame();
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[1]));
        player.Toggle();
        DistanceCounter counter = player.gameObject.AddComponent<DistanceCounter>();
        while (counter.distanceTravelled < 10f)
        {
            yield return null;
        }
        Destroy(counter);
        player.Toggle();
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[2]));
        currentLevel.StartPing();
        yield return new WaitForSeconds((60f / 105f) * 8f);
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[3]));
        player.Toggle();
        firstItem = false;
        while(!firstItem)
        {
            yield return null;
        }
        player.Toggle();
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[4]));
        player.Toggle();
        firstItem = false;
        while (!firstItem)
        {
            yield return null;
        }
        player.Toggle();
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[5]));
        player.Toggle();
    }

    IEnumerator EndTutorial()
    {
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[6]));
        finalScreen.SetBool("isOpen", true);
    }

    IEnumerator WaitForDialogue(Dialogue dialogue)
    {
        dialogueEnded = false;
        dialogueManager.StartDialogue(dialogue);
        while (!dialogueEnded)
        {
            yield return null;
        }
    }

    void EndDialogue()
    {
        dialogueEnded = true;
    }

    public void FirstItem()
    {
        firstItem = true;
    }

    void OnLevelCompleted(float radius, float endRadius, Vector3 target)
    {
        player.Toggle();
        player.Disappear();
        StartCoroutine(EndLevelAnimation(radius, endRadius, target));
        if (level == 0)
        {
            StartCoroutine(EndTutorial());
        }
        else
        {
            LoadNextLevel();
        }
    }

    IEnumerator EndLevelAnimation(float radius, float endRadius, Vector3 target)
    {
        StartCoroutine(FoWManager.instance.AnimateRadius(radius, 2, 3f));
        Vector3 fromPos = player.transform.position;

        float t = 0;
        float time = 0;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / 2f;
            player.transform.position = Vector3.Lerp(fromPos, target, t);
            yield return null;
        }

        StartCoroutine(FoWManager.instance.AnimateRadius(2, endRadius, 3f, true));
    }
}
