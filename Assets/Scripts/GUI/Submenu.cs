using UnityEngine;
using System.Collections;

public class Submenu : MonoBehaviour 
{
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

    public const int buttonsCount = 0;

    public GameObject[] buttons = new GameObject[buttonsCount];

    private EButton[] buttonTypes = new EButton[buttonsCount];

    public int activeIndex = 0;

	void Start () 
    {
	    for(int i = 0; i < buttonsCount; ++i)
        {
            EButton button = EButton.startgame;

            switch(buttons[i].name)
            {
                case "startgameSprite": button = EButton.startgame ; break;
                case "joingameSprite" : button = EButton.joingame  ; break;
                case "exitSprite"     : button = EButton.exit      ; break;
                case "racingSprite"   : button = EButton.racing    ; break;
                case "pursuitSprite"  : button = EButton.pursuit   ; break;
                case "cancelSprite"   : button = EButton.cancel    ; break;
                case "mainmenuSprite" : button = EButton.mainmenu  ; break;
                case "startnewSprite" : button = EButton.startnew  ; break;
                case "nextSprite"     : button = EButton.next      ; break;
            }

            buttonTypes[i] = button;
        }
	}

    public GameObject GetButton(int index = 0)
    {
        activeIndex = index;
        return buttons[activeIndex];
    }

    public GameObject GetNext()
    {
        ++activeIndex;
        activeIndex %= buttons.Length;

        return buttons[activeIndex];
    }

    public GameObject GetPrev()
    {
        if(activeIndex == 0)
        {
            activeIndex = buttons.Length;
        }
        --activeIndex;

        return buttons[activeIndex];
    }
}
