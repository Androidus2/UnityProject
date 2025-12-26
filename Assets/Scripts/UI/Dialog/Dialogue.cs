using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textComponent;

    [SerializeField]
    private GameObject dialogBox;

    [SerializeField]
    private DialogPiece defaultDialogPiece;

    [SerializeField]
    float textSpeed;

    [SerializeField]
    private SoundEffect typingSound;

    private string[] lines;
    private string[] unlockedMechanics; // Unused for now
    private int index; // Current line index for the dialogue array

    void Start()
    {
        if (defaultDialogPiece)
        {
            lines = defaultDialogPiece.Lines;
            unlockedMechanics = defaultDialogPiece.UnlockedMechanics;
            StartDialogue();
        }
    }

    void Update()
    {
        // TODO: Change to the new input system
        if(Input.GetKeyDown(KeyCode.Space) && index < lines.Length) // for testing purposes, press space to advance dialogue
        {
            if(textComponent.text == lines[index]) // If the current line is fully displayed
            {
                NextLine();
            }
            else
            {
                // Stop the typing coroutine and display the full line immediately
                typingSound.Stop();
                StopAllCoroutines(); 
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        textComponent.text = string.Empty;
        index = 0;
        dialogBox.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(TypeLine());  // Using coroutine to be able to wait between each letter
    }

    IEnumerator TypeLine() // Returns an IEnumerator to allow for coroutine functionality
    {
        typingSound.Play();
        // Start typing the current line letter by letter
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed); // Wait before adding the next character
        }
        typingSound.Stop();
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            foreach (string unlockedMechanic in unlockedMechanics)
            {
                // TODO: Move this to another script
                if(unlockedMechanic == "Enter game")
                {
                    Fade fade = FindFirstObjectByType<Fade>();
                    fade.BeginFade(() =>
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    });
                }
                PlayerMechanicsUnlocker.Instance.AddMechanic(unlockedMechanic);
            }
            dialogBox.SetActive(false);
        }
    }

    public void SetDialogPiece(DialogPiece piece)
    {
        lines = piece.Lines;
        unlockedMechanics = piece.UnlockedMechanics;
        StartDialogue();
    }
}
