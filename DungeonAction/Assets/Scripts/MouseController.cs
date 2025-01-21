using UnityEngine;

public class MouseController : MonoBehaviour
{
    void Start()
    {
        // �}�E�X�𒆉��ɌŒ肵�A��\���ɐݒ�
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Escape�L�[�Ń}�E�X�̃��b�N����������@�\�i�f�o�b�O�p�j
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // �ēx�}�E�X�𒆉��Œ肷��ꍇ�i�I�v�V�����j
        if (Input.GetMouseButtonDown(0)) // ���N���b�N�ōă��b�N
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}