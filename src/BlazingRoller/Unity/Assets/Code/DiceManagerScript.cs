using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceManagerScript : MonoBehaviour
{
    private int _frameCounter = 0;
    private BlazingRoller.Unity.DiceThrowConfiguration _throwConfiguration;

    public GameObject prefabD4;
    public GameObject prefabD6;
    public GameObject prefabD8;
    public GameObject prefabD10;
    public GameObject prefabD12;
    public GameObject prefabD20;

    public GameObject guiCanvas;

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif

#if UNITY_EDITOR
        NewThrow("{\"RandomSeed\":509848614,\"Offset\":3,\"Dice\":[{\"Id\":0,\"Sides\":6,\"Multiplier\":-1},{\"Id\":1,\"Sides\":10,\"Multiplier\":1}]}");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        _frameCounter = (_frameCounter + 1) % 15;

        if (_frameCounter != 0)
        {
            return;
        }

        RenderResultsUi();
    }

    public void NewThrow(string serializedConfig)
    {
        Debug.Log(serializedConfig);

        _throwConfiguration = JsonUtility.FromJson<BlazingRoller.Unity.DiceThrowConfiguration>(serializedConfig);

        var random = new System.Random(_throwConfiguration.RandomSeed);

        Debug.Log($"Seed used for randomization: {_throwConfiguration.RandomSeed}");

        ToggleUI(false);

        var oldDice = GameObject.FindGameObjectsWithTag("Dice");
        foreach (var die in oldDice)
        {
            Destroy(die);
        }

        var dice = new List<GameObject>();

        foreach (var c in _throwConfiguration.Dice.OrderBy(_ => _.Id))
        {
            var die = CreateDie(c.Sides);

            if (die == null)
            {
                continue;
            }

            SetupDie(die, c.Multiplier);

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

    private void ToggleUI(bool showUi)
    {
        guiCanvas.SetActive(showUi);
    }

    private GameObject CreateDie(int sides)
    {
        switch (sides)
        {
            case 4:
                return Instantiate(prefabD4, new Vector3(0, -10, 0), new Quaternion(0, 0, 0, 1));
            case 6:
                return Instantiate(prefabD6, new Vector3(0, -10, 0), new Quaternion(0, 0, 0, 1));
            case 8:
                return Instantiate(prefabD8, new Vector3(0, -10, 0), new Quaternion(0, 0, 0, 1));
            case 10:
                return Instantiate(prefabD10, new Vector3(0, -10, 0), new Quaternion(0, 0, 0, 1));
            case 12:
                return Instantiate(prefabD12, new Vector3(0, -10, 0), new Quaternion(0, 0, 0, 1));
            case 20:
                return Instantiate(prefabD20, new Vector3(0, -10, 0), new Quaternion(0, 0, 0, 1));
            default:
                Debug.Log($"Invalid die encountered: {sides}");
                return null;
        }
    }

    private void SetupDie(GameObject die, int valueMultiplier)
    {
        die.tag = "Dice";
        die.AddComponent<DieScript>();
        die.GetComponent<DieScript>().SetMultiplier(valueMultiplier);
        die.transform.localScale = new Vector3(2, 2, 2);
    }

    private void RenderResultsUi()
    {
        var dice = GameObject.FindGameObjectsWithTag("Dice");

        if (dice.Length == 0)
        {
            return;
        }

        if (!IsSystemStable(dice))
        {
            return;
        }

        var values = GetDiceValues(dice);

        var resultText = GetResultText(values);

        ToggleUI(true);

        var textBox = GameObject.Find("ThrowResult").GetComponent<UnityEngine.UI.Text>();
        textBox.text = resultText;
    }

    private bool IsSystemStable(GameObject[] dice)
    {
        var isStopped = true;

        foreach (var die in dice)
        {
            var body = die.GetComponent<Rigidbody>();
            if (body.velocity.sqrMagnitude > 0.5)
            {
                isStopped = false;
                break;
            }

            if (body.angularVelocity.sqrMagnitude > 0.5)
            {
                isStopped = false;
                break;
            }
        }

        return isStopped;
    }

    private int[] GetDiceValues(GameObject[] dice)
    {
        var values = new int[dice.Length];

        var i = 0;
        foreach (var die in dice)
        {
            values[i++] = die.GetComponent<DieScript>().GetValue();
        }

        return values;
    }

    private string GetResultText(int[] values)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < values.Length; ++i)
        {
            var value = values[i];

            if (i == 0)
            {
                sb.Append(value);
            }
            else
            {
                if (value > 0)
                {
                    sb.Append(" + ");
                    sb.Append(value);
                }
                else
                {
                    sb.Append(" - ");
                    sb.Append(-value);
                }
            }
        }

        if (_throwConfiguration.Offset > 0)
        {
            sb.Append(" + ");
            sb.Append(_throwConfiguration.Offset);
        }
        else if (_throwConfiguration.Offset < 0)
        {
            sb.Append(" - ");
            sb.Append(-_throwConfiguration.Offset);
        }

        sb.Append(" = ");
        sb.Append(values.Sum() + _throwConfiguration.Offset);

        return sb.ToString();
    }
}
