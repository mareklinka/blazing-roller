using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;
using BlazingRoller.Unity;

public class DiceManagerScript : MonoBehaviour
{
    private int _frameCounter;
    private Vector3 _diceStartPosition = new Vector3(0,-15, 0);
    private Quaternion _diceStartOrientation = new Quaternion(0,0, 0, 1);
    private DiceThrowConfiguration _throwConfiguration;
    private bool _resultSent;

    [DllImport("__Internal")]
    private static extern void PropagateValue(string id, int value, string config);

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
        Application.targetFrameRate = 60;
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif

#if UNITY_EDITOR
        NewThrow("{\"ThrowId\":\"113d70d1-0457-472f-8603-aa1c90da132b\",\"ReturnFinalConfiguration\":true,\"RandomSeed\":1610762363,\"Offset\":0,\"Dice\":[{\"Id\":0,\"Sides\":20,\"Multiplier\":1}]}");
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

        _resultSent = false;
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

            SetupDie(die, c.Multiplier, c.Id);

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

        Physics.autoSimulation = true;
    }

    public void RepositionDice(string configString)
    {
        if (_throwConfiguration == null)
        {
            return;
        }

        var configuration = JsonUtility.FromJson<DieFinalConfigurationWrapper>(configString).Configuration.ToDictionary(_ => _.Id);

        Physics.autoSimulation = false;
        var dice = GameObject.FindGameObjectsWithTag("Dice");
        foreach (var die in dice)
        {
            var script = die.GetComponent<DieScript>();
            script.RepositionTo(configuration[script.GetId()]);
        }

        _resultSent = false; // a new result will need to be send
    }

    private void ToggleUI(bool showUi)
    {
        guiCanvas.SetActive(showUi);
    }

    private GameObject CreateDie(int sides)
    {
        GameObject die;
        switch (sides)
        {
            case 4:
                die = Instantiate(prefabD4, _diceStartPosition, _diceStartOrientation);
                die.transform.localScale = new Vector3(2.2F, 2.2F, 2.2F);
                return die;
            case 6:
                die = Instantiate(prefabD6, _diceStartPosition, _diceStartOrientation);
                die.transform.localScale = new Vector3(2.3F, 2.3F, 2.3F);
                return die;
            case 8:
                die = Instantiate(prefabD8, _diceStartPosition, _diceStartOrientation);
                die.transform.localScale = new Vector3(2, 2, 2);
                return die;
            case 10:
                die = Instantiate(prefabD10, _diceStartPosition, _diceStartOrientation);
                die.transform.localScale = new Vector3(2.3F, 2.3F, 2.3F);
                return die;
            case 12:
                die = Instantiate(prefabD12, _diceStartPosition, _diceStartOrientation);
                die.transform.localScale = new Vector3(2, 2, 2);
                return die;
            case 20:
                die = Instantiate(prefabD20, _diceStartPosition, _diceStartOrientation);
                die.transform.localScale = new Vector3(2.2F, 2.2F, 2.2F);
                return die;
            default:
                Debug.Log($"Invalid die encountered: {sides}");
                return null;
        }
    }

    private void SetupDie(GameObject die, int valueMultiplier, int id)
    {
        die.tag = "Dice";
        die.AddComponent<DieScript>();
        DieScript dieScript = die.GetComponent<DieScript>();
        dieScript.SetMultiplier(valueMultiplier);
        dieScript.SetId(id);
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
        else
        {
            Physics.autoSimulation = false;
        }

        (var values, var positions) = GetDiceValues(dice);
        int total = values.Sum() + _throwConfiguration.Offset;

        var resultText = GetResultText(values, total);

        if (!_resultSent)
        {
#if !UNITY_EDITOR
            var serializedConfig = _throwConfiguration.ReturnFinalConfiguration
                ? JsonUtility.ToJson(new DieFinalConfigurationWrapper { Configuration = positions })
                : string.Empty;
            Debug.Log(serializedConfig);
            PropagateValue(_throwConfiguration.ThrowId, total, serializedConfig);
#endif
            _resultSent = true;
        }

        ToggleUI(true);

        var textBox = GameObject.Find("ThrowResult").GetComponent<UnityEngine.UI.Text>();
        textBox.text = resultText;
    }

    private bool IsSystemStable(GameObject[] dice)
    {
        var isStopped = true;
        const float threshold = 0.1F;

        foreach (var die in dice)
        {
            var body = die.GetComponent<Rigidbody>();
            var dieScript = die.GetComponent<DieScript>();

            if (dieScript.IsRepositioning())
            {
                isStopped = false;
                break;
            }

            if (body.velocity.sqrMagnitude > threshold)
            {
                isStopped = false;
                break;
            }

            if (body.angularVelocity.sqrMagnitude > threshold)
            {
                isStopped = false;
                break;
            }
        }

        return isStopped;
    }

    private (int[] Values, DieFinalConfiguration[] Positions) GetDiceValues(GameObject[] dice)
    {
        var values = new int[dice.Length];
        var finalPositions = new DieFinalConfiguration[dice.Length];

        var i = 0;
        foreach (var die in dice)
        {
            var dieScript = die.GetComponent<DieScript>();
            values[i] = dieScript.GetValue();

            if (_throwConfiguration.ReturnFinalConfiguration)
            {
                var dfc = new DieFinalConfiguration
                {
                    Id = dieScript.GetId(),
                    X = die.transform.position.x,
                    Y = die.transform.position.y,
                    Z = die.transform.position.z,
                    RX = die.transform.rotation.x,
                    RY = die.transform.rotation.y,
                    RZ = die.transform.rotation.z,
                    RW = die.transform.rotation.w
                };

                finalPositions[i] = dfc;
            }

            ++i;
        }

        return (values, finalPositions);
    }

    private string GetResultText(int[] values, int total)
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
        sb.Append(total);

        return sb.ToString();
    }
}
