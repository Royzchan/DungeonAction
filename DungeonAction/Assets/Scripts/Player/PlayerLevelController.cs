using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelController : MonoBehaviour
{
    public static PlayerLevelController Instance;

    private int _level = 1; // ���x��
    [SerializeField]
    private int _maxLevel = 99;
    private int _exp = 0; // �o���l
    [SerializeField]
    private int _maxExp = 999999;//�ő�o���l
    private int _skillPoint = 0; // �X�L���|�C���g

    //�X�L���|�C���g�ɂ���ĕύX�ł��鐔�l
    private float _hp = 100; // �ő�HP
    private float _attack = 15; // �U����
    private float _defense = 15; // �h���
    private float _skillPowerUpRatio = 1.5f; // �X�L���̌��ʔ{��
    private float _specialPowerUpRatio = 2.0f; // �K�E�Z�̌��ʔ{��

    [SerializeField, Header("���x���A�b�v���ɂ��炦��X�L���A�b�v�̃|�C���g")]
    private int _getSkillPoint = 5;
    private List<int> _levelUpEXP = new List<int>(); // ���x���A�b�v�ɕK�v�Ȍo���l

    #region �Q�b�^�[
    public int Level { get { return _level; } }
    public int EXP { get { return _exp; } }
    public int SkillPoint { get { return _skillPoint; } }
    public float HP { get { return _hp; } }
    public float Attack { get { return _attack; } }
    public float Defense { get { return _defense; } }
    public float SkillPowerUpRatio { get { return _skillPowerUpRatio; } }
    public float SpecialPowerUpRatio { get { return _specialPowerUpRatio; } }
    #endregion

    void Start()
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

        //���x���f�[�^�̓ǂݍ���
        TextAsset levelDataCSV = Resources.Load<TextAsset>("DungeonActionLevelData");
        if (levelDataCSV != null)
        {
            // �s���Ƃɕ���
            string[] lines = levelDataCSV.text.Split('\n');

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // ��s�𖳎�
                // �J���}��؂�Ńf�[�^�𕪊�
                string[] values = line.Split(',');
                _levelUpEXP.Add(int.Parse(values[1]));
            }
        }
        else
        {
            Debug.LogError("���x���f�[�^�̓ǂݍ��ݎ��s");
        }
    }

    //�Q�[���I�����ɌĂ΂��
    private void OnApplicationQuit()
    {
        //�����Ƀf�[�^�̕ۑ�������
        SaveData();
    }

    /// <summary>
    /// �o���l���l���������ɌĂяo��
    /// </summary>
    /// <param name="value">�l�������o���l</param>
    public void GetEXP(int value)
    {
        _exp += value;
        if (_exp >= _maxExp)
        {
            _exp = _maxExp;
            return;
        }
        LevelUpCheck();
    }

    /// <summary>
    /// ���x���A�b�v���Ă��邩�̊m�F
    /// </summary>
    public void LevelUpCheck()
    {
        //���X�g�����̊m�F
        if (_level - 1 < _levelUpEXP.Count)
        {
            if (_exp >= _levelUpEXP[_level - 1])
            {
                LevelUp();
                LevelUpCheck();
            }
        }
    }

    /// <summary>
    /// ���x���A�b�v�������ɌĂяo��
    /// </summary>
    public void LevelUp()
    {
        _level++;
        if (_level >= _maxLevel)
        {
            _level = _maxLevel;
            return;
        }
        GetSkillPoint();
    }

    /// <summary>
    /// �X�L���|�C���g�l������
    /// </summary>
    public void GetSkillPoint()
    {
        _skillPoint += _getSkillPoint;
    }

    /// <summary>
    /// HP�̒l���X�V
    /// </summary>
    /// <param name="value">�X�V���HP</param>
    public void SetHp(float value)
    {
        _hp = value;
    }

    /// <summary>
    /// �U���͂̒l���X�V
    /// </summary>
    /// <param name="value">�X�V��̍U����</param>
    public void SetAttack(float value)
    {
        _attack = value;
    }

    /// <summary>
    /// �h��͂̒l���X�V
    /// </summary>
    /// <param name="value">�X�V��̖h���</param>
    public void SetDefense(float value)
    {
        _defense = value;
    }

    /// <summary>
    /// �X�L���̌��ʔ{���̍X�V
    /// </summary>
    /// <param name="value">�X�V��̃X�L�����ʔ{��</param>
    public void SetSkillPowerUpRatio(float value)
    {
        _skillPowerUpRatio = value;
    }

    /// <summary>
    /// �K�E�Z�̌��ʔ{���̍X�V
    /// </summary>
    /// <param name="value">�X�V��̕K�E�Z���ʔ{��</param>
    public void SetSpecialPowerUpRatio(float value)
    {
        _specialPowerUpRatio = value;
    }

    /// <summary>
    /// �X�L���|�C���g�̒l�̍X�V
    /// </summary>
    /// <param name="value">�X�V��̃X�L���|�C���g</param>
    public void SetSkillPoint(int value)
    {
        _skillPoint = value;
    }

    /// <summary>
    /// �f�[�^�̃Z�[�u
    /// </summary>
    public void SaveData()
    {

    }
}
