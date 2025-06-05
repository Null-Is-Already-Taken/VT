using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using DG.Tweening;

namespace VT.ReuseableSystems.UI.Legacy
{
    public class UIPage : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField] protected CanvasGroup canvasGroup;

        [Header("Transition")]
        [SerializeField] protected UITransitionType transitionType = UITransitionType.ScaleBounce;

        [Header("Navigation Buttons")]
        [SerializeField] public List<TransitionMapping> transitionMappings = new();

        [Serializable]
        public class TransitionMapping
        {
            public Button transitionButton;
            public UIPage targetPage;
        }

        protected Action<UIPage> OnPageRequest;
        protected Func<UITransitionType, IUITransitionEffect> effectGetter;

        protected IUITransitionEffect fallbackEffect;

        public UITransitionType TransitionType => transitionType;
        public string Name => name;

        protected virtual void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            SetInteractable(false);

            fallbackEffect = new ScaleBounceEffect(1f);
        }

        protected virtual void Start()
        {
            SetupTransitionMappings();
        }

        public virtual void Initialize(Action<UIPage> onPageRequest, Func<UITransitionType, IUITransitionEffect> effectResolver)
        {
            OnPageRequest = onPageRequest;
            effectGetter = effectResolver;
            SetupTransitionMappings();
        }

        protected virtual void SetupTransitionMappings()
        {
            foreach (var mapping in transitionMappings)
            {
                if (mapping.transitionButton && mapping.targetPage)
                {
                    mapping.transitionButton.onClick.AddListener(() =>
                    {
                        OnPageRequest?.Invoke(mapping.targetPage);
                        var effect = effectGetter?.Invoke(mapping.targetPage.transitionType);
                        effect?.Play(mapping.targetPage.transform);
                    });
                }
            }
        }

        public virtual void ShowPage()
        {
            var effect = effectGetter?.Invoke(transitionType) ?? fallbackEffect;

            effect.Play(transform)
            .OnStart(() =>
            {
                gameObject.SetActive(true);
                SetInteractable(false);
            })
            .OnComplete(() =>
            {
                SetInteractable(true);
            });
        }

        public virtual void HidePage()
        {
            var effect = effectGetter?.Invoke(transitionType) ?? fallbackEffect;

            effect.Rewind(transform)
            .OnStart(() =>
            {
                SetInteractable(false);
            })
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }


        protected void SetInteractable(bool value)
        {
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }
    }
}
