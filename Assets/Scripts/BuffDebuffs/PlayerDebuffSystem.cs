// PlayerDebuffSystem.cs
using UnityEngine;
using StarterAssets;

namespace StarterAssets
{
    public class PlayerDebuffSystem : MonoBehaviour
    {
        [System.Serializable]
        public class DebuffStats
        {
            public float speedMultiplier = 0.7f; // 30% slowdown
            public float jumpSpeedMultiplier = 0.5f; // 50% jump speed reduction
            public float scaleMultiplier = 0.5f; // 50% size reduction
            public float jumpHeightMultiplier = 0.5f; // 50% jump height reduction
            public float duration = 15f;
        }

        [Header("Debuff Configuration")]
        public DebuffStats slowDebuff;
        public DebuffStats shrinkDebuff;
        public bool debugMode = true;

        // References
        private FirstPersonController _controller;
        private CharacterController _characterController;

        // Debuff timers
        private float _slowDebuffTimer;
        private float _shrinkDebuffTimer;

        // Original values
        private float _originalMoveSpeed;
        private float _originalSprintSpeed;
        private float _originalJumpHeight;
        private float _originalGravity;
        private Vector3 _originalScale;

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
            _characterController = GetComponent<CharacterController>();

            if (debugMode)
            {
                Debug.Log("PlayerDebuffSystem initialized");
                if (_controller == null) Debug.LogError("No FirstPersonController found!");
                if (_characterController == null) Debug.LogError("No CharacterController found!");
            }
        }

        private void StoreOriginalValues()
        {
            _originalMoveSpeed = _controller.MoveSpeed;
            _originalSprintSpeed = _controller.SprintSpeed;
            _originalJumpHeight = _controller.JumpHeight;
            _originalGravity = _controller.Gravity;
            _originalScale = transform.localScale;

            if (debugMode)
            {
                Debug.Log($"Original values stored - Speed: {_originalMoveSpeed}, Sprint: {_originalSprintSpeed}, " +
                         $"Jump Height: {_originalJumpHeight}, Gravity: {_originalGravity}, Scale: {_originalScale}");
            }
        }

        private void SetupUI()
        {
            _uiStyle = new GUIStyle
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            _uiStyle.normal.textColor = Color.red; // Red color for debuffs
        }

        private void Update()
        {
            UpdateDebuffTimers();
        }

        private void UpdateDebuffTimers()
        {
            // Slow debuff
            if (_slowDebuffTimer > 0)
            {
                _slowDebuffTimer -= Time.deltaTime;
                if (_slowDebuffTimer <= 0)
                {
                    if (debugMode) Debug.Log("Slow debuff expired");
                    ResetSlowDebuff();
                }
            }

            // Shrink debuff
            if (_shrinkDebuffTimer > 0)
            {
                _shrinkDebuffTimer -= Time.deltaTime;
                if (_shrinkDebuffTimer <= 0)
                {
                    if (debugMode) Debug.Log("Shrink debuff expired");
                    ResetShrinkDebuff();
                }
            }
        }

        private void OnGUI()
        {
            if (_slowDebuffTimer > 0 || _shrinkDebuffTimer > 0)
            {
                // Display active debuffs in the top-right corner
                GUILayout.BeginArea(new Rect(Screen.width - 250, 110, 240, 100));
                
                if (_slowDebuffTimer > 0)
                {
                    GUILayout.Label($"Slowed: {_slowDebuffTimer:F1}s", _uiStyle);
                }
                if (_shrinkDebuffTimer > 0)
                {
                    GUILayout.Label($"Shrunk: {_shrinkDebuffTimer:F1}s", _uiStyle);
                }
                
                GUILayout.EndArea();
            }
        }

        public void ActivateSlowDebuff()
        {
            _slowDebuffTimer = slowDebuff.duration;
            
            // Apply slow and jump speed debuff
            _controller.MoveSpeed = _originalMoveSpeed * slowDebuff.speedMultiplier;
            _controller.SprintSpeed = _originalSprintSpeed * slowDebuff.speedMultiplier;
            
            // Reduce jump speed by modifying gravity (stronger gravity = slower jumps)
            _controller.Gravity = _originalGravity * (1f / slowDebuff.jumpSpeedMultiplier); // Inverse multiplier for gravity

            if (debugMode)
            {
                Debug.Log($"Slow debuff activated for {slowDebuff.duration} seconds");
                Debug.Log($"Reduced values - Speed: {_controller.MoveSpeed}, Sprint: {_controller.SprintSpeed}, " +
                         $"Gravity: {_controller.Gravity}");
            }
        }

        public void ActivateShrinkDebuff()
        {
            _shrinkDebuffTimer = shrinkDebuff.duration;

            // Store the current CharacterController settings
            float currentHeight = _characterController.height;
            float currentRadius = _characterController.radius;
            
            // Disable CharacterController temporarily
            _characterController.enabled = false;
            
            // Apply scale change
            transform.localScale = _originalScale * shrinkDebuff.scaleMultiplier;
            
            // Update CharacterController dimensions
            _characterController.height = currentHeight * shrinkDebuff.scaleMultiplier;
            _characterController.radius = currentRadius * shrinkDebuff.scaleMultiplier;
            
            // Reduce jump height
            _controller.JumpHeight = _originalJumpHeight * shrinkDebuff.jumpHeightMultiplier;
            
            // Re-enable CharacterController
            _characterController.enabled = true;

            if (debugMode)
            {
                Debug.Log($"Shrink debuff activated for {shrinkDebuff.duration} seconds");
                Debug.Log($"New scale: {transform.localScale}, New jump height: {_controller.JumpHeight}");
            }
        }

        private void ResetSlowDebuff()
        {
            _controller.MoveSpeed = _originalMoveSpeed;
            _controller.SprintSpeed = _originalSprintSpeed;
            _controller.Gravity = _originalGravity;

            if (debugMode)
            {
                Debug.Log("Speed and jump speed reset to original values");
            }
        }

        private void ResetShrinkDebuff()
        {
            // Disable CharacterController temporarily
            _characterController.enabled = false;
            
            // Reset scale
            transform.localScale = _originalScale;
            
            // Reset CharacterController dimensions
            _characterController.height = 2f; // Default height
            _characterController.radius = 0.5f; // Default radius
            
            // Reset jump height
            _controller.JumpHeight = _originalJumpHeight;
            
            // Re-enable CharacterController
            _characterController.enabled = true;

            if (debugMode)
            {
                Debug.Log("Scale and jump height reset to original values");
            }
        }

        // Public methods to get debuff status
        public float GetSlowDebuffTimeRemaining() => _slowDebuffTimer;
        public float GetShrinkDebuffTimeRemaining() => _shrinkDebuffTimer;
        public bool HasSlowDebuff() => _slowDebuffTimer > 0;
        public bool HasShrinkDebuff() => _shrinkDebuffTimer > 0;
    }
}