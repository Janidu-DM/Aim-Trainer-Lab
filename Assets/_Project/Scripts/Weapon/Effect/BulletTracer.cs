using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BulletTracer : MonoBehaviour
{
    [Header("Bullet Tracer Settings")]
    [SerializeField] private float _tracerSpeed = 100f;

    private TrailRenderer _trail;

    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
    }

    public void InitiateFlight(Vector3 startPos , Vector3 endPos , Action<BulletTracer> OnCompleteCallBack)
    {
        _trail.Clear();

        StartCoroutine(FlightRoutine(startPos, endPos, OnCompleteCallBack));
    }

    private IEnumerator FlightRoutine(Vector3 startPos , Vector3 endPos , Action<BulletTracer> OnComplete) 
    {
        transform.position = startPos;
        float _distance = Vector3.Distance(startPos, endPos);
        float _remainingDistance = _distance;
        _trail.emitting = true;

        while (_remainingDistance < 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, _tracerSpeed * Time.deltaTime);
            _remainingDistance = Vector3.Distance(transform.position, endPos);
            yield return null;
        }
        transform.position = endPos;
        

        yield return new WaitForSeconds(_trail.time);
        _trail.emitting = false;
        OnComplete?.Invoke(this);
        
    }
}
