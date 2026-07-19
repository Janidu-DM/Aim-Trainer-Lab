using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ImpactFX : MonoBehaviour
{
    [Header("Fallback Impact Color")]
    [SerializeField] private Color _fallbackColor = Color.orangeRed;


    private ParticleSystem _impactParticle;

    private void Awake()
    {
        _impactParticle = GetComponent<ParticleSystem>();
    }
    public void InitilizeImpact(RaycastHit hit , Action<ImpactFX> OnCompleteImpact )
    {
        Quaternion _impactRotation = Quaternion.LookRotation(hit.normal);
        transform.position = hit.point;
        transform.rotation = _impactRotation;

        var mainModule = _impactParticle.main;

        _impactParticle.Clear(true);
        mainModule.startColor = new ParticleSystem.MinMaxGradient(FindHitSurfaceColor(hit));
        StartCoroutine(ShowImpact(hit,OnCompleteImpact));
    }

    private IEnumerator ShowImpact(RaycastHit hit , Action<ImpactFX> OnCompleteImpactFx)
    {
        
        _impactParticle.Play();
        yield return new WaitForSeconds(_impactParticle.main.duration);
        OnCompleteImpactFx?.Invoke(this);
        
        
    }

    private Color FindHitSurfaceColor(RaycastHit hit)
    {
        Renderer surfaceRenderer = hit.collider.GetComponent<Renderer>();

        if (surfaceRenderer != null && surfaceRenderer.sharedMaterial != null)
        {
            if (surfaceRenderer.sharedMaterial.HasProperty("_BaseColor"))
            {
                return surfaceRenderer.sharedMaterial.GetColor("_BaseColor");
            }else if (surfaceRenderer.sharedMaterial.HasProperty("_Color"))
            {
                return surfaceRenderer.sharedMaterial.GetColor("_Color");

            }
        }

        return _fallbackColor;
    }
}
