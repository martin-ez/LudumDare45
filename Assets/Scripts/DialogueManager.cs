using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public System.Action OnDialogueEnd;

    private Queue<string> sentences;
    private bool dialogueActive;
    private bool typing;
    private Animator animator;

    private bool skip;

    void Awake()
    {
        sentences = new Queue<string>();
        dialogueActive = false;
        typing = false;
        animator = GetComponent<Animator>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueActive = true;
        animator.SetBool("isOpen", true);
        sentences.Clear();
        foreach (string line in dialogue.lines)
        {
            sentences.Enqueue(line);
        }

       NextLine();
    }

    public void Action()
    {
        if (dialogueActive)
        {
            if (typing) EndLine();
            else NextLine();
        }
    }

    public void NextLine()
    {
        if (sentences.Count == 0)
        {
            dialogueText.text = "";
            OnDialogueEnd?.Invoke();
            dialogueActive = false;
            animator.SetBool("isOpen", false);
            return;
        }

        StartCoroutine(TypeLine(sentences.Dequeue()));
    }

    private void EndLine()
    {
        skip = true;
        typing = false;
    }

    IEnumerator TypeLine(string line)
    {
        skip = false;
        typing = true;
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
            if (skip)
            {
                dialogueText.text = line;
                break;
            }
        }
        typing = false;
    }
}

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 5)]
    public string[] lines;
}
