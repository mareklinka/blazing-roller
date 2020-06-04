using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Highlights the number that is face up. Disable this script to make your dice no longer glow.
public class DiceHighlight : MonoBehaviour
{
    public GameObject[] sides;
    DiceStats diceStats;
    // Start is called before the first frame update
    void Start()
    {
        diceStats=gameObject.GetComponent<DiceStats>();
    }

    // Update is called once per frame
    void Update()
    {
        HighlightSides();
    }
    void HighlightSides()
    {
        for(int i = 0; i<sides.Length; i++)
        {
            sides[i].SetActive(false);

        }
        sides[diceStats.side - 1].SetActive(true);
    }
}
