using BoomAR.Permission;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace BoomAR.Scenes
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Button _exitButton;
        [SerializeField] private ScenesList _sceneName;

        private PermissionController _permissionController;

        [Inject]
        private void Construct(PermissionController permissionController)
        {
            _permissionController = permissionController;
            _permissionController.PermissionWasGranted += LoadScenes;
        }

        private void Awake()
        {
            _button?.onClick.AddListener(LoadScenes);
            _exitButton?.onClick.AddListener(Exit);
        }

        public void LoadScenes()
        {
            if (_sceneName.ToString() != null)
                SceneManager.LoadScene(_sceneName.ToString(), LoadSceneMode.Single);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
