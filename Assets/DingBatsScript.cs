using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class DingBatsScript : MonoBehaviour
{
    [SerializeField] private KMBombInfo bombRef;
    [SerializeField] private KMAudio audioRef;

    [SerializeField] private KMSelectable[] buttons;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved = false;

    private int stage = 1;
    [SerializeField] private GameObject[] lights;
    [SerializeField] private Material green;
    [SerializeField] private Material red;

    private int[] charIndexes = new int[3];
    private string characters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ`~!@#$%^&*()-_=+[{]}|;:',<.>";

    void Awake()
    {
        //NEEDED, DONT TOUCH
        moduleId = moduleIdCounter++;

        //Gives each selectable object a function
        foreach (KMSelectable button in buttons)
        {
            button.OnInteract += delegate () { PressButton(button); return false; };
        }
    }

    void Start()
    {
        float scalar = transform.lossyScale.x;
        for (var i = 0; i < lights.Length; i++)
        {
            lights[i].GetComponent<Light>().range *= scalar;
        } 

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
            charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
            if (i == 1)
            {
                while (charIndexes[0] == charIndexes[1])
                {
                    buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                    charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                }
            }
            else if (i == 2)
            {
                while (charIndexes[0] == charIndexes[2] | charIndexes[1] == charIndexes[2])
                {
                    buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                    charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                }
            }
        }

        //Logging
        Debug.LogFormat("[DingBats #{0}] Current stage is 1. Button values from left to right are {1}, {2}, {3}.", moduleId, charIndexes[0], charIndexes[1], charIndexes[2]);
        if (charIndexes[0] + charIndexes[1] > 100)
        {
            Debug.LogFormat("[DingBats #{0}] {1} + {2} is greater than 100. Correct button to push is number 1", moduleId, charIndexes[0], charIndexes[1]);
        }
        else
        {
            Debug.LogFormat("[DingBats #{0}] {1} + {2} is not greater than 100.", moduleId, charIndexes[0], charIndexes[1]);
            if (charIndexes[0] == charIndexes.Min())
            {
                Debug.LogFormat("[DingBats #{0}] Button number 1 has the lowest value. Correct button to push is number 1", moduleId);
            }
            else if (charIndexes[1] == charIndexes.Min())
            {
                Debug.LogFormat("[DingBats #{0}] Button number 2 has the lowest value. Correct button to push is number 2", moduleId);
            }
            else
            {
                Debug.LogFormat("[DingBats #{0}] Button number 3 has the lowest value. Correct button to push is number 3", moduleId);
            }
        }
    }

    void StageChange(bool increase)
    {
        if (increase)
        {
            stage++;

            if (stage == 6)
            {
                moduleSolved = true;
                lights[4].GetComponent<MeshRenderer>().material = green;
                lights[4].GetComponentInChildren<Light>().color = Color.green;
                audioRef.PlaySoundAtTransform("PassSound", this.gameObject.transform);
                this.gameObject.GetComponent<KMBombModule>().HandlePass();
                return;
            }

            for (int j = 0; j < stage - 1; j++)
            {
                lights[j].GetComponent<MeshRenderer>().material = green;
                lights[j].GetComponentInChildren<Light>().color = Color.green;
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                if (i == 1)
                {
                    while (charIndexes[0] == charIndexes[1])
                    {
                        buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                        charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                    }
                }
                else if (i == 2)
                {
                    while (charIndexes[0] == charIndexes[2] | charIndexes[1] == charIndexes[2])
                    {
                        buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                        charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                    }
                }
            }

            //Logging after stage change
            Debug.LogFormat("[DingBats #{0}] Current stage is {1}. Button values from left to right are {2}, {3}, {4}.", moduleId, stage, charIndexes[0], charIndexes[1], charIndexes[2]);
            switch (stage)
            {
                case (2):
                    if (charIndexes[0] - charIndexes[2] > 40)
                    {
                        Debug.LogFormat("[DingBats #{0}] {1} - {2} is greater than 40. The correct button to push is number 2.", moduleId, charIndexes[0], charIndexes[2]);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] {1} - {2} is not greater than 40.", moduleId, charIndexes[0], charIndexes[2]);
                        if (charIndexes[0] == charIndexes.Max())
                        {
                            Debug.LogFormat("[DingBats #{0}] Button number 1 has the highest value. Correct button to push is number 1", moduleId);
                        }
                        else if (charIndexes[1] == charIndexes.Max())
                        {
                            Debug.LogFormat("[DingBats #{0}] Button number 2 has the highest value. Correct button to push is number 2", moduleId);
                        }
                        else
                        {
                            Debug.LogFormat("[DingBats #{0}] Button number 3 has the highest value. Correct button to push is number 3", moduleId);
                        }
                    }
                    break;
                case (3):
                    if (charIndexes[1] * charIndexes[2] > 2500)
                    {
                        Debug.LogFormat("[DingBats #{0}] {1} * {2} is greater than 2500. The correct button to push is number 3.", moduleId, charIndexes[0], charIndexes[2]);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] {1} * {2} is not greater than 2500.", moduleId, charIndexes[1], charIndexes[2]);
                        if (charIndexes[0] != charIndexes.Max() && charIndexes[0] != charIndexes.Min())
                        {
                            Debug.LogFormat("[DingBats #{0}] Button number 1 has neither the highest nor the lowest value. Correct button to push is number 1", moduleId);
                        }
                        else if (charIndexes[1] != charIndexes.Max() && charIndexes[1] != charIndexes.Min())
                        {
                            Debug.LogFormat("[DingBats #{0}] Button number 2 has neither the highest nor the lowest value. Correct button to push is number 2", moduleId);
                        }
                        else
                        {
                            Debug.LogFormat("[DingBats #{0}] Button number 3 has neither the highest nor the lowest value. Correct button to push is number 3", moduleId);
                        }
                    }
                    break;
                case (4):
                    if (charIndexes[0] == charIndexes.Min())
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 has the highest value. Correct button to push is number 1", moduleId);
                    }
                    else if (charIndexes[1] == charIndexes.Min())
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 has the highest value. Correct button to push is number 2", moduleId);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 has the highest value. Correct button to push is number 3", moduleId);
                    }
                    break;
                case (5):
                    if (charIndexes[0] == charIndexes.Max())
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 has the highest value. Correct button to push is number 1", moduleId);
                    }
                    else if (charIndexes[1] == charIndexes.Max())
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 has the highest value. Correct button to push is number 2", moduleId);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 has the highest value. Correct button to push is number 3", moduleId);
                    }
                    break;
            }
                
        }
        else
        {
            stage = 1;

            for (int j = 0; j < 5; j++)
            {
                lights[j].GetComponent<MeshRenderer>().material = red;
                lights[j].GetComponentInChildren<Light>().color = Color.red;
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                if (i == 1)
                {
                    while (charIndexes[0] == charIndexes[1])
                    {
                        buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                        charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                    }
                }
                else if (i == 2)
                {
                    while (charIndexes[0] == charIndexes[2] | charIndexes[1] == charIndexes[2])
                    {
                        buttons[i].GetComponentInChildren<TextMesh>().text = characters.ToArray()[UnityEngine.Random.Range(0, characters.ToCharArray().Length)].ToString();
                        charIndexes[i] = System.Array.IndexOf(characters.ToCharArray(), buttons[i].GetComponentInChildren<TextMesh>().text.ToCharArray()[0]);
                    }
                }
            }

            //Logging
            Debug.LogFormat("[DingBats #{0}] Current stage is 1. Button values from left to right are {1}, {2}, {3}.", moduleId, charIndexes[0], charIndexes[1], charIndexes[2]);
            if (charIndexes[0] + charIndexes[1] > 100)
            {
                Debug.LogFormat("[DingBats #{0}] {1} + {2} is greater than 100. Correct button to push is number 1", moduleId, charIndexes[0], charIndexes[1]);
            }
            else
            {
                Debug.LogFormat("[DingBats #{0}] {1} + {2} is not greater than 100.", moduleId, charIndexes[0], charIndexes[1]);
                if (charIndexes[0] == charIndexes.Min())
                {
                    Debug.LogFormat("[DingBats #{0}] Button number 1 has the lowest value. Correct button to push is number 1", moduleId);
                }
                else if (charIndexes[1] == charIndexes.Min())
                {
                    Debug.LogFormat("[DingBats #{0}] Button number 2 has the lowest value. Correct button to push is number 2", moduleId);
                }
                else
                {
                    Debug.LogFormat("[DingBats #{0}] Button number 3 has the lowest value. Correct button to push is number 3", moduleId);
                }
            }
        }
    }

    //When a button is pushed
    void PressButton(KMSelectable pressedButton)
    {
        if (moduleSolved)
        {
            return;
        }

        pressedButton.AddInteractionPunch(1f);
        audioRef.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, this.gameObject.transform);

        if (pressedButton.name == "Button1")
        {
            switch (stage)
            {
                case (1):
                    if (charIndexes[0] + charIndexes[1] > 100)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else if (charIndexes.Min() == charIndexes[0])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (2):
                    if (charIndexes[0] - charIndexes[2] > 40)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    else if (charIndexes.Max() == charIndexes[0])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (3):
                    if (charIndexes[1] * charIndexes[2] > 2500)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    else if (charIndexes.Min() != charIndexes[0] && charIndexes.Max() != charIndexes[0])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (4):
                    if (charIndexes.Min() == charIndexes[0])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (5):
                    if (charIndexes.Max() == charIndexes[0])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed correctly.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 1 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
            }  
        }
        else if (pressedButton.name == "Button2")
        {
            switch (stage)
            {
                case (1):
                    if (charIndexes[0] + charIndexes[1] > 100)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    else if (charIndexes.Min() == charIndexes[1])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (2):
                    if (charIndexes[0] - charIndexes[2] > 40)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else if (charIndexes.Max() == charIndexes[1])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (3):
                    if (charIndexes[1] * charIndexes[2] > 2500)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    else if (charIndexes.Min() != charIndexes[1] && charIndexes.Max() != charIndexes[1])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (4):
                    if (charIndexes.Min() == charIndexes[1])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (5):
                    if (charIndexes.Max() == charIndexes[1])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed correctly.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 2 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
            }
        }
        else if (pressedButton.name == "Button3")
        {
            switch (stage)
            {
                case (1):
                    if (charIndexes[0] + charIndexes[1] > 100)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    else if (charIndexes.Min() == charIndexes[2])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (2):
                    if (charIndexes[0] - charIndexes[2] > 40)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    else if (charIndexes.Max() == charIndexes[2])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (3):
                    if (charIndexes[1] * charIndexes[2] > 2500)
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else if (charIndexes.Min() != charIndexes[2] && charIndexes.Max() != charIndexes[2])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (4):
                    if (charIndexes.Min() == charIndexes[2])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed correctly, procceding to stage {1}.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
                case (5):
                    if (charIndexes.Max() == charIndexes[2])
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed correctly.", moduleId, stage + 1);
                        StageChange(true);
                    }
                    else
                    {
                        Debug.LogFormat("[DingBats #{0}] Button number 3 was pressed incorrectly, resetting module to stage 1.", moduleId);
                        this.gameObject.GetComponent<KMBombModule>().HandleStrike();
                        StageChange(false);
                    }
                    break;
            }
        }
    }
}
