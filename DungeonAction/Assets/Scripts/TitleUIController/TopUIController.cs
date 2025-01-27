using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUIController : TitleUIController
{
    enum SelectList
    {
        ModeSelect,
        Setting,
        GameEnd
    }

    public override void Decision()
    {
        switch (_nowSelect)
        {
            case (int)SelectList.ModeSelect:
                break;

            case (int)SelectList.Setting:
                break;

            case (int)SelectList.GameEnd:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
                break;
        }
    }
}
