using UnityEngine;
using System.Collections.Generic;

namespace StarterAssets
{
    public class PlayerBuffSystem : MonoBehaviour
    {
        [System.Serializable]
        public class BuffStats
        {
            public float speedMultiplier = 1.5f;
            public float jumpMultiplier = 1.3f;
            public float pushStrengthMultiplier = 2f;
            public float duration = 60f;
        }

        [Header("Buff Configuration")]
        public BuffStats speedAndJumpBuff;
        public BuffStats strengthBuff;
        public bool debugMode = true;

        // References
        private FirstPersonController _controller;
        private BasicRigidBodyPush _rigidBodyPush;

        // Buff timers
        private float _speedJumpBuffTimer;
        private float _strengthBuffTimer;

        // Original values
        private float _originalMoveSpeed;
        private float _originalSprintSpeed;
        private float _originalJumpHeight;
        private float _originalPushStrength;
        private bool _originalCanPush;

        // UI style
        private GUIStyle _uiStyle;

        private void Awake()
        {
            InitializeComponents();
            StoreOriginalValues();
            SetupUI();
        }

        private void InitializeComponents()
        {
            _controller = GetComponent<FirstPersonController>();
            _rigidBodyPush = GetComponent<BasicRigidBodyPush>();

            if (debugMode)
            {
                Debug.Log("PlayerBuffSystem initialized");
                if (_controller == null) Debug.LogError("No FirstPersonController found!");
                if (_rigidBodyPush == null) Debug.LogError("No BasicRigidBodyPush found!");
            }
        }

        private void StoreOriginalValues()
        {
            _originalMoveSpeed = _controller.MoveSpeed;
            _originalSprintSpeed = _controller.SprintSpeed;
            _originalJumpHeight = _controller.JumpHeight;
            
            if (_rigidBodyPush != null)
            {
                _originalPushStrength = _rigidBodyPush.strength;
                _originalCanPush = _rigidBodyPush.canPush;
            }

            if (debugMode)
            {
                Debug.Log($"Original values stored - Speed: {_originalMoveSpeed}, Sprint: {_originalSprintSpeed}, Jump: {_originalJumpHeight}");
            }
        }

        private void SetupUI()
        {
            _uiStyle = new GUIStyle
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            _uiStyle.normal.textColor = Color.white;
        }

        private void Update()
        {
            UpdateBuffTimers();
        }

        private void UpdateBuffTimers()
        {
            // Speed and Jump buff
            if (_speedJumpBuffTimer > 0)
            {
                _speedJumpBuffTimer -= Time.deltaTime;
                if (_speedJumpBuffTimer <= 0)
                {
                    if (debugMode) Debug.Log("Speed/Jump buff expired");
                    ResetSpeedAndJumpBuff();
                }
            }

            // Strength buff
            if (_strengthBuffTimer > 0)
            {
                _strengthBuffTimer -= Time.deltaTime;
                if (_strengthBuffTimer <= 0)
                {
                    if (debugMode) Debug.Log("Strength buff expired");
                    ResetStrengthBuff();
                }
            }
        }

        private void OnGUI()
        {
            if (debugMode)
            {
                // Display active buffs in the top-right corner
                GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 100));
                if (_speedJumpBuffTimer > 0)
                {
                    GUILayout.Label($"Speed/Jump Buff: {_speedJumpBuffTimer:F1}s");
                }
                if (_strengthBuffTimer > 0)
                {
                    GUILayout.Label($"Strength Buff: {_strengthBuffTimer:F1}s");
                }
                GUILayout.EndArea();
            }
        }

        public void ActivateSpeedAndJumpBuff()
        {
            _speedJumpBuffTimer = speedAndJumpBuff.duration;
            
            // Apply buff multipliers
            _controller.MoveSpeed = _originalMoveSpeed * speedAndJumpBuff.speedMultiplier;
            _controller.SprintSpeed = _originalSprintSpeed * speedAndJumpBuff.speedMultiplier;
            _controller.JumpHeight = _originalJumpHeight * speedAndJumpBuff.jumpMultiplier;

            if (debugMode)
            {
                Debug.Log($"Speed/Jump buff activated for {speedAndJumpBuff.duration} seconds");
                Debug.Log($"New values - Speed: {_controller.MoveSpeed}, Sprint: {_controller.SprintSpeed}, Jump: {_controller.JumpHeight}");
            }
        }

        public void ActivateStrengthBuff()
        {
            _strengthBuffTimer = strengthBuff.duration;

            if (_rigidBodyPush != null)
            {
                _rigidBodyPush.canPush = true;
                _rigidBodyPush.strength = _originalPushStrength * strengthBuff.pushStrengthMultiplier;

                if (debugMode)
                {
                    Debug.Log($"Strength buff activated for {strengthBuff.duration} seconds");
                    Debug.Log($"New push strength: {_rigidBodyPush.strength}");
                }
            }
        }

        private void ResetSpeedAndJumpBuff()
        {
            _controller.MoveSpeed = _originalMoveSpeed;
            _controller.SprintSpeed = _originalSprintSpeed;
            _controller.JumpHeight = _originalJumpHeight;

            if (debugMode)
            {
                Debug.Log("Speed/Jump buff reset to original values");
            }
        }

        private void ResetStrengthBuff()
        {
            if (_rigidBodyPush != null)
            {
                _rigidBodyPush.canPush = _originalCanPush;
                _rigidBodyPush.strength = _originalPushStrength;

                if (debugMode)
                {
                    Debug.Log("Strength buff reset to original values");
                }
            }
        }

        // Public methods to get buff status
        public float GetSpeedBuffTimeRemaining() => _speedJumpBuffTimer;
        public float GetStrengthBuffTimeRemaining() => _strengthBuffTimer;
        public bool HasSpeedBuff() => _speedJumpBuffTimer > 0;
        public bool HasStrengthBuff() => _strengthBuffTimer > 0;
    }
}