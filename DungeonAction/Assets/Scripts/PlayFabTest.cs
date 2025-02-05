using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabTest : MonoBehaviour
{
    private string _customId;

    void Start()
    {
        // �ȑO��ID���擾�A�Ȃ���ΐV�K�쐬
        if (PlayerPrefs.HasKey("PlayFab_CustomID"))
        {
            _customId = PlayerPrefs.GetString("PlayFab_CustomID");
        }
        else
        {
            _customId = System.Guid.NewGuid().ToString(); // �V�KID����
            PlayerPrefs.SetString("PlayFab_CustomID", _customId);
            PlayerPrefs.Save();
        }

        // PlayFab�Ƀ��O�C��
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                CustomId = "0000000000",
                CreateAccount = true
            },
            result => Debug.Log($"���O�C�������IID: {_customId}"),
            error => Debug.Log($"���O�C�����s...(�L�G�ցG�M) �G���[: {error.ErrorMessage}")
        );
    }
}
