using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelectUIController : TitleUIController
{
    enum SelectList
    {
        NormalMode,
        EndlessMode,
        CloseButton
    }

    public override void Decision()
    {
        switch (_nowSelect)
        {
            case (int)SelectList.NormalMode:
                _tm.GameStart_Normal();
                break;

            case (int)SelectList.EndlessMode:
                _tm.GameStartEndless();
                break;

            case (int)SelectList.CloseButton:
                _tm.GoTop();
                break;
        }
    }
}
