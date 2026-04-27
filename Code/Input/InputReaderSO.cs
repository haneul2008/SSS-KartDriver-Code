using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

namespace HN.Code.Input
{
    [CreateAssetMenu(menuName = "SO/InputReader", fileName = "InputReader", order = 0)]
    public class InputReaderSO : ScriptableObject, IPlayerActions
    {
        public event Action<bool> OnDriftKeyPressed;
        public event Action OnBoosterKeyPressed;
        
        public Vector2 MovementKey { get; private set; }
        
        private Controls _controls;

        private void OnEnable()
        {
            if (_controls == null)
                _controls = new Controls();
            
            _controls.Player.SetCallbacks(this);
            _controls.Enable();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            Vector2 movementKey = context.ReadValue<Vector2>();
            MovementKey = movementKey;
        }

        public void OnDrift(InputAction.CallbackContext context)
        {
            if(context.started)
                OnDriftKeyPressed?.Invoke(true);
            else if(context.canceled)
                OnDriftKeyPressed?.Invoke(false);
        }

        public void OnBooster(InputAction.CallbackContext context)
        {
            OnBoosterKeyPressed?.Invoke();
        }
    }
}
