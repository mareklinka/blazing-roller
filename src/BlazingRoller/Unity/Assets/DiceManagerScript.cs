using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceManagerScript : MonoBehaviour
{
    public GameObject prefabD6;

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif

#if UNITY_EDITOR
        NewThrow("{\"RandomSeed\":509848614,\"Dice\":[{\"Id\":0,\"Sides\":6},{\"Id\":1,\"Sides\":6},{\"Id\":2,\"Sides\":6}]}");
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NewThrow(string serializedConfig)
    {
        Debug.Log(serializedConfig);
        var config = JsonUtility.FromJson<BlazingRoller.Unity.DiceThrowConfiguration>(serializedConfig);

        var random = new System.Random(config.RandomSeed);

        Debug.Log($"Seed used for randomization: {config.RandomSeed}");

        var oldDice = GameObject.FindGameObjectsWithTag("Dice");
        foreach (var die in oldDice)
        {
            Destroy(die);
        }

        var dice = new List<GameObject>();

        foreach (var c in config.Dice.OrderBy(_ => _.Id))
        {
            if (c.Sides != 6)
            {
                continue;
            }

            var die = Instantiate(prefabD6, new Vector3(0, -10, 0), new Quaternion(0, 0, 0, 1));
            die.tag = "Dice";

            dice.Add(die);
        }

        var i = 0;
        foreach (var die in dice)
        {
            var dieSeed = random.Next();
            Debug.Log($"Seed for die {i++}: {dieSeed}");
            var script = die.GetComponent<DieScript>();
            script.RandomStart(dieSeed);
        }
    }
}
