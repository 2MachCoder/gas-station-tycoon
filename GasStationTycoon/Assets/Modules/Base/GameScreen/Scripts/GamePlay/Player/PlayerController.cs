using System;
using CodeBase.Core.GamePlay;
using CodeBase.CustomDIContainer;
using CodeBase.Systems.Gameplay;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Player
{
    public class PlayerController : BasePlayerController
    {
        [Inject] private CurrencySystem _currencySystem;
        
        private Vector3 _moveDirection;
        
        public float walkSpeed = 8f;
        
        public override void Initialize()
        {
            RootDiContainer.Instance.InjectMonoBehaviour(this);
            _currentMoney = _currencySystem.CurrentMoney;
            base.Initialize();
        }

        private void Update()
        {
            HandleInput();
            Move();
        }

        //TODO It is a terrible temporary solution. I'm integrating NewInputSystem  
        private void HandleInput()
        {
            float moveX = 0f;
            float moveZ = 0f;
            
            if (Input.GetKey(KeyCode.W)) moveZ = 1f;  
            if (Input.GetKey(KeyCode.S)) moveZ = -1f; 
            if (Input.GetKey(KeyCode.A)) moveX = -1f; 
            if (Input.GetKey(KeyCode.D)) moveX = 1f; 
            
            Vector3 localMoveDirection = new Vector3(moveX, 0f, moveZ).normalized;
            
            _moveDirection = transform.TransformDirection(localMoveDirection);
        }

        private void Move() => transform.position += _moveDirection * walkSpeed * Time.deltaTime;
    }
}