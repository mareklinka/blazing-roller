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

    public string NewThrow(int seed)
    {
        Debug.Log($"Seed used for randomization: {seed}");

        foreach (var die in GameObject.FindGameObjectsWithTag("Dice"))
        {
            var script = die.GetComponent<InitScript>();
            script.RandomStart(seed);
        }

        return seed.ToString();
    }
}
