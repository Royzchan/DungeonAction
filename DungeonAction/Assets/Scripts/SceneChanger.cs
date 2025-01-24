using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private string _sceneName;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();

        if (_button != null)
        {
            _button.onClick.AddListener(SceneChange);
        }
    }

    public void SceneChange()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
