using System;
using System.Collections;
using UnityEngine;

namespace Items
{
    public class AirStrikeFlow : ConsumableFlow
    {
        private GameObject crosshair;

        private void Start()
        {
            crosshair = transform.Find("Crosshair").gameObject;
            crosshair.SetActive(false);
        }

        public override void StartFlow(Action<bool> callback)
        {
            StartCoroutine(Flow(callback));
        }

        IEnumerator Flow(Action<bool> callback)
        {
            while (true)
            {
                DrawAimPoint();
            
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    StartCoroutine(DoStrike());
                    callback?.Invoke(true);
                    yield break;
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    crosshair.SetActive(false);
                    yield break;
                }
                yield return null;
            }
        }

        IEnumerator DoStrike()
        {
            yield return new WaitForSeconds(2);
            crosshair.SetActive(false);
        }
    
        void DrawAimPoint()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000))
            {
                crosshair.SetActive(true);
                crosshair.transform.position = hit.point;
            }
            else
            {
                crosshair.SetActive(false);
            }
        }
    }
}
