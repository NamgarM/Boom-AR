using BoomAR.Permission.Infrastructure;
using BoomAR.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using Zenject;

public class PermissionPopUp : MonoBehaviour
{
    [SerializeField] private Button _permissionButton;
    [SerializeField] private TextMeshProUGUI _text;

    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void OnEnable()
    {
        _permissionButton.onClick.AddListener(CheckPermission);
        SetPermissionDescription(Constants.AllowPermission);
    }

    private void OnDisable()
    {
        _permissionButton.onClick.RemoveListener(CheckPermission);
    }

    private void SetPermissionDescription(string allowPermission)
    {
        _text.text = allowPermission;
    }

    private void CheckPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);
        else
            _signalBus.Fire(new PermissionSignals
                .PermissionWasGrantedSignal());
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            SetPermissionDescription(Constants.PermissionWasNotGranted);
        else
            _signalBus.Fire(new PermissionSignals
                .PermissionWasGrantedSignal());
    }
}
