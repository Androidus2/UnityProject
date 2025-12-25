using UnityEngine;

public class Karma : MonoBehaviour
{
    public static Karma instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }
    public int killCount = 0;
    public int stealCount = 0;
    public void AddKarmaKill(int amount)
    {
       killCount += amount;
    }
    public void AddKarmaSteal(int amount)
    {
        stealCount += amount;
    }

    public int KarmaPrice() //mostly for vendor prices
    {
        int sum = (killCount * 30) + stealCount*5;
        return sum;
    }

    //for endings, we will will care more about kills than steals
}