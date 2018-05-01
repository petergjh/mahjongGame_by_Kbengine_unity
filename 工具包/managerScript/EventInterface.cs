using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using UnityEngine.Events;

public class EventInterface  : UnityEngine.EventSystems.EventTrigger {
		public delegate void VoidDelegate(GameObject go,BaseEventData be);
		public delegate void callBC(Transform go);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegate onDrag;
        public VoidDelegate onPress;//ex by yangxun
        
	static public EventInterface Get(Transform go)
        {
		EventInterface listener = go.GetComponent<EventInterface>();
		if (listener == null) listener = go.gameObject.AddComponent<EventInterface>();
            return listener;
        }
        /// <summary>
        /// 添加事件
        /// </summary>
		/// <param name="tr">需要添加事件的Transform</param>
		/// <param name="bc">回调的函数</param>
		/// <param name="nType">事件类型,默认“onClick”</param>
	public static void AddOnEvent(Transform go,callBC bc,string nType="onClick")
        {
            switch (nType)
            {
		case "onClick":
			EventInterface.Get(go).onClick = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "onDown":
			EventInterface.Get(go).onDown = delegate(GameObject o,BaseEventData be)
                    {
					bc(go);
                    };
                    break;
                case "onEnter":
			EventInterface.Get(go).onEnter = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "onExit":
			EventInterface.Get(go).onExit = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "onUp":
			EventInterface.Get(go).onUp = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "onSelect":
			EventInterface.Get(go).onSelect = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "onUpdateSelect":
			EventInterface.Get(go).onUpdateSelect = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "onDrag":
			EventInterface.Get(go).onDrag = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "onPress":
			EventInterface.Get(go).onPress = delegate(GameObject o,BaseEventData be)
                    {
						bc(go);
                    };
                    break;
                case "OnPointerExit":
			EventInterface.Get(go).onPress = delegate(GameObject o, BaseEventData be)
                    {
						bc(go);
				};
                    break;
                case "onSliderChang":
                    Slider sd = go.gameObject.GetComponent<Slider>();
                    sd.onValueChanged.RemoveAllListeners();
                    sd.onValueChanged.AddListener(delegate(float value)
                    {
						bc(go);
				});
                    break;
                case "ButtononClick":
                    {
                        Button btn = go.GetComponent<Button>();
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(delegate()
                        {
							bc(go);
				});
                        break;
                    }

                case "onEditEnd":
                    {
                        InputField input = go.GetComponent<InputField>();
                        input.onEndEdit.RemoveAllListeners();
                        input.onEndEdit.AddListener(delegate(string inputText)
                        {
							bc(go);
						});
                        break;
                    }

            }

        }

        public override void OnPointerClick(PointerEventData eventData)
        {
			if (onClick != null) onClick(gameObject,eventData);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            isPress = true;
			if (onDown != null) onDown(gameObject,eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
			if (onEnter != null) onEnter(gameObject,eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
			if (onExit != null) onExit(gameObject,eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            isPress = false;
			if (onUp != null) onUp(gameObject,eventData);
        }
        public override void OnSelect(BaseEventData eventData)
        {
			if (onSelect != null) onSelect(gameObject,eventData);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
			if (onUpdateSelect != null) onUpdateSelect(gameObject,eventData);
        }
        public override void OnDrag(PointerEventData data)
        {
			if (onDrag != null) onDrag(gameObject,data);
        }
        //ex by yangxun  
        protected bool isPress = false;
		protected BaseEventData be = null;
        protected void OnPress()
        {
            if (onPress != null)
				onPress(gameObject,be);
        }
        void Update()
        {
            if (isPress)
                OnPress();
        }

        public static Vector3 GetInutPosition()
        {
            return Input.mousePosition;
        }
  
    }