/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions.EasingCore;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Controls the scroll position.
    /// </summary>
    public class Scroller : UIBehaviour, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler
    {
        [SerializeField] RectTransform viewport = default;

        /// <summary>
        /// Viewport size.
        /// </summary>
        public float ViewportSize => scrollDirection == ScrollDirection.Horizontal
            ? viewport.rect.size.x
            : viewport.rect.size.y;

        [SerializeField] ScrollDirection scrollDirection = ScrollDirection.Vertical;

        /// <summary>
        /// Scroll direction.
        /// </summary>
        public ScrollDirection ScrollDirection => scrollDirection;

        [SerializeField] MovementType movementType = MovementType.Elastic;

        /// <summary>
        /// Behavior when content moves beyond scroll range.
        /// </summary>
        public MovementType MovementType
        {
            get => movementType;
            set => movementType = value;
        }

        [SerializeField] float elasticity = 0.1f;

        /// <summary>
        /// Elasticity when content moves beyond scroll range.
        /// </summary>
        public float Elasticity
        {
            get => elasticity;
            set => elasticity = value;
        }

        [SerializeField] float scrollSensitivity = 1f;

        /// <summary>
        /// Scroll sensitivity.
        /// </summary>
        public float ScrollSensitivity
        {
            get => scrollSensitivity;
            set => scrollSensitivity = value;
        }

        [SerializeField] bool inertia = true;

        /// <summary>
        /// Use inertia.
        /// </summary>
        public bool Inertia
        {
            get => inertia;
            set => inertia = value;
        }

        [SerializeField] float decelerationRate = 0.03f;

        /// <summary>
        /// Deceleration rate when inertia is enabled.
        /// </summary>
        public float DecelerationRate
        {
            get => decelerationRate;
            set => decelerationRate = value;
        }

        [SerializeField] Snap snap = new Snap {
            Enable = true,
            VelocityThreshold = 0.5f,
            Duration = 0.3f,
            Easing = Ease.InOutCubic
        };

        /// <summary>
        /// Enable snapping.
        /// </summary>
        public bool SnapEnabled
        {
            get => snap.Enable;
            set => snap.Enable = value;
        }

        [SerializeField] bool draggable = true;

        /// <summary>
        /// Allow drag input.
        /// </summary>
        public bool Draggable
        {
            get => draggable;
            set => draggable = value;
        }

        [SerializeField] Scrollbar scrollbar = default;

        /// <summary>
        /// Scrollbar object.
        /// </summary>
        public Scrollbar Scrollbar => scrollbar;

        /// <summary>
        /// Current scroll position.
        /// </summary>
        public float Position
        {
            get => currentPosition;
            set
            {
                autoScrollState.Reset();
                velocity = 0f;
                dragging = false;
                UpdatePosition(value);
            }
        }

        readonly AutoScrollState autoScrollState = new AutoScrollState();

        Action<float> onValueChanged;
        Action<int> onSelectionChanged;

        Vector2 beginDragPointerPosition;
        float scrollStartPosition;
        float prevPosition;
        float currentPosition;

        int totalCount;

        bool hold;
        bool scrolling;
        bool dragging;
        float velocity;

        [Serializable]
        class Snap
        {
            public bool Enable;
            public float VelocityThreshold;
            public float Duration;
            public Ease Easing;
        }

        static readonly EasingFunction DefaultEasingFunction = Easing.Get(Ease.OutCubic);

        class AutoScrollState
        {
            public bool Enable;
            public bool Elastic;
            public float Duration;
            public EasingFunction EasingFunction;
            public float StartTime;
            public float EndPosition;
            public Action OnComplete;

            public void Reset()
            {
                Enable = false;
                Elastic = false;
                Duration = 0f;
                StartTime = 0f;
                EasingFunction = DefaultEasingFunction;
                EndPosition = 0f;
                OnComplete = null;
            }

            public void Complete()
            {
                OnComplete?.Invoke();
                Reset();
            }
        }

        protected override void Start()
        {
            base.Start();

            if (scrollbar)
            {
                scrollbar.onValueChanged.AddListener(x => UpdatePosition(x * (totalCount - 1f), false));
            }
        }

        /// <summary>
        /// Set callback for value changes.
        /// </summary>
        /// <param name="callback">Callback for value changes.</param>
        public void OnValueChanged(Action<float> callback) => onValueChanged = callback;

        /// <summary>
        /// Set callback for selection changes.
        /// </summary>
        /// <param name="callback">Callback for selection changes.</param>
        public void OnSelectionChanged(Action<int> callback) => onSelectionChanged = callback;

        /// <summary>
        /// Set total item count.
        /// </summary>
        /// <param name="totalCount">Total item count.</param>
        public void SetTotalCount(int totalCount) => this.totalCount = totalCount;

        /// <summary>
        /// Scroll to specified position.
        /// </summary>
        /// <param name="position">Scroll position.</param>
        /// <param name="duration">Scroll duration.</param>
        /// <param name="onComplete">Callback on completion.</param>
        public void ScrollTo(float position, float duration, Action onComplete = null) => ScrollTo(position, duration, Ease.OutCubic, onComplete);

        /// <summary>
        /// Scroll to specified position with easing.
        /// </summary>
        /// <param name="position">Scroll position.</param>
        /// <param name="duration">Scroll duration.</param>
        /// <param name="easing">Easing function.</param>
        /// <param name="onComplete">Callback on completion.</param>
        public void ScrollTo(float position, float duration, Ease easing, Action onComplete = null) => ScrollTo(position, duration, Easing.Get(easing), onComplete);

        /// <summary>
        /// Scroll to specified position with custom easing function.
        /// </summary>
        /// <param name="position">Scroll position.</param>
        /// <param name="duration">Scroll duration.</param>
        /// <param name="easingFunction">Custom easing function.</param>
        /// <param name="onComplete">Callback on completion.</param>
        public void ScrollTo(float position, float duration, EasingFunction easingFunction, Action onComplete = null)
        {
            if (duration <= 0f)
            {
                Position = CircularPosition(position, totalCount);
                onComplete?.Invoke();
                return;
            }

            autoScrollState.Reset();
            autoScrollState.Enable = true;
            autoScrollState.Duration = duration;
            autoScrollState.EasingFunction = easingFunction ?? DefaultEasingFunction;
            autoScrollState.StartTime = Time.unscaledTime;
            autoScrollState.EndPosition = currentPosition + CalculateMovementAmount(currentPosition, position);
            autoScrollState.OnComplete = onComplete;

            velocity = 0f;
            scrollStartPosition = currentPosition;

            UpdateSelection(Mathf.RoundToInt(CircularPosition(autoScrollState.EndPosition, totalCount)));
        }

        /// <summary>
        /// Jump to specified index.
        /// </summary>
        /// <param name="index">Item index.</param>
        public void JumpTo(int index)
        {
            if (index < 0 || index > totalCount - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            UpdateSelection(index);
            Position = index;
        }

        /// <summary>
        /// Get movement direction from source to destination index.
        /// </summary>
        /// <param name="sourceIndex">Source index.</param>
        /// <param name="destIndex">Destination index.</param>
        /// <returns></returns>
        public MovementDirection GetMovementDirection(int sourceIndex, int destIndex)
        {
            var movementAmount = CalculateMovementAmount(sourceIndex, destIndex);
            return scrollDirection == ScrollDirection.Horizontal
                ? movementAmount > 0
                    ? MovementDirection.Left
                    : MovementDirection.Right
                : movementAmount > 0
                    ? MovementDirection.Up
                    : MovementDirection.Down;
        }

        /// <inheritdoc/>
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            hold = true;
            velocity = 0f;
            autoScrollState.Reset();
        }

        /// <inheritdoc/>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (hold && snap.Enable)
            {
                UpdateSelection(Mathf.RoundToInt(CircularPosition(currentPosition, totalCount)));
                ScrollTo(Mathf.RoundToInt(currentPosition), snap.Duration, snap.Easing);
            }

            hold = false;
        }

        /// <inheritdoc/>
        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (!draggable)
            {
                return;
            }

            var delta = eventData.scrollDelta;

            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            var scrollDelta = scrollDirection == ScrollDirection.Horizontal
                ? Mathf.Abs(delta.y) > Mathf.Abs(delta.x)
                        ? delta.y
                        : delta.x
                : Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                        ? delta.x
                        : delta.y;

            if (eventData.IsScrolling())
            {
                scrolling = true;
            }

            var position = currentPosition + scrollDelta / ViewportSize * scrollSensitivity;
            if (movementType == MovementType.Clamped)
            {
                position += CalculateOffset(position);
            }

            if (autoScrollState.Enable)
            {
                autoScrollState.Reset();
            }

            UpdatePosition(position);
        }

        /// <inheritdoc/>
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            hold = false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewport,
                eventData.position,
                eventData.pressEventCamera,
                out beginDragPointerPosition);

            scrollStartPosition = currentPosition;
            dragging = true;
            autoScrollState.Reset();
        }

        /// <inheritdoc/>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left || !dragging)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewport,
                eventData.position,
                eventData.pressEventCamera,
                out var dragPointerPosition))
            {
                return;
            }

            var pointerDelta = dragPointerPosition - beginDragPointerPosition;
            var position = (scrollDirection == ScrollDirection.Horizontal ? -pointerDelta.x : pointerDelta.y)
                           / ViewportSize
                           * scrollSensitivity
                           + scrollStartPosition;

            var offset = CalculateOffset(position);
            position += offset;

            if (movementType == MovementType.Elastic)
            {
                if (offset != 0f)
                {
                    position -= RubberDelta(offset, scrollSensitivity);
                }
            }

            UpdatePosition(position);
        }

        /// <inheritdoc/>
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            dragging = false;
        }

        float CalculateOffset(float position)
        {
            if (movementType == MovementType.Unrestricted)
            {
                return 0f;
            }

            if (position < 0f)
            {
                return -position;
            }

            if (position > totalCount - 1)
            {
                return totalCount - 1 - position;
            }

            return 0f;
        }

        void UpdatePosition(float position, bool updateScrollbar = true)
        {
            onValueChanged?.Invoke(currentPosition = position);

            if (scrollbar && updateScrollbar)
            {
                scrollbar.value = Mathf.Clamp01(position / Mathf.Max(totalCount - 1f, 1e-4f));
            }
        }

        void UpdateSelection(int index) => onSelectionChanged?.Invoke(index);

        float RubberDelta(float overStretching, float viewSize) =>
            (1 - 1 / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1)) * viewSize * Mathf.Sign(overStretching);

        void Update()
        {
            var deltaTime = Time.unscaledDeltaTime;
            var offset = CalculateOffset(currentPosition);

            if (autoScrollState.Enable)
            {
                var position = 0f;

                if (autoScrollState.Elastic)
                {
                    position = Mathf.SmoothDamp(currentPosition, currentPosition + offset, ref velocity,
                        elasticity, Mathf.Infinity, deltaTime);

                    if (Mathf.Abs(velocity) < 0.01f)
                    {
                        position = Mathf.Clamp(Mathf.RoundToInt(position), 0, totalCount - 1);
                        velocity = 0f;
                        autoScrollState.Complete();
                    }
                }
                else
                {
                    var alpha = Mathf.Clamp01((Time.unscaledTime - autoScrollState.StartTime) /
                                               Mathf.Max(autoScrollState.Duration, float.Epsilon));
                    position = Mathf.LerpUnclamped(scrollStartPosition, autoScrollState.EndPosition,
                        autoScrollState.EasingFunction(alpha));

                    if (Mathf.Approximately(alpha, 1f))
                    {
                        autoScrollState.Complete();
                    }
                }

                UpdatePosition(position);
            }
            else if (!(dragging || scrolling) && (!Mathf.Approximately(offset, 0f) || !Mathf.Approximately(velocity, 0f)))
            {
                var position = currentPosition;

                if (movementType == MovementType.Elastic && !Mathf.Approximately(offset, 0f))
                {
                    autoScrollState.Reset();
                    autoScrollState.Enable = true;
                    autoScrollState.Elastic = true;

                    UpdateSelection(Mathf.Clamp(Mathf.RoundToInt(position), 0, totalCount - 1));
                }
                else if (inertia)
                {
                    velocity *= Mathf.Pow(decelerationRate, deltaTime);

                    if (Mathf.Abs(velocity) < 0.001f)
                    {
                        velocity = 0f;
                    }

                    position += velocity * deltaTime;

                    if (snap.Enable && Mathf.Abs(velocity) < snap.VelocityThreshold)
                    {
                        ScrollTo(Mathf.RoundToInt(currentPosition), snap.Duration, snap.Easing);
                    }
                }
                else
                {
                    velocity = 0f;
                }

                if (!Mathf.Approximately(velocity, 0f))
                {
                    if (movementType == MovementType.Clamped)
                    {
                        offset = CalculateOffset(position);
                        position += offset;

                        if (Mathf.Approximately(position, 0f) || Mathf.Approximately(position, totalCount - 1f))
                        {
                            velocity = 0f;
                            UpdateSelection(Mathf.RoundToInt(position));
                        }
                    }

                    UpdatePosition(position);
                }
            }

            if (!autoScrollState.Enable && (dragging || scrolling) && inertia)
            {
                var newVelocity = (currentPosition - prevPosition) / deltaTime;
                velocity = Mathf.Lerp(velocity, newVelocity, deltaTime * 10f);
            }

            prevPosition = currentPosition;
            scrolling = false;
        }

        float CalculateMovementAmount(float sourcePosition, float destPosition)
        {
            if (movementType != MovementType.Unrestricted)
            {
                return Mathf.Clamp(destPosition, 0, totalCount - 1) - sourcePosition;
            }

            var amount = CircularPosition(destPosition, totalCount) - CircularPosition(sourcePosition, totalCount);

            if (Mathf.Abs(amount) > totalCount * 0.5f)
            {
                amount = Mathf.Sign(-amount) * (totalCount - Mathf.Abs(amount));
            }

            return amount;
        }

        float CircularPosition(float p, int size) => size < 1 ? 0 : p < 0 ? size - 1 + (p + 1) % size : p % size;
    }
}
