using UnityEngine;

public class StartMatchConsole : MonoBehaviour , IInteractable
{
    [Header("Interaction Reactions")]
    [SerializeField] private Color _idleColor = Color.green;
    [SerializeField] private Color _pressedColor = Color.yellow;
    [SerializeField] private MeshRenderer _meshRenderer;

    private Material _objectMaterial;
    private void Awake()
    {
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        if (_meshRenderer != null)
        {
            _objectMaterial = _meshRenderer.material;
            
            SetColor(_idleColor);
        }
    }

    private void Start()
    {
        GameManager.Instance.OnRoundEnd += ResetColor;
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnRoundEnd -= ResetColor;
    }

    private void ResetColor()
    {
        SetColor(_idleColor);
    }

    private void SetColor(Color baseColor)
    {
        if (baseColor == null) return;
        if (_objectMaterial == null) return;

        _objectMaterial.SetColor("_BaseColor",baseColor);

    }

    public void Interact()
    {
        if (GameManager.Instance != null && !(GameManager.Instance.IsRoundActive || GameManager.Instance.IsRoundPreWarm))
        {
            GameManager.Instance.TryStartRound();

            if (_pressedColor != null)
            {
                SetColor(_pressedColor);
            }
        }
    }

    public string GetInteractText()
    {
        return GameManager.Instance.IsRoundActive || GameManager.Instance.IsRoundPreWarm ? "" : "START MATCH";
    }
}
