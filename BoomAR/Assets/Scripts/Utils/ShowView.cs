using UnityEngine;
using UnityEngine.UI;

public class ShowView : MonoBehaviour
{
    [SerializeField] private GameObject _view;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(Show);
    }

    public void Show()
    {
        _view.SetActive(true);
    }
}
