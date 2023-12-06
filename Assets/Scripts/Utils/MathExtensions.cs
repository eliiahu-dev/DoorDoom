using UnityEngine;

namespace Utils
{
    public static class MathExtensions
    {
        public static Vector3 GetBezierUpLerp(Vector3 startPoint, Vector3 endPoint, float upDistance, float t)
        {
            var upVector = Vector3.up * upDistance;
            return GetBezierLerp(startPoint, startPoint + upVector, endPoint + upVector, endPoint, t);
        }
        
        public static Vector3 GetBezierLerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return (((-p0 + 3*(p1-p2) + p3)* t + (3*(p0+p2) - 6*p1))* t + 3*(p1-p0))* t + p0;
        }
    }
}