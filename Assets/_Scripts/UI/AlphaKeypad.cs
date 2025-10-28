using UnityEngine;
using TMPro;

public class AlphaKeypad : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject buttonQ;
    public GameObject buttonW;
    public GameObject buttonE;
    public GameObject buttonR;
    public GameObject buttonT;
    public GameObject buttonY;
    public GameObject buttonU;
    public GameObject buttonI;
    public GameObject buttonO;
    public GameObject buttonP;
    public GameObject buttonA;
    public GameObject buttonS;
    public GameObject buttonD;
    public GameObject buttonF;
    public GameObject buttonG;
    public GameObject buttonH;
    public GameObject buttonJ;
    public GameObject buttonK;
    public GameObject buttonL;
    public GameObject buttonZ;
    public GameObject buttonX;
    public GameObject buttonC;
    public GameObject buttonV;
    public GameObject buttonB;
    public GameObject buttonN;
    public GameObject buttonM;
    public GameObject buttonSpace;
    public GameObject buttonBackspace;

    public void bQ()
    {
        inputField.text += "Q";
    }
    public void bW()
    {
        inputField.text += "W";
    }
    public void bE()
    {
        inputField.text += "E";
    }
    public void bR()
    {
        inputField.text += "R";
    }
    public void bT()
    {
        inputField.text += "T";
    }
    public void bY()
    {
        inputField.text += "Y";
    }
    public void bU()
    {
        inputField.text += "U";
    }
    public void bI()
    {
        inputField.text += "I";
    }
    public void bO()
    {
        inputField.text += "O";
    }
    public void bP()
    {
        inputField.text += "P";
    }
    public void bA()
    {
        inputField.text += "A";
    }
    public void bS()
    {
        inputField.text += "S";
    }
    public void bD()
    {
        inputField.text += "D";
    }
    public void bF()
    {
        inputField.text += "F";
    }
    public void bG()
    {
        inputField.text += "G";
    }
    public void bH()
    {
        inputField.text += "H";
    }
    public void bJ()
    {
        inputField.text += "J";
    }
    public void bK()
    {
        inputField.text += "K";
    }
    public void bL()
    {
        inputField.text += "L";
    }
    public void bZ()
    {
        inputField.text += "Z";
    }
    public void bX()
    {
        inputField.text += "X";
    }
    public void bC()
    {
        inputField.text += "C";
    }

    public void bV()
    {
        inputField.text += "V";
    }
    public void bB()
    {
        inputField.text += "B";
    }
    public void bN()
    {
        inputField.text += "N";
    }
    public void bM()
    {
        inputField.text += "M";
    }

    public void bSpace()
    {
        inputField.text += " ";
    }
    public void bBackspace()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
