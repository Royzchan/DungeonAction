using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelController : MonoBehaviour
{
    public static PlayerLevelController Instance;

    private int _level = 1; // レベル
    [SerializeField]
    private int _maxLevel = 99;
    private int _exp = 0; // 経験値
    [SerializeField]
    private int _maxExp = 999999;//最大経験値  
    private int _skillPoint = 10; // スキルポイント
    private int _maxHaveSkillPoint = 10; // 現状持てる最大のスキルポイント数

    //各ステータスの初期ステータス
    private float _firstHp = 100;
    private float _firstAttack = 100;
    private float _firstDefense = 100;
    private float _firstSkillPowerUpRatio = 1.5f;
    private float _firstSpecialPowerUpRatio = 2.0f;

    //スキルポイントによって変更できる数値
    private float _hp = 100; // 最大HP
    private float _attack = 100; // 攻撃力
    private float _defense = 100; // 防御力
    private float _skillPowerUpRatio = 1.5f; // スキルの効果倍率
    private float _specialPowerUpRatio = 2.0f; // 必殺技の効果倍率

    [SerializeField, Header("レベルアップ時にもらえるスキルアップのポイント")]
    private int _getSkillPoint = 5;
    private List<int> _levelUpEXP = new List<int>(); // レベルアップに必要な経験値

    private float _hpUpValue = 15; // 1回で強化されるHPの量
    private float _attackUpValue = 15; // 1回で強化される攻撃の量
    private float _defenseUpValue = 15; // 1回で強化される防御力の量
    private float _skillPowerUpValue = 0.1f; // 1回で強化されるスキル効果倍率の量
    private float _specialPowerUpValue = 0.1f; // 1回で強化される必殺技の効果倍率の量

    #region ゲッター
    public int Level { get { return _level; } }
    public int EXP { get { return _exp; } }
    public int SkillPoint { get { return _skillPoint; } }
    public int MaxHaveSkillPoint { get { return _maxHaveSkillPoint; } }
    public float FirstHp { get { return _firstHp; } }
    public float FirstAttack { get { return _firstAttack; } }
    public float FirstDefense { get { return _firstDefense; } }
    public float FirstSkillPowerUpRatio { get { return _firstSkillPowerUpRatio; } }
    public float FirstSpecialPowerUpRatio { get { return _firstSpecialPowerUpRatio; } }
    public float HP { get { return _hp; } }
    public float Attack { get { return _attack; } }
    public float Defense { get { return _defense; } }
    public float SkillPowerUpRatio { get { return _skillPowerUpRatio; } }
    public float SpecialPowerUpRatio { get { return _specialPowerUpRatio; } }
    public float HPUpValue { get { return _hpUpValue; } }
    public float AttackUpValue { get { return _attackUpValue; } }
    public float DefenseUpValue { get { return _defenseUpValue; } }
    public float SkillPowerUpValue { get { return _skillPowerUpValue; } }
    public float SpecialPowerUpValue { get { return _specialPowerUpValue; } }
    #endregion

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

        //レベルデータの読み込み
        TextAsset levelDataCSV = Resources.Load<TextAsset>("DungeonActionLevelData");
        if (levelDataCSV != null)
        {
            // 行ごとに分割
            string[] lines = levelDataCSV.text.Split('\n');

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // 空行を無視
                // カンマ区切りでデータを分割
                string[] values = line.Split(',');
                _levelUpEXP.Add(int.Parse(values[1]));
            }
        }
        else
        {
            Debug.LogError("レベルデータの読み込み失敗");
        }
    }

    void Start()
    {

    }

    //ゲーム終了時に呼ばれる
    private void OnApplicationQuit()
    {
        //ここにデータの保存を書く
        SaveData();
    }

    /// <summary>
    /// 経験値を獲得した時に呼び出す
    /// </summary>
    /// <param name="value">獲得した経験値</param>
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
    /// レベルアップしているかの確認
    /// </summary>
    public void LevelUpCheck()
    {
        //リスト内かの確認
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
    /// レベルアップした時に呼び出す
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
    /// スキルポイント獲得処理
    /// </summary>
    public void GetSkillPoint()
    {
        _skillPoint += _getSkillPoint;
        _maxHaveSkillPoint += _getSkillPoint;
    }

    /// <summary>
    /// HPの値を更新
    /// </summary>
    /// <param name="value">更新後のHP</param>
    public void SetHp(float value)
    {
        _hp = value;
    }

    /// <summary>
    /// 攻撃力の値を更新
    /// </summary>
    /// <param name="value">更新後の攻撃力</param>
    public void SetAttack(float value)
    {
        _attack = value;
    }

    /// <summary>
    /// 防御力の値を更新
    /// </summary>
    /// <param name="value">更新後の防御力</param>
    public void SetDefense(float value)
    {
        _defense = value;
    }

    /// <summary>
    /// スキルの効果倍率の更新
    /// </summary>
    /// <param name="value">更新後のスキル効果倍率</param>
    public void SetSkillPowerUpRatio(float value)
    {
        _skillPowerUpRatio = value;
    }

    /// <summary>
    /// 必殺技の効果倍率の更新
    /// </summary>
    /// <param name="value">更新後の必殺技効果倍率</param>
    public void SetSpecialPowerUpRatio(float value)
    {
        _specialPowerUpRatio = value;
    }

    /// <summary>
    /// スキルポイントの値の更新
    /// </summary>
    /// <param name="value">更新後のスキルポイント</param>
    public void SetSkillPoint(int value)
    {
        _skillPoint = value;
    }

    /// <summary>
    /// データのセーブ
    /// </summary>
    public void SaveData()
    {

    }
}
