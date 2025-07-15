namespace SAGE.Framework.UI
{
    using Extensions;
    using System.Collections.Generic;
    using UnityEngine;

    public class FlyTextManager : BehaviorSingleton<FlyTextManager>
    {
        [SerializeField] private FlyText _flyTextPrefab;
        private Queue<FlyText> _flyTextQueue = new Queue<FlyText>();

        public void ShowFlyText(string text, Color color)
        {
            FlyText flyText = GetFlyText();
            if (flyText == null)
            {
                flyText = Instantiate(_flyTextPrefab, transform);
            }

            flyText.ShowFlyText(text, color);
        }

        private FlyText GetFlyText()
        {
            if (_flyTextQueue.Count > 0)
            {
                return _flyTextQueue.Dequeue();
            }

            return null;
        }

        public void ReturnFlyText(FlyText flyText)
        {
            _flyTextQueue.Enqueue(flyText);
        }
    }
}