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

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Level", level.ToString() },
                { "EXP", exp.ToString() }
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

        PlayerLevelController.Instance.SetStatus(level, exp);
    }

    void OnDataRetrieveFailure(PlayFabError error)
    {
        Debug.LogError("���[�U�[�f�[�^�̎擾�Ɏ��s���܂���: " + error.GenerateErrorReport());
    }
}
