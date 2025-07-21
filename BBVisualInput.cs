using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BBModMenu;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace BBVisualInput
{
    public class BBVisualInput : MelonMod
    {
        [HarmonyPatch(typeof(GameUI), "Awake")]
        private static class PatchGameUILightAwake
        {
            [HarmonyPrefix]
            private static void HarmonyPatchPrefixGameUi(GameUI __instance) {
                __instance.gameObject.AddComponent<BBInputVisualComponent>();
            }
        }
    }

    class BBInputVisualComponent : MonoBehaviour
        {
            private VisualElement visualInputRoot;
            private Dictionary<string, VisualElement> keyElements = new Dictionary<string, VisualElement>();
            private bool _enable = true;
            private static Color textColor = new Color(0.945f, 0.894f, 0.780f, 1.000f);
            private float baseKeySize = 70f;
            private float baseFontSize = 30f;
            private float baseMarginBottom = 10;
            private GameUI gameUI;
            private VisualElement HuDCustomMapVisualElement;
            private VisualElement HuDVisualElement;
            private ModMenu _modMenu;
            private InputAction inputAction;

            public void Start() {
                Debug.Log("InputVisual Loaded Plugin Start");

                visualInputRoot = new VisualElement();
                visualInputRoot.style.position = Position.Absolute;
                visualInputRoot.style.unityTextAlign = TextAnchor.MiddleCenter;
                visualInputRoot.name = "InputVisualRoot";
                visualInputRoot.transform.position = new Vector3(0f, 0f, 0f);
                
                inputAction = global::PlayerInput.InputMap.FindAction(GameObject.Find("Input").GetComponent<PlayerInput>().moveAction.action.name, false);

                var topRow = CreateKeyRow(("Z",0));
                var middleRow = CreateKeyRow(("Q",2), ("S",1), ("D",3));
                var bottomRow = CreateKeyRow(("Sprint",-1),( "Jump",-1), ("Sneak",-1));

                visualInputRoot.Add(topRow);
                visualInputRoot.Add(middleRow);
                visualInputRoot.Add(bottomRow);

                gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
                var screens = typeof(GameUI).GetField("screens", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(gameUI) as List<UIScreen>;

                
                
                
                if (screens != null)
                {
                    var hudScreen = screens.FirstOrDefault(screen => screen is HUDScreen) as HUDScreen;
                    var backingField = typeof(UIScreen).GetField($"<Screen>k__BackingField",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    CustomMapHUDScreen customMapHUDScreen  = screens.FirstOrDefault(screen => screen is CustomMapHUDScreen) as CustomMapHUDScreen;
                    if (backingField != null)
                    {
                        var visualElementOfMainScreen = backingField.GetValue(hudScreen) as VisualElement;
                        HuDVisualElement = visualElementOfMainScreen;
                        HuDVisualElement?.Add(visualInputRoot);
                        
                        var visualElementOfCustomMap = backingField.GetValue(customMapHUDScreen) as VisualElement;
                        HuDCustomMapVisualElement = visualElementOfCustomMap;
                        
                    }
                }
                
                _modMenu = screens?.FirstOrDefault(screen => screen is ModMenu) as ModMenu;

                if (_modMenu is null)
                {
                    Debug.Log("ModMenu not found");
                    return;
                }
                
                String categoryName = "InputVisual";
                var inputVisualSetting = _modMenu.AddSetting(categoryName);

                var xSlider = _modMenu.CreateSlider(categoryName,"XPositon",0, 100, 2, true);
                var ySlider = _modMenu.CreateSlider(categoryName,"YPosition",0, 100,74 , true);
                var scaleSlider = _modMenu.CreateSlider(categoryName,"Scale",0.1f, 3f, 0.7f, false);

                Action updatePosition = () => {
                    visualInputRoot.style.top = Length.Percent(ySlider.value);
                    visualInputRoot.style.left = Length.Percent(xSlider.value);
                    DisplayInSetting();
                };

                Action updateScale = () => {
                    float scale = scaleSlider.value;
                    foreach (var row in visualInputRoot.Children())
                    {
                        foreach (var keyElement in row.Children())
                        {
                            keyElement.style.width = baseKeySize * scale;
                            keyElement.style.height = baseKeySize * scale;
                            if (keyElement.childCount > 0 && keyElement[0] is Label label)
                            {
                                label.style.fontSize = baseFontSize * scale;
                            }
                        }

                        row.style.marginBottom = baseMarginBottom * scale;
                    }
                };

                xSlider.RegisterValueChangedCallback(evt => updatePosition());
                ySlider.RegisterValueChangedCallback(evt => updatePosition());
                scaleSlider.RegisterValueChangedCallback(evt => updateScale());

                
                
                var position = _modMenu.CreateGroup("Position");
                var xWrapper = _modMenu.CreateWrapper();
                xWrapper.Add(_modMenu.CreateLabel("X position"));
                xWrapper.Add(xSlider);
                var yWrapper = _modMenu.CreateWrapper();
                yWrapper.Add(_modMenu.CreateLabel("Y position"));
                yWrapper.Add(ySlider);

                var scaleWrapper = _modMenu.CreateWrapper();
                scaleWrapper.Add(_modMenu.CreateLabel("Scale"));
                scaleWrapper.Add(scaleSlider);

                var control = _modMenu.CreateGroup("Control");
                var enableWrapper = _modMenu.CreateWrapper();
                enableWrapper.Add(_modMenu.CreateLabel("Enable"));
                var toggleOnOFf = _modMenu.CreateToggle(categoryName,"On", enabled);
                toggleOnOFf.RegisterValueChangedCallback(delegate(ChangeEvent<bool> b){
                    enabled = b.newValue;
                    visualInputRoot.visible = enabled;
                });
                enableWrapper.Add(toggleOnOFf);

                position.Add(xWrapper);
                position.Add(yWrapper);
                position.Add(scaleWrapper);

                control.Add(enableWrapper);

                inputVisualSetting.Add(position);
                inputVisualSetting.Add(control);
                
                enabled = toggleOnOFf.value;
                visualInputRoot.visible = enabled;
                updatePosition();
                updateScale();
            }

            private VisualElement CreateKeyRow(params (string keyText, int input)[] keys) {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.Center;
                row.style.marginBottom = baseMarginBottom;
    
                foreach (var (keyText, input) in keys)
                {
                    var keyElement = input==-1 ? CreateKeyElement(keyText):CreateKeyElement(inputAction.controls[input].displayName);
                    keyElements[keyText] = keyElement;
                    row.Add(keyElement);
                }

                return row;
            }

            private VisualElement CreateKeyElement(string keyText) {
                var element = new VisualElement();
                element.style.height = baseKeySize;
                element.style.marginLeft = 3;
                element.style.marginRight = 3;
                element.style.justifyContent = Justify.Center;
                element.style.alignItems = Align.Center;
                element.style.backgroundColor = ModMenu._BBBackGround;
                element.style.flexShrink = 0;
                element.style.borderTopLeftRadius = 5;
                element.style.borderTopRightRadius = 5;
                element.style.borderBottomLeftRadius = 5;
                element.style.borderBottomRightRadius = 5;

                var label = new Label(keyText);
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                label.style.position = Position.Relative;
                label.style.fontSize = baseFontSize;
                element.Add(label);
                return element;
            }

            private void Update() {                
                if (!enabled)
                {
                    return;
                }
                if (gameUI.ActiveScreen.Name == "CustomMapHUD"&& visualInputRoot.parent!=HuDCustomMapVisualElement)
                {
                    HuDCustomMapVisualElement.Add(visualInputRoot);
                    visualInputRoot.style.backgroundColor = new Color(0, 0, 0, 0);
                    
                }
                if(gameUI.ActiveScreen.Name == "HUD" && visualInputRoot.parent!=HuDVisualElement)
                {
                    HuDVisualElement.Add(visualInputRoot);
                    visualInputRoot.style.backgroundColor = new Color(0, 0, 0, 0);
                    
                }
                var control = PlayerInput.InputMap.Main.Move.ReadValue<Vector2>();
                var sprint = PlayerInput.InputMap.Main.Sprint.IsPressed() || PlayerInput.SprintHeld;
                var shift = PlayerInput.InputMap.Main.Sneak.IsPressed() || PlayerInput.SneakHeld;
                var jump = PlayerInput.InputMap.Main.Jump.IsPressed()
                                  || PlayerInput.JumpHeld
                                  || GameModeManager.Instance.player.spaceDown;
                UpdateKeyElement("Z", control.y > 0.5f);
                UpdateKeyElement("S", control.y < -0.5f);
                UpdateKeyElement("Q", control.x < -0.5f);
                UpdateKeyElement("D", control.x > 0.5f);

                UpdateKeyElement("Sprint", sprint);
                UpdateKeyElement("Jump", jump);
                UpdateKeyElement("Sneak", shift);
            }




            private void DisplayInSetting() {
                _modMenu.getRootVisualElement().Add(visualInputRoot);
                visualInputRoot.style.backgroundColor = Color.white;
            }

            private void UpdateKeyElement(string key, bool isActive) {
                if (keyElements.TryGetValue(key, out var element))
                {
                    if (isActive)
                    {
                        element.style.backgroundColor = new Color(1f, 1f, 1f, 0.8f);
                        element[0].style.color = Color.black;
                    }
                    else
                    {
                        element.style.backgroundColor = ModMenu._BBBackGround;
                        element[0].style.color = textColor;
                    }
                }
            }
        }
}
