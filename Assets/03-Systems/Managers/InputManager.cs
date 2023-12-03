using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HappyTroll
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        [SerializeField] private InputActionAsset actions;

        protected override void Awake()
        {
            base.Awake();

            actions.FindActionMap(Enums.ActionMap.Gameplay.ToString()).FindAction(Enums.InputAction.Press.ToString())
                .performed += HandlePress;
            actions.FindActionMap(Enums.ActionMap.Gameplay.ToString()).FindAction(Enums.InputAction.Release.ToString())
                .performed += HandleRelease;
        }

        private void HandlePress(InputAction.CallbackContext ctx)
        {
            var position = Pointer.current.position.ReadValue();
            EventManager.InputPressEvent(position);
        }

        private void HandleRelease(InputAction.CallbackContext ctx)
        {
            var position = Pointer.current.position.ReadValue();
            EventManager.InputReleaseEvent(position);
        }

        private void OnEnable()
        {
            actions.Enable();
        }

        private void OnDisable()
        {
            actions.Disable();
        }

        public static Vector2 GetPointerPosition()
        {
            var position = Pointer.current.position.ReadValue();
            return position;
        }
    }
}
