using BoomAR.Utils;
using UnityEngine;

namespace BoomAR
{
    public class TutorialWindow : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            bool showTutorial = DidShowTutorial();
            switch (showTutorial)
            {
                case true:
                    SetTutorialAsShown();
                    break;
            }

            this.gameObject.SetActive(showTutorial);
        }

        private bool DidShowTutorial()
        {
            return !(PlayerPrefs.GetFloat(Constants.DidShowTutorial, 0f) == 1);
        }

        private void SetTutorialAsShown()
        {
            PlayerPrefs.SetFloat(Constants.DidShowTutorial, 1f);
        }
    }
}
