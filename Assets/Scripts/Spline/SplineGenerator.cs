using Dreamteck.Splines;
using UnityEngine;

namespace Spline
{
    public class SplineGenerator : MonoBehaviour
    {
        public Vector3 startPosition = Vector3.zero;

        public Vector3 endPosition = Vector3.right * 120;
        
        public float pointOffsetMax = 1.5f;
        
        public int count = 5;
        
        public SplineComputer editorPreview;

        public bool autoUpdate = true;


        public SplineComputer RandomSplineBetweenPoints(Vector3 startPos, Vector3 endPos)
        {
            var generatedSpline = new GameObject("GeneratedSpline").AddComponent<SplineComputer>();
            
            var points = CreateRandomSplinePoints(startPos, endPos);
            
            generatedSpline.SetPoints(points);
            generatedSpline.type = Dreamteck.Splines.Spline.Type.Bezier;
            generatedSpline.Rebuild();
            generatedSpline.isNewlyCreated = false;
            generatedSpline.editorAlwaysDraw = true;
            return generatedSpline;
        }
        
        public void EditorRandomSplineBetweenPoints(SplineComputer obj, Vector3 startPos, Vector3 endPos)
        {
            var generatedSpline = obj;
            
            var points = CreateRandomSplinePoints(startPos, endPos);
            
            generatedSpline.SetPoints(points);
            generatedSpline.type = Dreamteck.Splines.Spline.Type.Bezier;
            generatedSpline.Rebuild();
            generatedSpline.isNewlyCreated = false;
            generatedSpline.editorAlwaysDraw = true;
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
                        endPosition
                };
                var distanceToPrevious = Vector3.Distance(splinePoints[i].position, lastPos);
                splinePoints[i].normal = Vector3.up;
                splinePoints[i].tangent = splinePoints[i].position-direction * distanceToPrevious / 2;
                splinePoints[i].tangent2 = splinePoints[i].position+direction * distanceToPrevious / 2;
                lastPos = splinePoints[i].position;
            }

            return splinePoints;
        }
        
        public void DrawSplinesInEditor()
        {
            EditorRandomSplineBetweenPoints(editorPreview, startPosition, endPosition);
        }
    }
}
