using UnityEngine;

public class DiceManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
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

        Debug.Log($"Seed used for randomization: {config.RandomSeed}");

        foreach (var die in GameObject.FindGameObjectsWithTag("Dice"))
        {
            var script = die.GetComponent<InitScript>();
            script.RandomStart(config.RandomSeed);
        }
    }
}
