using UnityEngine;

public class DieModelSelector : MonoBehaviour
{
    public GameObject[] diceSet0;
    public GameObject[] diceSet1;
    public GameObject[] diceSet2;
    public GameObject[] diceSet3;
    public GameObject[] diceSet4;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal GameObject GetDiePrefab(int modelSet, int sides)
    {
        GameObject[] diceModels;

        switch (modelSet)
        {
            case 0:
                diceModels = diceSet0;
                break;
            case 1:
                diceModels = diceSet1;
                break;
            case 2:
                diceModels = diceSet2;
                break;
            case 3:
                diceModels = diceSet3;
                break;
            case 4:
                diceModels = diceSet4;
                break;
            default:
                diceModels = null;
                break;
        }

        if (diceModels == null)
        {
            return null;
        }

        switch (sides)
        {
            case 4:
                return diceModels[0];
            case 6:
                return diceModels[1];
            case 8:
                return diceModels[2];
            case 10:
                return diceModels[3];
            case 12:
                return diceModels[4];
            case 20:
                return diceModels[5];
            default:
                return null;
        }
    }
}
