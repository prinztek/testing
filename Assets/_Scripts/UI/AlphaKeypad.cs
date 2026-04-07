using UnityEngine;
using TMPro;
using System.Collections;

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

    public TMP_Text maxCharactersLabel;


    public void bQ()
    {
        if (inputField.text.Length >= 20) return;
        inputField.text += "Q";
    }
    public void bW()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "W";
    }
    public void bE()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "E";
    }
    public void bR()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "R";
    }
    public void bT()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "T";
    }
    public void bY()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "Y";
    }
    public void bU()
    {

        if (inputField.text.Length >= 20) return;

        inputField.text += "U";
    }
    public void bI()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "I";
    }
    public void bO()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "O";
    }
    public void bP()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "P";
    }
    public void bA()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "A";
    }
    public void bS()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "S";
    }
    public void bD()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "D";
    }
    public void bF()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "F";
    }
    public void bG()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "G";
    }
    public void bH()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "H";
    }
    public void bJ()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "J";
    }
    public void bK()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "K";
    }
    public void bL()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "L";
    }
    public void bZ()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "Z";
    }
    public void bX()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "X";
    }
    public void bC()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "C";
    }

    public void bV()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "V";
    }
    public void bB()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "B";
    }
    public void bN()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "N";
    }
    public void bM()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += "M";
    }

    public void bSpace()
    {
        if (inputField.text.Length >= 20) return;

        inputField.text += " ";
    }
    public void bBackspace()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public Vector3 dragScale = new Vector3(1.1f, 1.1f, 1.1f); // scale when dragging

    private IEnumerator MaximumCharactersReached()
    {
        if (inputField.text.Length >= 20)
        {
            // make the label text for Max of 20 characters appear for red for 0.5 seconds
            maxCharactersLabel.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            maxCharactersLabel.color = Color.white;
        }
    }
}
