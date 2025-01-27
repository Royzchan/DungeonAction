using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIController : TitleUIController
{
    enum SelectList
    {
        Setting1,
        Setting2,
        CloseButton
    }

    public override void Decision()
    {
        switch (_nowSelect)
        {
            case (int)SelectList.Setting1:
                break;

            case (int)SelectList.Setting2:
                break;

            case (int)SelectList.CloseButton:
                _tm.GoTop();
                break;
        }
    }
}
