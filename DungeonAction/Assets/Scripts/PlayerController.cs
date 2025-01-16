using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField]
    private float _maxHP = 100f;
    private float _hp;
    [SerializeField]
    private float _maxStamina;
    private float _stamina;
    [SerializeField]
    private float _attackPower = 100f;
    [SerializeField]
    private float _defenses = 100f;
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private InputAction _moveXAction;

    private Vector2 _moveDirection = Vector2.zero;

    private void OnEnable()
    {
        _moveXAction?.Enable();
    }

    private void OnDisable()
    {
        _moveXAction?.Disable();
    }



    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _moveDirection.x = _moveXAction.ReadValue<float>();
        Move();
    }

    //à⁄ìÆ
    private void Move()
    {
        _rb.velocity = new Vector3(_moveDirection.x * _speed, 0f, _moveDirection.y * _speed);
    }

    //çUåÇ
    private void Attack()
    {

    }

    //ÉXÉLÉã
    private void Skill()
    {

    }

    //ïKéEãZ
    private void Special()
    {

    }
}
