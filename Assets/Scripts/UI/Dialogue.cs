using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textComponent;

    [SerializeField]
    string[] lines; // Array to hold multiple lines of dialogue

    [SerializeField]
    float textSpeed;

    private int index; // Current line index for the dialogue array

    void Start()
    {
        textComponent.text = string.Empty; 
        StartDialogue();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) // for testing purposes, press space to advance dialogue
        {
            if(textComponent.text == lines[index]) // If the current line is fully displayed
            {
                NextLine();
            }
            else
            {
                // Stop the typing coroutine and display the full line immediately
                StopAllCoroutines(); 
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());  // Using coroutine to be able to wait between each letter
    }

    IEnumerator TypeLine() // Returns an IEnumerator to allow for coroutine functionality
    {
        // Start typing the current line letter by letter
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed); // Wait before adding the next character
        }
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
            gameObject.SetActive(false);
        }
    }
}
