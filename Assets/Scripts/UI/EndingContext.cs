using UnityEngine;

public static class EndingContext
{
    public enum Source
    {
        GameCompleted,
        QuitFromMainMenu
    }

    public static Source source = Source.GameCompleted;

    public static void SetQuit() => source = Source.QuitFromMainMenu;
    public static void SetCompleted() => source = Source.GameCompleted; //to be used when ending is reached normally (stealing the wings)
}

