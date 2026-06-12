using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const string _idle = "Idle";
    const string _walk = "Walk";

    CustomActions _inputActions;
    NavMeshAgent _agent;
    Animator _animator;

    [SerializeField] ParticleSystem _pointClickEffect;
    [SerializeField] LayerMask _clickableLayer;

    [Header("Hover Settings")]
    [SerializeField] private Texture2D _normalCursor;
    [SerializeField] private Texture2D _interactCursor;
    [SerializeField] private LayerMask _interactableLayer;

    private GameObject _lastHoveredObject;

    float lookRotationSpeed = 5f; 
    private string _currentAnim = "";

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false; 

        _animator = GetComponent<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        _inputActions = new CustomActions();

        if (_pointClickEffect != null)
        {
            _pointClickEffect = Instantiate(_pointClickEffect);
            _pointClickEffect.Stop();
            _pointClickEffect.gameObject.SetActive(false);
        }

        InputHandler();
    }

    void InputHandler()
    {
        _inputActions.Prabu.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        if (GameStateManager.Instance == null) return;
        if (!GameStateManager.Instance.IsState(GameState.Exploration)) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (TeleportManager.Instance != null && TeleportManager.Instance.IsTeleporting) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _clickableLayer))
        {
            _agent.SetDestination(hit.point);
            SpawnClickEffect(hit.point);
        }
    }

    void SpawnClickEffect(Vector3 position)
    {
        if (_pointClickEffect == null) return;

        _pointClickEffect.gameObject.SetActive(true);
        _pointClickEffect.transform.position = position + new Vector3(0, 0.1f, 0);
        _pointClickEffect.Stop();
        _pointClickEffect.Play();
    }

    void OnEnable()
    {
        _inputActions.Enable();
    }

    void OnDisable()
    {
        _inputActions.Disable();
    }

    void Update()
    {
        FaceTarget();
        SetAnimations();
        HandleHover();
    }

    void HandleHover()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ResetHover();
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _interactableLayer))
        {
            Cursor.SetCursor(_interactCursor, Vector2.zero, CursorMode.Auto);
            _lastHoveredObject = hit.collider.gameObject;
        }
        else
        {
            ResetHover();
        }
    }

    void ResetHover()
    {
        Cursor.SetCursor(_normalCursor, Vector2.zero, CursorMode.Auto);
        _lastHoveredObject = null;
    }

    void FaceTarget()
    {
        if (!_agent.isOnNavMesh) return;
        if (_agent.remainingDistance <= _agent.stoppingDistance) return;

        Vector3 direction = (_agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    void SetAnimations()
    {
        string targetAnim = (_agent.velocity.sqrMagnitude < 0.1f) ? _idle : _walk;

        if (_currentAnim == targetAnim) return;

        _currentAnim = targetAnim;
        _animator.Play(targetAnim);
    }
}
