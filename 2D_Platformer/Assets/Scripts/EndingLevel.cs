using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingLevel : MonoBehaviour
{
    [SerializeField]
    public string levelname;

    [SerializeField]
    private EndingTrigger color1Trigger;

    [SerializeField]
    private EndingTrigger color2Trigger;

    float playersReady = 0;

    int playersInTrigger = 0;

    private void Start()
    {
        color1Trigger.OnEntered.AddListener(WhiteEntered);
        color1Trigger.OnExited.AddListener(WhiteExited);
        color2Trigger.OnEntered.AddListener(BlackEntered);
        color2Trigger.OnExited.AddListener(BlackExited);

    }

    private void WhiteEntered(ColoredCharacter character)
    {
        if(character.color == ElementColor.Color1)
        {
            playersReady++;
            CheckEnd();
        }
    }

    private void WhiteExited(ColoredCharacter character)
    {
        if (character.color == ElementColor.Color1)
        {
            playersReady--;
            CheckEnd();
        }
    }

    private void BlackEntered(ColoredCharacter character)
    {
        if (character.color == ElementColor.Color2)
        {
            playersReady++;
            CheckEnd();
        }
    }

    private void BlackExited(ColoredCharacter character)
    {
        if (character.color == ElementColor.Color2)
        {
            playersReady--;
            CheckEnd();
        }
    }

    private void CheckEnd()
    {
        if(playersReady >= 2)
        {
            SceneManager.LoadScene(levelname);
        }
    }
}
