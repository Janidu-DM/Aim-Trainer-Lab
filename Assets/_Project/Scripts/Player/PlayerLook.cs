using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [SerializeField] private float _baseMouseSensitivity = 15f;
    [SerializeField] private float _maxPitch = 85f;
    [SerializeField] private float _minPitch = -85f;

    [Header("Reference")]
    [SerializeField] private Transform _cameraHolder;

    private float xRotation = 0f;

    
    public void Look(Vector2 lookInput)
    {
        float currentMouseSensitivity = _baseMouseSensitivity * GameManager.Instance.SensetivityMultiplier;
        float mouseX = lookInput.x * currentMouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * currentMouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, _minPitch, _maxPitch);
        _cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
    public void InjectRecoil(float recoil_X , float recoil_Y)
    {
        xRotation = Mathf.Lerp(xRotation,xRotation-recoil_X,10 * Time.deltaTime);
        xRotation = Mathf.Clamp(xRotation, _minPitch, _maxPitch);
        _cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
