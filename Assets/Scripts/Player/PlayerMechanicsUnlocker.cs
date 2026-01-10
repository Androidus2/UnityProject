using UnityEngine;
using System.Collections.Generic;

public class PlayerMechanicsUnlocker : MonoBehaviour
{
    [SerializeField]
    private bool isTutorial = false;

    private static PlayerMechanicsUnlocker instance;
    public static PlayerMechanicsUnlocker Instance => instance;

    private Dictionary<string, bool> mechanicsMap = new Dictionary<string, bool>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void AddMechanic(string mechanicName)
    {
        mechanicsMap[mechanicName] = true;
    }

    public void ResetMechanics()
    {
        mechanicsMap.Clear();
    }

    // Check if key exists AND value is true or we are outside the tutorial
    public bool IsMechanicUnlocked(string mechanicName)
    {
        return !isTutorial || (mechanicsMap.TryGetValue(mechanicName, out bool isUnlocked) && isUnlocked);
    }
}
