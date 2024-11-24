using UnityEngine;
using Cinemachine;

namespace StarterAssets
{
    public class PossessionSystem : MonoBehaviour
    {
        [Header("Possession Settings")]
        public float possessionDuration = 30f;
        public LayerMask possessableLayers;
        public float possessionRange = 5f;
        public KeyCode possessionKey = KeyCode.F;
        public bool showPossessionUI = true;
        public CinemachineVirtualCamera playerCamera;

        [Header("Debug")]
        public bool debugMode = true;

        private FirstPersonController _controller;
        private StarterAssetsInputs _input;
        private GameObject _originalPlayer;
        private Possessable _possessedObject;
        private Transform _originalCameraTarget;
        private Vector3 _originalPlayerPosition;
        private Quaternion _originalPlayerRotation;
        private bool _isPossessing;
        private bool _hasPossessionBuff;
        private float _possessionTimer;
        private GUIStyle _uiStyle;

        private void Awake()
        {
            InitializeComponents();
            SetupUI();
        }

        private void InitializeComponents()
        {
            _controller = GetComponent<FirstPersonController>();
            _input = GetComponent<StarterAssetsInputs>();
            _originalPlayer = gameObject;

            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }

            if (playerCamera != null)
            {
                _originalCameraTarget = playerCamera.Follow;
                if (debugMode) Debug.Log("Camera reference found");
            }
            else
            {
                Debug.LogError("No camera reference! Please assign PlayerFollowCamera in the inspector.");
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
            if (debugMode && Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"F pressed. Has buff: {_hasPossessionBuff}, Is possessing: {_isPossessing}");
            }

            // Check for early exit from possession
            if (_isPossessing && Input.GetKeyDown(possessionKey))
            {
                if (debugMode) Debug.Log("Ending possession early");
                EndPossession();
                return;
            }

            // If not possessing, try to start possession
            if (!_isPossessing && _hasPossessionBuff && Input.GetKeyDown(possessionKey))
            {
                if (debugMode) Debug.Log("Attempting possession...");
                TryPossessObject();
            }

            if (_isPossessing)
            {
                UpdatePossession();
            }

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            if (_hasPossessionBuff)
            {
                _possessionTimer -= Time.deltaTime;
                if (_possessionTimer <= 0)
                {
                    if (_isPossessing)
                    {
                        EndPossession();
                    }
                    _hasPossessionBuff = false;
                }
            }
        }

        private void OnGUI()
        {
            if (debugMode)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 100));
                GUILayout.Label($"Has Possession Buff: {_hasPossessionBuff}");
                GUILayout.Label($"Possession Timer: {_possessionTimer:F1}");
                GUILayout.Label($"Is Possessing: {_isPossessing}");
                GUILayout.EndArea();
            }

            if (showPossessionUI && _hasPossessionBuff && !_isPossessing)
            {
                RaycastHit hit;
                bool canPossess = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, possessionRange, possessableLayers);

                string message = canPossess 
                    ? $"Press {possessionKey} to possess target" 
                    : "Look at a possessable object";

                Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height - 100);
                GUI.Label(new Rect(screenCenter.x - 100, screenCenter.y, 200, 30), message, _uiStyle);

                string timeMessage = $"Possession Buff: {_possessionTimer:F1}s";
                GUI.Label(new Rect(screenCenter.x - 100, screenCenter.y - 30, 200, 30), timeMessage, _uiStyle);
            }
        }

        public void ActivatePossessionBuff()
        {
            _hasPossessionBuff = true;
            _possessionTimer = possessionDuration;
            if (debugMode) Debug.Log("Possession buff activated!");
        }

        private void TryPossessObject()
        {
            if (_isPossessing) return;

            RaycastHit hit;
            Vector3 rayStart = Camera.main.transform.position;
            Vector3 rayDirection = Camera.main.transform.forward;

            if (debugMode)
            {
                Debug.DrawRay(rayStart, rayDirection * possessionRange, Color.red, 2f);
                Debug.Log($"Casting ray with range {possessionRange} and layer mask {possessableLayers.value}");
            }

            if (Physics.Raycast(rayStart, rayDirection, out hit, possessionRange, possessableLayers))
            {
                if (debugMode) Debug.Log($"Hit object: {hit.collider.gameObject.name}");

                var possessable = hit.collider.GetComponent<Possessable>();
                if (possessable != null)
                {
                    if (debugMode) Debug.Log("Found possessable component, starting possession");
                    StartPossession(possessable);
                }
            }
        }

        private void StartPossession(Possessable target)
        {
            if (target == null) return;
            
            _possessedObject = target;
            _isPossessing = true;

            // Store original player transform info
            _originalPlayerPosition = transform.position;
            _originalPlayerRotation = transform.rotation;

            // Disable player controller
            if (_controller != null)
            {
                _controller.enabled = false;
            }

            // Update camera
            if (playerCamera != null)
            {
                // Only set Follow, remove LookAt
                playerCamera.Follow = target.fpsCameraPoint;
                playerCamera.LookAt = null;
                
                // Set the camera to use POV mode
                var composer = playerCamera.GetCinemachineComponent<CinemachineComposer>();
                if (composer != null)
                {
                    DestroyImmediate(composer);
                }
                
                var pov = playerCamera.GetCinemachineComponent<CinemachinePOV>();
                if (pov == null)
                {
                    pov = playerCamera.AddCinemachineComponent<CinemachinePOV>();
                }
                
                // Reset POV settings
                pov.m_VerticalAxis.Value = 0;
                pov.m_HorizontalAxis.Value = 0;
            }

            // Hide possessed object mesh (optional)
            var renderers = target.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }

            if (debugMode) Debug.Log($"Started possessing {target.gameObject.name}");
        }

        private void UpdatePossession()
        {
            if (_possessedObject != null && _input != null)
            {
                _possessedObject.HandleMovement(_input.move, _input.look);
            }
        }

        private void EndPossession()
        {
            if (debugMode) Debug.Log("Ending possession");
            
            _isPossessing = false;
            _hasPossessionBuff = false;

            // Restore player position and control
            transform.position = _originalPlayerPosition;
            transform.rotation = _originalPlayerRotation;
            if (_controller != null)
            {
                _controller.enabled = true;
            }

            // Reset camera
            if (playerCamera != null)
            {
                playerCamera.Follow = _originalCameraTarget;
                playerCamera.LookAt = _originalCameraTarget;
                
                // Remove POV and restore original behavior
                var pov = playerCamera.GetCinemachineComponent<CinemachinePOV>();
                if (pov != null)
                {
                    DestroyImmediate(pov);
                }
            }

            // Reset possessed object
            if (_possessedObject != null)
            {
                var renderers = _possessedObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
                _possessedObject = null;
            }
        }

        public float GetPossessionTimeRemaining() => _possessionTimer;
        public bool IsPossessing() => _isPossessing;
        public bool HasPossessionBuff() => _hasPossessionBuff;
    }
}