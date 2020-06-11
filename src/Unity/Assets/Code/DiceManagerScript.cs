using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;
using BlazingRoller.Unity;

public class DiceManagerScript : MonoBehaviour
{
    private int _frameCounter;
    private Vector3 _diceStartPosition = new Vector3(0, -15, 0);
    private Quaternion _diceStartOrientation = new Quaternion(0, 0, 0, 1);
    private DiceThrowConfiguration _throwConfiguration;
    private bool _resultSent;

    [DllImport("__Internal")]
    private static extern void PropagateValue(string id, int value, string config);

    public GameObject guiCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif

#if UNITY_EDITOR
        NewThrow("{\"ThrowId\":\"113d70d1-0457-472f-8603-aa1c90da132b\",\"DiceSet\":1,\"ReturnFinalConfiguration\":true,\"RandomSeed\":1610762363,\"Offset\":0,\"Dice\":[{\"Id\":0,\"Sides\":100,\"Multiplier\":1}]}");
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
        _resultSent = false;
        _throwConfiguration = JsonUtility.FromJson<DiceThrowConfiguration>(serializedConfig);

        if (_throwConfiguration.Dice.Length > 15)
        {
            return;
        }

        var random = new System.Random(_throwConfiguration.RandomSeed);

        ToggleUI(false);

        foreach (var die in GameObject.FindGameObjectsWithTag("Dice"))
        {
            Destroy(die);
        }

        var dice = new List<GameObject>();

        foreach (var c in _throwConfiguration.Dice.OrderBy(_ => _.Id))
        {
            var modelSelector = GetComponent<DieModelSelector>();

            var createdDice = CreateDie(modelSelector, _throwConfiguration.DiceSet, c.Sides, c.Id, c.Multiplier);

            if (createdDice == null)
            {
                continue;
            }

            dice.AddRange(createdDice);
        }

        foreach (var die in dice)
        {
            var dieSeed = random.Next();
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

        foreach (var die in GameObject.FindGameObjectsWithTag("Dice"))
        {
            var script = die.GetComponent<DieScript>();
            script.RepositionTo(configuration[script.Id]);
        }

        _resultSent = false; // a new result will need to be sent
    }

    private void ToggleUI(bool showUi) => guiCanvas.SetActive(showUi);

    private GameObject[] CreateDie(DieModelSelector modelSelector, int modelSet, int sides, int id, int sign)
    {
        var (prefabToUse, numberOfDice) = sides != 100 ? (sides, 1) : (10, 2);

        var prefab = modelSelector.GetDiePrefab(modelSet, prefabToUse);

        if (prefab == null)
        {
            return null;
        }

        var result = new GameObject[numberOfDice];
        for (var i = 0; i < numberOfDice; ++i)
        {
            var die = Instantiate(prefab, _diceStartPosition, _diceStartOrientation);
            die.tag = "Dice";

            var dieScript = die.AddComponent<DieScript>();

            dieScript.Id = sides == 100 ? 100 + (id * 100) + i : id;
            dieScript.IsPrimary = sides != 100 || i == 0;
            dieScript.IsPercentile = sides == 100 && i == 0;
            dieScript.SetSign(sign);

            die.transform.localScale = sides == 100 && i == 0
                ? new Vector3(3F, 3F, 3F)
                : new Vector3(2.2F, 2.2F, 2.2F);

            result[i] = die;
        }

        if (sides == 100)
        {
            var dieScript = result[0].GetComponent<DieScript>();
            dieScript.SecondaryDice = result.Skip(1).ToArray();
        }

        return result;
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
        var total = values.Sum() + _throwConfiguration.Offset;

        var resultText = GetResultText(values, total);

        if (!_resultSent)
        {
            var serializedConfig = _throwConfiguration.ReturnFinalConfiguration
                ? JsonUtility.ToJson(new DieFinalConfigurationWrapper { Configuration = positions })
                : string.Empty;
#if !UNITY_EDITOR
            PropagateValue(_throwConfiguration.ThrowId, total, serializedConfig);
#endif
            _resultSent = true;
        }

        ToggleUI(true);

        var textBox = GameObject.Find("ThrowResult").GetComponent<UnityEngine.UI.Text>();
        textBox.text = resultText;

        var background = GameObject.Find("ThrowResultBackground").GetComponent<RectTransform>();

        switch (textBox.cachedTextGenerator.lineCount)
        {
            case 0:
            case 1:
                background.sizeDelta = new Vector2(background.sizeDelta.x, 60);
                break;
            case 2:
                background.sizeDelta = new Vector2(background.sizeDelta.x, 106);
                break;
        }
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
        var values = new List<int>();
        var finalPositions = new DieFinalConfiguration[dice.Length];

        var i = 0;
        foreach (var die in dice)
        {
            var dieScript = die.GetComponent<DieScript>();

            if (dieScript.IsPercentile)
            {
                // only use the primary dice to compute values
                // secondary dice will be used by their primaries
                values.Add(dieScript.GetValue());
            }

            if (_throwConfiguration.ReturnFinalConfiguration)
            {
                var dfc = new DieFinalConfiguration
                {
                    Id = dieScript.Id,
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

        return (values.ToArray(), finalPositions);
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
