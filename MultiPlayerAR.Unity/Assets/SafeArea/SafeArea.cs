// 参考
// http://tsubakit1.hateblo.jp/entry/2019/10/30/235150

using System;
using UnityEngine;

namespace net.caffeineinject.safearea
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    [DefaultExecutionOrder(-100)]
    public class SafeArea : MonoBehaviour
    {
        private DeviceOrientation _postOrientation;
        private SafeAreaTypeEnum _postSafeAreaType;
        private RectTransform _rect;

        [SerializeField] private bool comfortXMax = true;
        [SerializeField] private bool comfortXMin = true;
        [SerializeField] private bool comfortYMax = true;
        [SerializeField] private bool comfortYMin = true;
        [SerializeField] private SafeAreaTypeEnum safeAreaType = SafeAreaTypeEnum.Anchors;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void Start()
        {
            AdjustView();
        }

        private void Update()
        {
            AdjustView();
        }

        private void AdjustView()
        {
            if (Input.deviceOrientation != DeviceOrientation.Unknown && _postOrientation == Input.deviceOrientation &&
                _postSafeAreaType == safeAreaType)
                return;

            _postOrientation = Input.deviceOrientation;
            _postSafeAreaType = safeAreaType;

            var area = Screen.safeArea;
            var resolution = Screen.currentResolution;

            switch (safeAreaType)
            {
                case SafeAreaTypeEnum.Anchors:
                    _rect.sizeDelta = Vector2.zero;

                    var xMax = comfortXMax ? area.xMax / resolution.width : 1;
                    var yMax = comfortYMax ? area.yMax / resolution.height : 1;
                    var xMin = comfortXMin ? area.xMin / resolution.width : 0;
                    var yMin = comfortYMin ? area.yMin / resolution.height : 0;

                    _rect.anchorMax = new Vector2(xMax, yMax);
                    _rect.anchorMin = new Vector2(xMin, yMin);
                    break;
                case SafeAreaTypeEnum.Offset:
                    var scale = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>().localScale;

                    var parentRectTransform = transform.parent.GetComponent<RectTransform>();

                    var width = parentRectTransform.rect.width;
                    if (comfortXMax)
                    {
                        width -= (resolution.width - area.xMax) / scale.x;
                    }

                    if (comfortXMin)
                    {
                        width -= area.xMin / scale.x;
                    }

                    var height = parentRectTransform.rect.height;
                    if (comfortYMax)
                    {
                        height -= (resolution.height - area.yMax) / scale.y;
                    }

                    if (comfortYMin)
                    {
                        height -= area.yMin / scale.y;
                    }

                    _rect.SetInsetAndSizeFromParentEdge(
                        RectTransform.Edge.Left,
                        comfortXMin ? area.xMin / scale.x : 0,
                        width);
                    _rect.SetInsetAndSizeFromParentEdge(
                        RectTransform.Edge.Bottom,
                        comfortYMin ? area.yMin / scale.y : 0,
                        height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum SafeAreaTypeEnum
        {
            Anchors,
            Offset
        }
    }
}