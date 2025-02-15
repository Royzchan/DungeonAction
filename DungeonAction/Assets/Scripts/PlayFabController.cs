using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController Instance;

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

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
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
        string customId;

        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            customId = PlayerPrefs.GetString(PlayerPrefsKey);
            Debug.Log("������Custom ID���g�p: " + customId);
        }
        else
        {
            customId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(PlayerPrefsKey, customId);
            PlayerPrefs.Save();
            Debug.Log("�V����Custom ID���쐬: " + customId);
        }

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
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab���O�C�����s: " + error.GenerateErrorReport());
    }

    public void SaveUserStatus()
    {
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
    }
}
