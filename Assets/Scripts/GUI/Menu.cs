using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{
    public enum EType
    {
        main,
        gamemodeSelection,
        ingame,
        popupWin,
        popupLost,
        none,
    }

    public enum EButton
    {
        startgame,
        joingame,
        exit,
        racing,
        pursuit,
        cancel,
        mainmenu,
        startnew,
        next,
    }

    public EType type;

    public GameObject[] buttons;
    public EButton[] buttonTypes;

    public int selectedButtonIndex = 0;

    public EType Type
    {
        get { return type; }
    }

    public EButton CurrentButton
    {
        get { return buttonTypes[selectedButtonIndex]; }
    }

	void Start () 
    {

	}

    public EButton GetButton(int index = 0)
    {
        buttons[selectedButtonIndex].renderer.material.color = Color.white;        
        selectedButtonIndex = index;
        buttons[selectedButtonIndex].renderer.material.color = Color.green;
        
        return buttonTypes[selectedButtonIndex];
    }

    public EButton GetNext()
    {
        buttons[selectedButtonIndex].renderer.material.color = Color.white;
        ++selectedButtonIndex;
        selectedButtonIndex %= buttons.Length;
        buttons[selectedButtonIndex].renderer.material.color = Color.green;
        
        return buttonTypes[selectedButtonIndex];
    }

    public EButton GetPrev()
    {
        buttons[selectedButtonIndex].renderer.material.color = Color.white;
        if(selectedButtonIndex == 0)
        {
            selectedButtonIndex = buttons.Length;
        }
        --selectedButtonIndex;
        buttons[selectedButtonIndex].renderer.material.color = Color.green;
        
        return buttonTypes[selectedButtonIndex];
    }

    public void ClearButtonColors()
    {
        foreach(GameObject button in buttons)
        {
            button.renderer.material.color = Color.white;
        }
    }
}
