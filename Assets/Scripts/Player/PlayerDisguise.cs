using UnityEngine;

public class PlayerDisguise : MonoBehaviour
{
    private static PlayerDisguise Instance { get; set; }

    private bool IsWearingGuardArmour { get; set; }

    private void Awake()
    {
        Debug.Log("PlayerDisguise Awake called.");
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public static PlayerDisguise GetInstance()
    {
        return Instance;
    }

    public bool GetIsWearingGuardArmour()
    {
        return IsWearingGuardArmour;
    }

    public void SetGuardArmour(bool active)
    {
        IsWearingGuardArmour = active;
    }
}
