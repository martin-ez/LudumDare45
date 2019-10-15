using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Dialogue introDialogue;
    public Dialogue[] tutorialDialogue;
    public Dialogue[] levelDialogue;
    public string[] levelName;
    public DialogueManager dialogueManager;
    public Animator titleScreen;
    public Animator exitScreen;
    public Animator finalScreen;
    public Animator nextLevelScreen;
    public Animator startLevelScreen;
    public Text startLevelText;

    public bool showIntro = false;
    public int noLevels;

    public int level = 0;
    private Level currentLevel;
    private bool dialogueEnded;
    private bool firstItem;
    private Player player;
    private float endRadius = 60f;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        dialogueManager.OnDialogueEnd += EndDialogue;
       if (showIntro) StartCoroutine(IntroSequence());
       else StartCoroutine(LevelTransitionSequence());
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

    public void NextLevel()
    {
        nextLevelScreen.SetBool("isOpen", false);
        StartCoroutine(LevelTransitionSequence());
    }

    public void StartLevel()
    {
        startLevelScreen.SetBool("isOpen", false);
        currentLevel.StartGame();
        currentLevel.StartPing();
        player.Toggle();
        AudioManager.instance.StartLevel(level);
    }

    void EndDialogue()
    {
        dialogueEnded = true;
    }

    public void FirstItem()
    {
        firstItem = true;
    }

    void OnLevelCompleted(float radius, float nEndRadius, Vector3 target)
    {
        endRadius = nEndRadius;
        player.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName("Base"));
        player.Toggle();
        player.Disappear();
        StartCoroutine(EndLevelAnimation(radius, endRadius, target));
        if (level == 0)
        {
            StartCoroutine(EndTutorial());
        }
    }

    IEnumerator LoadLevel(string level, string unload = "None")
    {
        if (unload != "None")
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(unload);
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(level));
        currentLevel = FindObjectOfType<Level>();
        currentLevel.OnLevelComplete += OnLevelCompleted;
    }

    IEnumerator IntroSequence()
    {
        yield return StartCoroutine(LoadLevel("Tutorial"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(WaitForDialogue(introDialogue));
        currentLevel.RevealMap(10f);
        player.Appear(6f);
        AudioManager.instance.PlaySound(AudioManager.Sound.Intro);
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
        while (!firstItem)
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

        yield return StartCoroutine(FoWManager.instance.AnimateRadius(2, endRadius, 3f, true));

        if (level != 0)
        {
            if (level == noLevels) finalScreen.SetBool("isOpen", true);
            else nextLevelScreen.SetBool("isOpen", true);
        }
    }

    IEnumerator ResetLevelAnimation()
    {
        StartCoroutine(FoWManager.instance.AnimateRadius(endRadius, 0, 4f));
        Vector3 fromPos = player.transform.position;

        float t = 0;
        float time = 0;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / 4f;
            player.transform.position = Vector3.Lerp(fromPos, Vector3.zero, t);
            yield return null;
        }
    }

    IEnumerator LevelTransitionSequence()
    {
        yield return StartCoroutine(ResetLevelAnimation());
        level++;
        string nextLevel = "Level_" + level;
        string unload = level == 1 ? "Tutorial" : "Level_" + (level - 1);
        yield return StartCoroutine(LoadLevel(nextLevel, unload));
        currentLevel.RevealMap(3f);
        player.Appear(3f);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(WaitForDialogue(levelDialogue[level - 1]));
        startLevelText.text = "Level " + level + ":\n" + levelName[level - 1];
        startLevelScreen.SetBool("isOpen", true);
    }

    IEnumerator EndTutorial()
    {
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(WaitForDialogue(tutorialDialogue[6]));
        nextLevelScreen.SetBool("isOpen", true);
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
}
