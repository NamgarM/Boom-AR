using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWindow : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //PlayerPrefs.SetFloat("DidShowTutorial", 0f);
        bool showTutorial = DidShowTutorial();
        switch(showTutorial)
        {
            case true:
                SetTutorialAsShown();
                break;
        }

        this.gameObject.SetActive(showTutorial);

    }

    private bool DidShowTutorial()
    {
        return !(PlayerPrefs.GetFloat("DidShowTutorial", 0f) == 1);
    }

    private void SetTutorialAsShown()
    {
        PlayerPrefs.SetFloat("DidShowTutorial", 1f);
    }
}
