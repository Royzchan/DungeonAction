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

    //アプリ終了時の処理
    private void OnApplicationQuit()
    {
        //データの保存
        SaveUserStatus();
    }

    void LoginWithSavedOrNewCustomID()
    {
        string customId;

        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            customId = PlayerPrefs.GetString(PlayerPrefsKey);
            Debug.Log("既存のCustom IDを使用: " + customId);
        }
        else
        {
            customId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(PlayerPrefsKey, customId);
            PlayerPrefs.Save();
            Debug.Log("新しいCustom IDを作成: " + customId);
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
        Debug.Log("PlayFabログイン成功！");
        Debug.Log("PlayFab ID: " + result.PlayFabId);

        // ステータスを保存
        //SaveUserStatus();

        // ステータスを取得
        GetUserStatus();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFabログイン失敗: " + error.GenerateErrorReport());
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
        Debug.Log("ユーザーデータの保存に成功しました！");
    }

    void OnDataSaveFailure(PlayFabError error)
    {
        Debug.LogError("ユーザーデータの保存に失敗しました: " + error.GenerateErrorReport());
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
            Debug.Log("ユーザーデータが存在しません。");
            return;
        }

        if (result.Data.ContainsKey("Level"))
        {
            level = int.Parse(result.Data["Level"].Value);
            Debug.Log("保存されたレベル: " + result.Data["Level"].Value);
        }
        else
        {
            Debug.LogError("PlayFabにLevelが登録されていません。");
        }

        if (result.Data.ContainsKey("EXP"))
        {
            exp = int.Parse(result.Data["EXP"].Value);
            Debug.Log("保存された経験値: " + result.Data["EXP"].Value);
        }
        else
        {
            Debug.LogError("PlayFabにEXPが登録されていません。");
        }

        PlayerLevelController.Instance.SetStatus(level, exp);
    }

    void OnDataRetrieveFailure(PlayFabError error)
    {
        Debug.LogError("ユーザーデータの取得に失敗しました: " + error.GenerateErrorReport());
    }
}
