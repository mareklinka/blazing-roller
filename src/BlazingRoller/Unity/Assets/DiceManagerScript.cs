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

    public void NewThrow()
    {
        Debug.Log("Starting NewThrow");
        var dice = GameObject.FindGameObjectsWithTag("Dice");

        Debug.Log($"Found {dice.Length} dice");
        foreach (var die in dice)
        {
            var script = die.GetComponent<InitScript>();
            script.RandomStart();
        }

        Debug.Log("Done with NewThrow");
    }
}
