using System.Reflection;
using DG.Tweening;

namespace Utils
{
    public static class DotweenExtensions
    {
        public static Sequence AppendEx(this Sequence sequence, Tweener tweener)
        {
            var onStartField = typeof(Tweener)
                .GetField("onStart", BindingFlags.NonPublic | BindingFlags.Instance);

            var onStart = onStartField.GetValue(tweener) as TweenCallback;
            if (onStart != null)
            {
                sequence.AppendCallback(onStart);
                onStartField.SetValue(tweener, null);
            }

            sequence.Append(tweener);

            if (tweener.onComplete != null)
            {
                sequence.AppendCallback(tweener.onComplete);
                tweener.onComplete = null;
            }

            return sequence;
        }
    }
}