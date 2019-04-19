using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MathBoard : MonoBehaviour
{
    [SerializeField] Text _numberText = null;
    [SerializeField] Text _targetNumberText = null;
    [SerializeField] Button[] _numberButtons = null;
    [SerializeField] Button[] _opperatorButtons = null;
    [SerializeField] Text[] _answerTexts = null;

    int answerIndex = 0;

    int sumNum = 0;

    string lastOpp = "";


    // Use this for initialization
    void Start()
    {
        for(int i = 0; i < _numberButtons.Length; ++i)
        {
            int b = i;
            NumberButtonClick(b);
        }

        for (int i = 0; i < _opperatorButtons.Length; ++i)
        {
            int b = i;
            OppButtonClick(b);
        }
    }

    void NumberButtonClick(int index)
    {
        if (answerIndex % 2 != 0)
            return;

        int num = index + 1;

        if (lastOpp == "")
            sumNum += num;

        if (lastOpp == "+")
            sumNum += num;

        if (lastOpp == "-")
            sumNum -= num;

        if (lastOpp == "*")
            sumNum *= num;

        answerIndex++;
    }

    void OppButtonClick(int index)
    {
        if (answerIndex % 2 == 0)
            return;

        string opp = "";
        if (index == 0)
            opp = "+";
        if (index == 1)
            opp = "-";
        if (index == 2)
            opp = "*";

        answerIndex++;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
