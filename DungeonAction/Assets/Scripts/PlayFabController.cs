using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController Instance;
    [SerializeField]
    private TMP_Text _playerIDText;
    [SerializeField]
    private GameObject _loadResultUI;
    [SerializeField]
    private TMP_Text _loadResultText;

    private TitleManager _titleManager;
    private bool _endLoadData = false; // �f�[�^��ǂݍ��ݏI����Ă��邩
    private const string PlayerPrefsKey = "PlayFabCustomID";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        _titleManager = FindAnyObjectByType<TitleManager>();
        if (_titleManager == null) Debug.LogError("TitleManager���o�^����Ă��܂���B");

        if (_loadResultUI != null)
        {
            _loadResultUI.SetActive(false);
        }
        else
        {
            Debug.LogError("���[�h���ʂ�UI���o�^����Ă��܂���B");
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _titleManager.enabled = false;
        LoginWithSavedOrNewCustomID();
    }

    //�A�v���I�����̏���
    private void OnApplicationQuit()
    {
        //�f�[�^�̕ۑ�
        SaveUserStatus();
    }

    void LoginWithSavedOrNewCustomID()
    {
        if (_loadResultUI != null)
        {
            _loadResultUI.SetActive(true);
        }
        else
        {
            Debug.LogError("���[�h���ʂ�UI���o�^����Ă��܂���B");
        }

        if (_loadResultText != null)
        {
            _loadResultText.text = "�f�[�^��ǂݍ��ݒ�...";
        }
        else
        {
            Debug.LogError("���[�h���ʂ̃e�L�X�g���o�^����Ă��܂���B");
        }

        string customId;

        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            customId = PlayerPrefs.GetString(PlayerPrefsKey);
            if (_playerIDText != null)
            {
                _playerIDText.text = "ID : " + customId;
            }
            else
            {
                Debug.LogError("ID�̃e�L�X�g��o�^���Ă��������B");
            }
            Debug.Log("������Custom ID���g�p: " + customId);
        }
        else
        {
            customId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(PlayerPrefsKey, customId);
            PlayerPrefs.Save();
            Debug.Log("�V����Custom ID���쐬: " + customId);
        }

        Debug.Log(customId);
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFab���O�C�������I");
        Debug.Log("PlayFab ID: " + result.PlayFabId);

        // �X�e�[�^�X��ۑ�
        //SaveUserStatus();

        // �X�e�[�^�X���擾
        GetUserStatus();
        _loadResultUI.SetActive(false);
        _titleManager.enabled = true;
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab���O�C�����s: " + error.GenerateErrorReport());
        _loadResultText.text = "�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B";
    }

    public void SaveUserStatus()
    {
        if (!_endLoadData) return;

        var level = PlayerLevelController.Instance.Level;
        var exp = PlayerLevelController.Instance.EXP;
        var maxSkillPoint = PlayerLevelController.Instance.MaxHaveSkillPoint;
        var skillPoint = PlayerLevelController.Instance.SkillPoint;
        var hp = PlayerLevelController.Instance.HP;
        var attack = PlayerLevelController.Instance.Attack;
        var defense = PlayerLevelController.Instance.Defense;
        var skillPowerUpRatio = PlayerLevelController.Instance.SkillPowerUpRatio;
        var specialPowerUpRatio = PlayerLevelController.Instance.SpecialPowerUpRatio;

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Level", level.ToString() },
                { "EXP", exp.ToString() },
                { "MaxSkillPoint", maxSkillPoint.ToString() },
                { "SkillPoint", skillPoint.ToString() },
                { "HP", hp.ToString() },
                { "Attack", attack.ToString() },
                { "Defense", defense.ToString() },
                { "SkillPowerUpRatio", skillPowerUpRatio.ToString() },
                { "SpecialPowerUpRatio", specialPowerUpRatio.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSaveSuccess, OnDataSaveFailure);
    }

    void OnDataSaveSuccess(UpdateUserDataResult result)
    {
        Debug.Log("���[�U�[�f�[�^�̕ۑ��ɐ������܂����I");
    }

    void OnDataSaveFailure(PlayFabError error)
    {
        Debug.LogError("���[�U�[�f�[�^�̕ۑ��Ɏ��s���܂���: " + error.GenerateErrorReport());
    }

    void GetUserStatus()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnDataRetrieveSuccess, OnDataRetrieveFailure);
    }

    void OnDataRetrieveSuccess(GetUserDataResult result)
    {
        int level = 1;
        int exp = 0;
        int maxSkillPoint = 0;
        int skillPoint = 0;
        float hp = PlayerLevelController.Instance.FirstHp;
        float attack = PlayerLevelController.Instance.FirstAttack;
        float defense = PlayerLevelController.Instance.FirstDefense;
        float skillPowerUpRatio = PlayerLevelController.Instance.FirstSkillPowerUpRatio;
        float specialPowerUpRatio = PlayerLevelController.Instance.FirstSpecialPowerUpRatio;

        if (result.Data == null || result.Data.Count == 0)
        {
            Debug.Log("���[�U�[�f�[�^�����݂��܂���B");
            return;
        }

        _endLoadData = true;

        if (result.Data.ContainsKey("Level"))
        {
            level = int.Parse(result.Data["Level"].Value);
            Debug.Log("�ۑ����ꂽ���x��: " + result.Data["Level"].Value);
        }
        else
        {
            Debug.LogError("PlayFab��Level���o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("EXP"))
        {
            exp = int.Parse(result.Data["EXP"].Value);
            Debug.Log("�ۑ����ꂽ�o���l: " + result.Data["EXP"].Value);
        }
        else
        {
            Debug.LogError("PlayFab��EXP���o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("MaxSkillPoint"))
        {
            maxSkillPoint = int.Parse(result.Data["MaxSkillPoint"].Value);
            Debug.Log("�ۑ����ꂽ�ő�X�L���|�C���g : " + result.Data["MaxSkillPoint"].Value);
        }
        else
        {
            Debug.LogError("PlayFab��MaxSkillPoint���o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("SkillPoint"))
        {
            skillPoint = int.Parse(result.Data["SkillPoint"].Value);
            Debug.Log("�ۑ����ꂽ�X�L���|�C���g : " + result.Data["SkillPoint"].Value);
        }
        else
        {
            Debug.LogError("PlayFab��SkillPoint���o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("HP"))
        {
            hp = float.Parse(result.Data["HP"].Value);
            Debug.Log("�ۑ����ꂽHP : " + result.Data["HP"].Value);
        }
        else
        {
            Debug.LogError("PlayFab��HP���o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("Attack"))
        {
            attack = float.Parse(result.Data["Attack"].Value);
            Debug.Log("�ۑ����ꂽ�U���� : " + result.Data["Attack"].Value);
        }
        else
        {
            Debug.LogError("PlayFab�ɍU���͂��o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("Defense"))
        {
            defense = float.Parse(result.Data["Defense"].Value);
            Debug.Log("�ۑ����ꂽ�h��� : " + result.Data["Defense"].Value);
        }
        else
        {
            Debug.LogError("PlayFab�ɖh��͂��o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("SkillPowerUpRatio"))
        {
            skillPowerUpRatio = float.Parse(result.Data["SkillPowerUpRatio"].Value);
            Debug.Log("�ۑ����ꂽ�X�L�����ʔ{�� : " + result.Data["SkillPowerUpRatio"].Value);
        }
        else
        {
            Debug.LogError("PlayFab�ɃX�L�����ʔ{�����o�^����Ă��܂���B");
        }

        if (result.Data.ContainsKey("SpecialPowerUpRatio"))
        {
            specialPowerUpRatio = float.Parse(result.Data["SpecialPowerUpRatio"].Value);
            Debug.Log("�ۑ����ꂽ�K�E�Z���ʔ{�� : " + result.Data["SpecialPowerUpRatio"].Value);
        }
        else
        {
            Debug.LogError("PlayFab�ɕK�E�Z���ʔ{�����o�^����Ă��܂���B");
        }

        PlayerLevelController.Instance.SetStatus(level, exp, maxSkillPoint, skillPoint,
            hp, attack, defense, skillPowerUpRatio, specialPowerUpRatio);
    }

    void OnDataRetrieveFailure(PlayFabError error)
    {
        Debug.LogError("���[�U�[�f�[�^�̎擾�Ɏ��s���܂���: " + error.GenerateErrorReport());

        // �l�b�g���[�N�G���[�̃`�F�b�N
        if (error.Error == PlayFabErrorCode.ConnectionError ||
            error.Error == PlayFabErrorCode.ServiceUnavailable)
        {
            Debug.LogError("�l�b�g���[�N�ڑ�������܂���B�C���^�[�l�b�g�ɐڑ����čĎ��s���Ă��������B");
            _loadResultText.text = "�l�b�g���[�N�G���[: �C���^�[�l�b�g�ڑ����m�F���Ă��������B";
        }
        else
        {
            _loadResultText.text = "�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B�G���[: " + error.ErrorMessage;
        }
    }
}
