namespace SAGE.Framework.UI
{
    using UnityEngine;

    public interface IBackScreen { void BackToScreen(); }

    public interface IBaseScreen
    {
        void Initialize();
        void SetData(IUIData data = null);
        void Show();
        void Hide();
        RectTransform RectTransform { get; set; }
    }

    public interface IUIData {}
}