using System;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Player Interaction Settings")]
    [SerializeField] private float _interactRange = 2.0f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private Transform _playerCameraTransform;

    public event Action<String> OnInteractableHovered;
    public void TryInteract()
    {
        if (_playerCameraTransform == null)
        {
            return;
        }
        if (Physics.Raycast(_playerCameraTransform.position, _playerCameraTransform.forward, out RaycastHit hit, _interactRange, _interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    public void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsRoundActive || GameManager.Instance.IsRoundPreWarm)
        {
            OnInteractableHovered?.Invoke(null);
            return ;
        }

        if (Physics.Raycast(_playerCameraTransform.position,_playerCameraTransform.forward,out RaycastHit hit, _interactRange, _interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                OnInteractableHovered?.Invoke(interactable.GetInteractText());
                return;
            }
        }
        OnInteractableHovered?.Invoke(null);
    }

}
