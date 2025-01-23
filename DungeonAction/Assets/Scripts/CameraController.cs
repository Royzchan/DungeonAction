using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject _player; // �v���C���[�i�^�[�Q�b�g�j�ƂȂ�I�u�W�F�N�g
    private PlayerController _playerController;
    private GameManager _gm;
    public float _distance = 5.0f; // �^�[�Q�b�g����̋���
    public float _rotationSpeed = 100.0f; // ��]���x
    public Vector2 _pitchLimits = new Vector2(-30, 60); // �㉺�̊p�x����
    public Vector3 _playerOffset = new Vector3(0, 1.5f, 0); // �v���C���[�ʒu�����p�̃I�t�Z�b�g

    private float _currentYaw = 0.0f; // ���݂̐����p�x
    private float _currentPitch = 0.0f; // ���݂̐����p�x

    [SerializeField]
    private InputAction _cameraRotateAction;

    private void OnEnable()
    {
        _cameraRotateAction?.Enable();
    }

    private void OnDisable()
    {
        _cameraRotateAction?.Disable();
    }

    void Start()
    {
        if (_player == null)
        {
            Debug.LogError("Target is not assigned! Please assign a target for the camera.");
        }
        _playerController = _player.GetComponent<PlayerController>();
        _gm = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (_player == null) return;
        if (!_playerController.Alive) return;
        if (!_gm.GamePlaying) return;

        // �}�E�X���͂���p�x���v�Z
        float mouseX = _cameraRotateAction.ReadValue<Vector2>().x * _rotationSpeed * Time.deltaTime;
        float mouseY = _cameraRotateAction.ReadValue<Vector2>().y * _rotationSpeed * Time.deltaTime;

        _currentYaw += mouseX;
        _currentPitch -= mouseY;

        // �����p�x�ɐ�����������
        _currentPitch = Mathf.Clamp(_currentPitch, _pitchLimits.x, _pitchLimits.y);

        // �J�����̈ʒu�Ɖ�]���X�V
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // �v���C���[�̒��S�ʒu�ɃI�t�Z�b�g��ǉ�
        Vector3 targetPosition = _player.transform.position + _playerOffset;

        // �J�����̗��z�I�Ȉʒu���v�Z
        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
        Vector3 desiredOffset = rotation * new Vector3(0, 0, -_distance);
        Vector3 desiredCameraPosition = targetPosition + desiredOffset;

        // Raycast���g�p���ăJ�����ƃ^�[�Q�b�g�Ԃ̏�Q�������o
        RaycastHit hit;
        if (Physics.Raycast(targetPosition, desiredOffset.normalized, out hit, _distance))
        {
            // ��Q��������ꍇ�A�J��������Q���̎�O�ɔz�u
            transform.position = hit.point - desiredOffset.normalized * 0.1f; // ������O�ɂ��炷
        }
        else
        {
            // ��Q�����Ȃ��ꍇ�A���z�I�Ȉʒu�ɔz�u
            transform.position = desiredCameraPosition;
        }

        // �J�������^�[�Q�b�g�������悤�ɐݒ�
        transform.LookAt(targetPosition);
    }
}
