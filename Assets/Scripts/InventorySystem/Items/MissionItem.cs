using UnityEngine;

[CreateAssetMenu(fileName = "New Mission Object", menuName = "Scripts/InventorySystem/Items/Mission")]
public class MissionObject : ItemObject
{
    [SerializeField]
    private string missionName;

    public void Awake()
    {
        type = ItemType.Mission;
    }

    public string GetMissionName () { return missionName; }
    public void SetMissionName(string missionName) { this.missionName = missionName; }
}