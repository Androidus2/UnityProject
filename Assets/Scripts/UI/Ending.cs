using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class Ending : MonoBehaviour
{
    [SerializeField] 
    private Fade fade;

    [SerializeField] 
    private TMP_Text textSection;

    [SerializeField]
    private int kill_limit;

    private void Start()
    {
        UpdateEndingText();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateEndingText()
    {
        string endingText;

        if (EndingContext.source == EndingContext.Source.QuitFromMainMenu)
        {
            Debug.Log("Player quit from main menu, showing LOSER ending.");
            endingText =
                "<b>YOU QUIT.</b>\n" +
                "Your choices have shaped who you became.\n\n" +
                "<b>LOSER</b>\n\n" +
                "No wings, so no afterlife for you...your body unthreads into dust, pixel by pixel and the angels have never been more delighted."; 
            textSection.text = endingText;
            return;
        }


        int kills = Karma.GetInstance().getKillScore();
        

        if (kills <= 0)
        {
            endingText =
                "<b>THE END</b>\n" +
                "Your choices have shaped who you became.\n\n" +
                "<b>SAINT</b>\n\n" +
                "The angels fear your goodness; afraid you’re a saint in disguise and will shut down their playground, " +
                "but ultimately send you ‘up’.";
        }
        else if (kills <= kill_limit)
        {
            endingText =
                "<b>THE END</b>\n" +
                "Your choices have shaped who you became.\n\n" +
                "<b>DELINQUENT</b>\n\n" +
                "The angels make fun of you, berate you, guilttrip you for taking the easy route and send you down in flames.";
        }
        else
        {
            endingText =
                "<b>THE END</b>\n" +
                "Your choices have shaped who you became.\n\n" +
                "<b>ONE OF THEM</b>\n\n" +
                "The angels offer you to join them in running the playground – you seem to enjoy torture as much as they do.";
        }

        if (textSection != null)
            textSection.text = endingText;
    }

    public void GoToMainMenu()
    {

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fade != null)
        {
            fade.BeginFade(() =>
            {
                EndingContext.SetCompleted();
                SceneManager.LoadScene("MainMenu");
            });
        }
        else
        {
            EndingContext.SetCompleted();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
