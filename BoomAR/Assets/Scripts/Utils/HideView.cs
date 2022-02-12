using UnityEngine;
using UnityEngine.UI;

public class HideView : MonoBehaviour
{
    [SerializeField] private GameObject _view;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(Close);
    }

    public void Close()
    {
        _view.SetActive(false);
    }
}
