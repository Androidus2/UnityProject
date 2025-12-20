using UnityEngine;

public class EnemyLookRandomizer : MonoBehaviour
{

    [SerializeField]
    private GameObject[] heads;
    [SerializeField]
    private GameObject[] chests;
    [SerializeField]
    private GameObject[] arms;
    [SerializeField]
    private GameObject[] belts;
    [SerializeField]
    private GameObject[] legs;
    [SerializeField]
    private GameObject[] feet;

    [SerializeField]
    private GameObject[] noses;
    [SerializeField]
    private GameObject[] hairs;
    [SerializeField]
    private GameObject[] faceHairs;
    [SerializeField]
    private GameObject[] eyes;
    [SerializeField]
    private GameObject[] eyebrows;
    [SerializeField]
    private GameObject[] ears;

    private void Awake()
    {
        // Armor isn't mandatory
        Generate(chests, false);
        Generate(arms, false);
        Generate(belts, false);
        Generate(legs, false);
        Generate(feet, false);

        // Facial details are mandatory
        Generate(noses, true);
        Generate(faceHairs, true);
        Generate(eyes, true);
        Generate(eyebrows, true);

        // A helmet and the hair should be mutually excluse
        // If we generate a helmet, we mustn't generate the ears, otherwise they are mandatory
        int shouldGenerateHelmet = Random.Range(0, 2);
        if (shouldGenerateHelmet == 0)
            Generate(heads, false);
        else
        {
            Generate(hairs, false);
            Generate(ears, true);
        }
    }

    // Activate at most 1 piece
    void Generate(GameObject[] pieces, bool required)
    {
        int maxx = pieces.Length;
        if (!required)
            maxx += 1;

        int ind = Random.Range(0, maxx);
        Debug.Log(ind);
        if (ind < pieces.Length)
            pieces[ind].SetActive(true);
        else
            Debug.Log("Did not activate piece");
    }

}
