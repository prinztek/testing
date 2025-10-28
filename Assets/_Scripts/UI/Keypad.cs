using TMPro;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    public GameObject button5;
    public GameObject button6;
    public GameObject button7;
    public GameObject button8;
    public GameObject button9;
    public GameObject button0;
    public GameObject buttonClear;
    public GameObject buttonPeriod;


    public void b1()
    {
        Debug.Log("Button 1 pressed");
        inputField.text += "1";
    }

    public void b2()
    {
        Debug.Log("Button 2 pressed");
        inputField.text += "2";
    }

    public void b3()
    {
        Debug.Log("Button 3 pressed");
        inputField.text += "3";
    }

    public void b4()
    {
        Debug.Log("Button 4 pressed");
        inputField.text += "4";
    }

    public void b5()
    {
        Debug.Log("Button 5 pressed");
        inputField.text += "5";
    }

    public void b6()
    {
        Debug.Log("Button 6 pressed");
        inputField.text += "6";
    }
    public void b7()
    {
        Debug.Log("Button 7 pressed");
        inputField.text += "7";
    }
    public void b8()
    {
        Debug.Log("Button 8 pressed");
        inputField.text += "8";
    }
    public void b9()
    {
        Debug.Log("Button 9 pressed");
        inputField.text += "9";
    }
    public void b0()
    {
        Debug.Log("Button 0 pressed");
        inputField.text += "0";
    }
    public void bClear()
    {
        Debug.Log("Button Clear pressed");
        inputField.text = "";
    }
    public void bPeriod()
    {
        Debug.Log("Button Period pressed");
        inputField.text += ".";
    }
}
