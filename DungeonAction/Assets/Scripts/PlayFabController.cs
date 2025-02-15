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
        int maxSkillPoint = 0;
        int skillPoint = 0;
        float hp = PlayerLevelController.Instance.FirstHp;
        float attack = PlayerLevelController.Instance.FirstAttack;
        float defense = PlayerLevelController.Instance.FirstDefense;
        float skillPowerUpRatio = PlayerLevelController.Instance.FirstSkillPowerUpRatio;
        float specialPowerUpRatio = PlayerLevelController.Instance.FirstSpecialPowerUpRatio;

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

        if (result.Data.ContainsKey("MaxSkillPoint"))
        {
            maxSkillPoint = int.Parse(result.Data["MaxSkillPoint"].Value);
            Debug.Log("保存された最大スキルポイント : " + result.Data["MaxSkillPoint"].Value);
        }
        else
        {
            Debug.LogError("PlayFabにMaxSkillPointが登録されていません。");
        }

        if (result.Data.ContainsKey("SkillPoint"))
        {
            skillPoint = int.Parse(result.Data["SkillPoint"].Value);
            Debug.Log("保存されたスキルポイント : " + result.Data["SkillPoint"].Value);
        }
        else
        {
            Debug.LogError("PlayFabにSkillPointが登録されていません。");
        }

        if (result.Data.ContainsKey("HP"))
        {
            hp = float.Parse(result.Data["HP"].Value);
            Debug.Log("保存されたHP : " + result.Data["HP"].Value);
        }
        else
        {
            Debug.LogError("PlayFabにHPが登録されていません。");
        }

        if (result.Data.ContainsKey("Attack"))
        {
            attack = float.Parse(result.Data["Attack"].Value);
            Debug.Log("保存された攻撃力 : " + result.Data["Attack"].Value);
        }
        else
        {
            Debug.LogError("PlayFabに攻撃力が登録されていません。");
        }

        if (result.Data.ContainsKey("Defense"))
        {
            defense = float.Parse(result.Data["Defense"].Value);
            Debug.Log("保存された防御力 : " + result.Data["Defense"].Value);
        }
        else
        {
            Debug.LogError("PlayFabに防御力が登録されていません。");
        }

        if (result.Data.ContainsKey("SkillPowerUpRatio"))
        {
            skillPowerUpRatio = float.Parse(result.Data["SkillPowerUpRatio"].Value);
            Debug.Log("保存されたスキル効果倍率 : " + result.Data["SkillPowerUpRatio"].Value);
        }
        else
        {
            Debug.LogError("PlayFabにスキル効果倍率が登録されていません。");
        }

        if (result.Data.ContainsKey("SpecialPowerUpRatio"))
        {
            specialPowerUpRatio = float.Parse(result.Data["SpecialPowerUpRatio"].Value);
            Debug.Log("保存された必殺技効果倍率 : " + result.Data["SpecialPowerUpRatio"].Value);
        }
        else
        {
            Debug.LogError("PlayFabに必殺技効果倍率が登録されていません。");
        }

        PlayerLevelController.Instance.SetStatus(level, exp, maxSkillPoint, skillPoint,
            hp, attack, defense, skillPowerUpRatio, specialPowerUpRatio);
    }

    void OnDataRetrieveFailure(PlayFabError error)
    {
        Debug.LogError("ユーザーデータの取得に失敗しました: " + error.GenerateErrorReport());
    }
}
