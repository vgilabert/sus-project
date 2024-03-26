using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spline
{
    public class SplineGenerator : MonoBehaviour
    {
        public Vector3 editorSplineStartPosition = Vector3.zero;

        public Vector3 editorSplineEndPosition = Vector3.right * 120;
        
        public float pointOffsetMax = 1.5f;
        
        public int count = 5;
        
        public bool autoUpdate = true;

        public List<Vector3> SplinesToPoints(List<SplineComputer> splines)
        {
            var points = new List<Vector3>();
            foreach (var spline in splines)
            {
                for (int i = 0; i < 100; i += 2)
                {
                    var percent = i / 100f;
                    points.Add(spline.EvaluatePosition(percent));
                }
            }
            return points;
        }
        
        public SplineComputer RandomSplineBetweenPoints(Vector3 startPos, Vector3 endPos)
        {
            var generatedSpline = new GameObject("GeneratedSpline(" + startPos + " to " + endPos + ")").AddComponent<SplineComputer>();
            
            var points = CreateRandomSplinePoints(startPos, endPos);
            
            generatedSpline.SetPoints(points);
            generatedSpline.type = Dreamteck.Splines.Spline.Type.Bezier;
            generatedSpline.Rebuild();
            generatedSpline.isNewlyCreated = false;
            generatedSpline.editorAlwaysDraw = true;
            return generatedSpline;
        }

        private SplinePoint[] CreateRandomSplinePoints(Vector3 startPos, Vector3 endPos)
        {
            var splinePoints = new SplinePoint[count];
            
            var lastPos = startPos;
            
            splinePoints[0] = new SplinePoint();
            splinePoints[0].position = startPos;
            
            for (int i = 1; i < count; i++)
            {
                var direction = (endPos - lastPos).normalized;
                var right = Vector3.Cross(direction, Vector3.up);
                var distance = Vector3.Distance(startPos, endPos) / count;
                var randomLateralOffset = Random.Range(-pointOffsetMax, pointOffsetMax) * distance;

                splinePoints[i] = new SplinePoint
                {
                    position = i != count-1 ? 
                        lastPos + direction * distance + right * randomLateralOffset :
                        endPos
                };
                var distanceToPrevious = Vector3.Distance(splinePoints[i].position, lastPos);
                splinePoints[i].normal = Vector3.up;
                splinePoints[i].tangent = splinePoints[i].position-direction * distanceToPrevious / 2;
                splinePoints[i].tangent2 = splinePoints[i].position+direction * distanceToPrevious / 2;
                lastPos = splinePoints[i].position;
            }

            return splinePoints;
        }
    }
}
