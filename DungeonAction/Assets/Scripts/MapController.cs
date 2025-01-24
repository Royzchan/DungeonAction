using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public Transform mapGridParent; // UI��Grid�̐e�I�u�W�F�N�g (Hierarchy��̋�̃I�u�W�F�N�g���w��)
    public GameObject wallTilePrefab;   // �ǃ^�C���̃v���n�u
    public GameObject floorTilePrefab;  // ���^�C���̃v���n�u
    public GameObject startTilePrefab;  // �X�^�[�g�����^�C���̃v���n�u
    public GameObject bossTilePrefab;   // �{�X�����^�C���̃v���n�u
    public GameObject playerIconPrefab; // �v���C���[�A�C�R���̃v���n�u

    private GameObject[,] mapTiles;     // �^�C����GameObject�z��
    private int[,] dungeonMap;          // �_���W�����}�b�v�̃f�[�^
    private Vector2Int mapSize;         // �}�b�v�S�̂̃T�C�Y (width, height)

    private GameObject playerIcon;      // �v���C���[��\���A�C�R��
    private float tileSize = 20f;       // UI���1�}�X�̃T�C�Y (�s�N�Z��)

    private Vector2 playerPosition;     // �v���C���[�̌��݈ʒu (�ڍׂȈʒu�w�肪�\)

    /// <summary>
    /// �}�b�v�����������Đ�������
    /// </summary>
    /// <param name="dungeonMap">�_���W�����}�b�v�̃f�[�^</param>
    public void InitializeMap(int[,] dungeonMap)
    {
        this.dungeonMap = dungeonMap;
        mapSize = new Vector2Int(dungeonMap.GetLength(0), dungeonMap.GetLength(1));

        // �^�C���I�u�W�F�N�g�z���������
        mapTiles = new GameObject[mapSize.x, mapSize.y];

        // Grid��UI�^�C���𓮓I����
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // �^�C��Prefab���擾
                GameObject tilePrefab = GetTilePrefab(dungeonMap[x, y]);
                GameObject tile = Instantiate(tilePrefab, mapGridParent);

                // �^�C���̃T�C�Y�ƈʒu��ݒ�
                RectTransform rt = tile.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileSize, tileSize);
                rt.anchoredPosition = new Vector2((x - mapSize.x / 2f) * tileSize, (y - mapSize.y / 2f) * tileSize);

                mapTiles[x, y] = tile; // �^�C����z��ɕۑ�
            }
        }

        // �v���C���[�A�C�R���𐶐�
        playerIcon = Instantiate(playerIconPrefab, mapGridParent);
        RectTransform playerRect = playerIcon.GetComponent<RectTransform>();
        playerRect.sizeDelta = new Vector2(tileSize, tileSize);

        // �e�̒��S�ʒu����
        CenterMapGrid();

        // �����v���C���[�ʒu��ݒ�
        playerPosition = Vector2.zero;
        UpdatePlayerPosition(playerPosition, Vector2.up);
    }

    /// <summary>
    /// Grid�S�̂̐e�I�u�W�F�N�g����ʂ̒��S�ɔz�u
    /// </summary>
    private void CenterMapGrid()
    {
        RectTransform gridParentRect = mapGridParent.GetComponent<RectTransform>();
        gridParentRect.anchoredPosition = Vector2.zero; // �e����ɉ�ʒ��S�ɔz�u�����
    }

    /// <summary>
    /// �}�b�v�̃^�C��Prefab���擾
    /// </summary>
    /// <param name="tileType">�^�C���^�C�v (0:��, 1:��, 2:�X�^�[�g����, 3:�{�X����)</param>
    /// <returns>�Ή�����Prefab</returns>
    private GameObject GetTilePrefab(int tileType)
    {
        switch (tileType)
        {
            case 1: return floorTilePrefab; // ��
            case 2: return startTilePrefab; // �X�^�[�g����
            case 3: return bossTilePrefab;  // �{�X����
            default: return wallTilePrefab; // ��
        }
    }

    /// <summary>
    /// �v���C���[�̌��݈ʒu�ƌ������X�V
    /// </summary>
    /// <param name="newPosition">�v���C���[�̐V�����ʒu (�ڍׂȍ��W�w��)</param>
    /// <param name="facingDirection">�v���C���[�������Ă������ (��: �� (0,1))</param>
    public void UpdatePlayerPosition(Vector2 newPosition, Vector2 facingDirection)
    {
        playerPosition = newPosition;

        // �v���C���[�A�C�R���̈ʒu���X�V (0,0 �𒆐S�Ƃ����ʒu�v�Z)
        RectTransform playerRect = playerIcon.GetComponent<RectTransform>();
        playerRect.anchoredPosition = new Vector2(playerPosition.x * tileSize, playerPosition.y * tileSize);

        // �v���C���[�A�C�R���̌������X�V
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
        playerRect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// �v���C���[�̌��ݍ��W���擾
    /// </summary>
    /// <returns>���݂̍��W (�ڍׂȈʒu)</returns>
    public Vector2 GetPlayerPosition()
    {
        return playerPosition;
    }
}
