
/*
namespace SAGE.Framework.Core.UI
{
#if UNITY_EDITOR
    using UnityEditor.Animations;
    using UnityEditor;
#endif
    using Extensions;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using UnityEngine;

    public class ButtonTransition : MonoBehaviour
    {
        [SerializeField] private bool playSound = true;
        [SerializeField] private bool useAnimator = true;

        [Header("Sound")] [ShowIf("playSound")] [SerializeField]
        private string soundId = "Button";

        private ButtonHandler _buttonHandler = null;

        public ButtonHandler Button
        {
            get
            {
                if (_buttonHandler == null)
                {
                    _buttonHandler = GetComponent<ButtonHandler>();
                    _buttonHandler.onClick.AddListener(OnClick);
                }

                return _buttonHandler;
            }
        }

        private void Start()
        {
            if (_buttonHandler == null)
            {
                _buttonHandler = GetComponent<ButtonHandler>();
                _buttonHandler.onClick.AddListener(OnClick);
            }

#if UNITY_EDITOR
            OnValidate();
#endif
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            Button.transition = useAnimator ? Selectable.Transition.Animation : Selectable.Transition.ColorTint;

            if (useAnimator)
            {
                AnimatorController controller =
                    AssetDatabase.LoadAssetAtPath<AnimatorController>(
                        "Assets/QuickFramework/0. Animations/Button/Button.controller");
                if (controller == null) return;
                Animator animator = gameObject.GetOrAddComponent<Animator>();
                animator.runtimeAnimatorController = controller;
            }
            else
            {
                DestroyImmediate(GetComponent<Animator>());
            }
#endif
        }

        public void OnClick()
        {
            if (playSound)
            {
                AudioManager.Instance.PlaySound(soundId);
            }
        }
    }
}*/