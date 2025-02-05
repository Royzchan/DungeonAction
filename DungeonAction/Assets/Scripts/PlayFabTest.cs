using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabTest : MonoBehaviour
{
    private string _customId;

    void Start()
    {
        // 以前のIDを取得、なければ新規作成
        if (PlayerPrefs.HasKey("PlayFab_CustomID"))
        {
            _customId = PlayerPrefs.GetString("PlayFab_CustomID");
        }
        else
        {
            _customId = System.Guid.NewGuid().ToString(); // 新規ID生成
            PlayerPrefs.SetString("PlayFab_CustomID", _customId);
            PlayerPrefs.Save();
        }

        // PlayFabにログイン
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                CustomId = "0000000000",
                CreateAccount = true
            },
            result => Debug.Log($"ログイン成功！ID: {_customId}"),
            error => Debug.Log($"ログイン失敗...(´；ω；｀) エラー: {error.ErrorMessage}")
        );
    }
}
