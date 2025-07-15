namespace SAGE.Framework.Core.UI
{
    using Extensions;
    using UnityEngine;

    public class BaseScreen : MonoBehaviour, IBaseScreen, IBackScreen
    {
        private RectTransform rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = gameObject.GetOrAddComponent<RectTransform>();
                }

                return rectTransform;
            }

            set => rectTransform = value;
        }

        public virtual void Initialize()
        {
        }

        public virtual void SetData(IUIData data = null)
        {
        }

        public virtual void Show()
        {
            UIManager.Instance.RegisterBackScreen(this);
            gameObject.SetSafeActive(true);
        }

        public virtual void Hide()
        {
            UIManager.Instance.UnregisterBackScreen(this);
            gameObject.SetSafeActive(false);
        }

        public virtual void BackToScreen()
        {
            Hide();
        }
    }
}