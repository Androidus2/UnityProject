using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    private float sensitivity = 1.0f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public float GetSensitivity()
    {
        return sensitivity;
    }

    public void SetSensitivity(float sensitivity)
    {
        this.sensitivity = sensitivity;
    }

}
