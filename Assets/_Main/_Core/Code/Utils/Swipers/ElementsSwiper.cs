using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class ElementsSwiper : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] protected AnimationCurve mScaleCurve = new AnimationCurve() { keys = new Keyframe[] { new Keyframe(-1, 1), new Keyframe(0, 1), new Keyframe(1, 1) } };
        [SerializeField] private AnimationCurve mRotationCurve = new AnimationCurve() { keys = new Keyframe[] { new Keyframe(-1, 0), new Keyframe(0, 0), new Keyframe(1, 0) } };
        [SerializeField] private Vector3 rotationVec;
        [SerializeField] private AnimationCurve mOffsetCurve = new AnimationCurve() { keys = new Keyframe[] { new Keyframe(-1, 0), new Keyframe(0, 0), new Keyframe(1, 0) } };
        [SerializeField] private Vector3 offsetVec;

        [SerializeField] protected GameObject ElementExample;
        [SerializeField] protected Transform elementsParent;
        [SerializeField] protected List<Transform> elements = new List<Transform>();

        [SerializeField] protected float spacing = 0f;
        [SerializeField] private float percentThreshold = 0.4f;
        [SerializeField] private float easing = 0.5f;

        protected int curElementNumber;
        protected bool isBlocked;

        private float lastScreenWidth = 0f, lastScreenHeight = 0f;
        private Tween moveTween;
        private bool isMoving;

        public ReactiveProperty<int> Element = new ReactiveProperty<int>();
        public ReactiveCommand<int> ElementChangeCommand = new ReactiveCommand<int>();
        public ReactiveCommand ElementAddCommand = new ReactiveCommand();
        public ReactiveCommand ElementRemoveCommand = new ReactiveCommand();
        public bool IsDragging { get; protected set; }
        protected abstract float ElementSize { get; }
        protected abstract Vector3 ParentPosition { get; }
        public int ElementsCount => elements.Count;
        public int CurElementNumber => curElementNumber;

        private void Awake()
        {
            if (elementsParent == null)
                elementsParent = transform;

            Initialize();
        }

        void Start()
        {
            InitializeStart();
            SetElementsStartPositions();
            RefreshActualPosition();

            lastScreenHeight = Screen.height;
            lastScreenWidth = Screen.width;
        }

        private void Update()
        {
            if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
            {
                lastScreenHeight = Screen.height;
                lastScreenWidth = Screen.width;

                RefreshActualPosition();
            }
            SetElementsStartPositions();
            curElementNumber = GetClosestNumberToCurrentCenterPosition();
            Element.Value = curElementNumber;
            ElementChangeCommand.Execute(curElementNumber);
            OnUpdate();
            ApplyEffects();
        }
 
        protected abstract void InitializeStart();

        protected abstract void Initialize();

        protected virtual void OnUpdate() 
        {
        
        }

        public Transform GetElement(int elementIndex)
        {
            if (elements.Count > elementIndex)
            {
                return elements[elementIndex];
            }
            else
            {
                return AddNewElement();
            }
        }

        protected virtual Transform AddNewElement()
        {
            var newElement = diContainer.InstantiatePrefab(ElementExample, elementsParent);
            ElementAdded(newElement);
            ElementAddCommand.Execute();

            RefreshActualPosition();

            return newElement.transform;
        }

        protected void ElementAdded(GameObject go)
        {
            elements.Add(go.transform);
        }

        protected void RefreshActualPosition()
        {
            SetElementsStartPositions();
            Move(GetParentLocation(curElementNumber));
        }

        protected void SetElementsStartPositions()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                SetItemPosition(i);
            }
        }

        private void Move(Vector3 pos)
        {
            Move(pos, pos, 1f);
        }

        protected void Move(Vector3 startpos, Vector3 endpos, float t)
        {
            SetParentPosition(Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0.0f, 1.0f, t)));
        }

        protected abstract Vector3 GetParentLocation(int i);

        protected abstract void SetParentPosition(Vector3 position);

        protected abstract Vector3 GetElementPosition(int i);

        protected abstract void SetItemPosition(int i);

        protected abstract void ApplyOffset(Transform transform, Vector3 vector3);

        private void ApplyEffects()
        {
            for (int i = 0; i < elements.Count; i++)
            {

                var currentLocation = ParentPosition;

                var current = curElementNumber;
                var target = GetClosestNumberToCurrent();

                var curPos = GetParentLocation(current).x;
                var targetPos = GetParentLocation(target).x;

                float currentT = i - (curElementNumber);

                if (current == i)
                    currentT += Mathf.Sign(current - target) * Mathf.InverseLerp(curPos, targetPos, currentLocation.x);
                else
                    currentT += Mathf.Sign(current - target) * Mathf.InverseLerp(curPos, targetPos, currentLocation.x);

                ApplyEffects(elements[i], currentT);
            }
        }

        private void ApplyEffects(Transform transform, float currentT)
        {
            ApplyScale(transform, mScaleCurve.Evaluate(currentT) * Vector3.one);
            ApplyRotation(transform, mRotationCurve.Evaluate(currentT) * rotationVec);
            ApplyOffset(transform, mOffsetCurve.Evaluate(currentT) * offsetVec);

            var scaleOffset = (mScaleCurve.Evaluate(0) - 1) * Vector3.right * ElementSize / 2 * Mathf.Sign(currentT);
            scaleOffset = Vector3.Lerp(Vector3.zero, scaleOffset, Mathf.Abs(currentT));

            ApplyOffset(transform, scaleOffset);
        }

        private void ApplyScale(Transform transform, Vector3 scale)
        {
            transform.localScale = scale;
        }


        private void ApplyRotation(Transform transform, Vector3 rotation)
        {
            transform.localRotation = Quaternion.Euler(rotation);
        }

        protected int GetClosestNumberToCurrentCenterPosition()
        {
            int elementNumber = 0;

            for (int i = 0; i < ElementsCount; i++)
            {
                elementNumber = i;
                var newLocation = GetParentLocation(elementNumber);
                if (newLocation.x < ParentPosition.x)
                {
                    break;
                }
            }

            // check if previous is closer
            if (elementNumber != 0)
            {
                if (Mathf.Abs(GetParentLocation(elementNumber - 1).x - ParentPosition.x) <
                    Mathf.Abs(GetParentLocation(elementNumber).x - ParentPosition.x))
                {
                    elementNumber--;
                }
            }

            return elementNumber;
        }

        private int GetClosestNumberToCurrent()
        {
            var targetPosition = GetParentLocation(curElementNumber);
            var centerPosition = ParentPosition;

            if (targetPosition.x < centerPosition.x)
                return Mathf.Clamp(curElementNumber - 1, 0, elements.Count);
            else
                return Mathf.Clamp(curElementNumber + 1, 0, elements.Count);
        }
        public virtual void MoveElements(int elementsDiff, bool instant)
        {
            MoveToElement(curElementNumber + elementsDiff, instant);
        }

        public virtual void MoveTo(float elementNum)
        {
            var nearestIndex = Mathf.RoundToInt(elementNum);
            var nextIndex = nearestIndex + (int)Mathf.Sign(elementNum - nearestIndex);

            if (nearestIndex > nextIndex)
            {
                var tmp = nearestIndex;
                nearestIndex = nextIndex;
                nextIndex = tmp;
            }

            var t = Mathf.InverseLerp(nearestIndex, nextIndex, elementNum);

            var pos1 = GetParentLocation(nearestIndex);
            var pos2 = GetParentLocation(nextIndex);

            var pos = Vector3.Lerp(pos1, pos2, t);

            SetParentPosition(pos);
            SetElementsStartPositions();
            ApplyEffects();
        }

        public virtual void MoveToElement(int elementNum, bool instant)
        {
            if (elementNum >= elements.Count)
            {
                elementNum = 0;
            }
            if (elementNum < 0)
            {
                elementNum = elements.Count - 1;
            }
            curElementNumber = elementNum;
            var finalPosition = GetParentLocation(elementNum);
            if (instant)
            {
                SetParentPosition(finalPosition);
                SetElementsStartPositions();
                ApplyEffects();
            }
            else
                SmoothMove(ParentPosition, finalPosition, easing, 0f);
        }

        protected void MoveToClosestElement()
        {
            if (gameObject.activeInHierarchy == false)
                return;

            if (moveTween != null)
                moveTween.Complete();

            var closestNumber = GetClosestNumberToCurrentCenterPosition();

            var finalPosition = GetParentLocation(closestNumber);
            SmoothMove(ParentPosition, finalPosition, easing, 0f);
        }

        private void SmoothMove(Vector3 startpos, Vector3 endpos, float seconds, float startT)
        {
            float t = startT;

            if (moveTween != null)
                moveTween.Complete();

            moveTween = DOTween.To(() => t, x => t = x, 1f, seconds).OnUpdate(() =>
            {
                isMoving = true;

                if (startT == 1f)
                    seconds = 0;
                else
                    seconds /= (1 - startT);

                t += Time.deltaTime / seconds;
                Move(startpos, endpos, t);
                isMoving = false;
            }).OnComplete(() =>
            {
                moveTween = null;
                Move(startpos, endpos, 1f);
            });
        }

        public void Block()
        {
            isBlocked = true;
        }
    }