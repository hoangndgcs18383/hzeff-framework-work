
/*
namespace SAGE.Framework.Core
{
    using DG.Tweening;
    using Sirenix.OdinInspector;
    using Extensions;
    using UnityEngine;

    public class VibrationManager : BehaviorSingleton<VibrationManager>
    {
        private const string VibrationKey = "EnableVibration";
        private Camera _camera;
        
        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
        }

        public void Vibrate()
        {
            if (PlayerPrefs.GetInt(VibrationKey, 1) == 1)
            {
                Handheld.Vibrate();
            }
        }
        
        [Button]
        public void ShakeCamera(float duration = 0.5f, float strength = 0.1f)
        {
            if (PlayerPrefs.GetInt(VibrationKey, 1) == 1)
            {
                _camera.DOComplete();
                _camera.DOShakePosition(duration, strength, 10, 90, false);
            }
        }

        public void SetVibration(bool isEnabled)
        {
            PlayerPrefs.SetInt(VibrationKey, isEnabled ? 1 : 0);
        }

        public bool IsVibrationEnabled()
        {
            return PlayerPrefs.GetInt(VibrationKey, 1) == 1;
        }
    }
}*/