using UnityEngine;

public class Karma : MonoBehaviour
{
    private static Karma instance;
    private void Awake()
    {
        instance = this;
        
    }
    private int killCount = 0;
    private int stealCount = 0;

    public static Karma GetInstance()
    {
        return instance;
    }


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

    public int getKillScore()
    {
        return killCount;
    }
}