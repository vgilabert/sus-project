using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public enum CartType
{
    Missile,
    Gatling
}

public class Train : MonoBehaviour
{
    List<GameObject> carts;
    
    [SerializeField] SplineComputer spline;

    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject menuCamera;
    
    GameObject locomotive;
    SplineFollower locomotiveFollower;
    [SerializeField] GameObject locomotivePrefab;
    [SerializeField] GameObject gatlingPrefab;
    [SerializeField] GameObject missilePrefab;
    
    [SerializeField] float speed;

    [SerializeField] GameObject InGamePanel;
    [SerializeField] GameObject PausePanel;
    bool isPaused = false;

    [SerializeField, Range(0,1)] float positionInPause;
    float lastPosition;
    
    void Start()
    {
        carts = new List<GameObject>();
        InitializeLocomotive();
    }
    
    void InitializeLocomotive()
    {
        locomotive = Instantiate(locomotivePrefab, transform);
        locomotiveFollower = locomotive.GetComponent<SplineFollower>();
        locomotive.transform.position = spline.EvaluatePosition(0);
        locomotiveFollower.spline = spline;
        locomotiveFollower.followSpeed = speed;
        carts.Add(locomotive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwitchMenus();
        }
    }

    void SwitchMenus()
    {
        isPaused = !isPaused;
        PausePanel.SetActive(!PausePanel.activeSelf);
        InGamePanel.SetActive(!InGamePanel.activeSelf);
        mainCamera.SetActive(!mainCamera.activeSelf);
        menuCamera.SetActive(!menuCamera.activeSelf);
        if (isPaused)
        {
            lastPosition = (float)locomotiveFollower.GetPercent();
            locomotiveFollower.SetPercent(positionInPause);
            locomotiveFollower.followSpeed = 0;
        }
        else
        {
            locomotiveFollower.SetPercent(lastPosition);
            locomotiveFollower.followSpeed = speed;
        }
    }

    public void AddCart(bool isGatling)
    {
        AddCart(isGatling ? CartType.Gatling : CartType.Missile);
    }
    
    
    void AddCart(CartType type)
    {
        GameObject cart = type switch
        {
            CartType.Missile => Instantiate(missilePrefab, transform),
            CartType.Gatling => Instantiate(gatlingPrefab, transform),
            _ => null
        };
        SplinePositioner splinePositioner = cart.GetComponent<SplinePositioner>();
        splinePositioner.spline = spline;
        splinePositioner.followTarget = carts[^1].GetComponent<SplineTracer>();
        splinePositioner.followTargetDistance = carts.Count == 1 ? 2 : 1.5f;
        carts.Add(cart);
    }
}