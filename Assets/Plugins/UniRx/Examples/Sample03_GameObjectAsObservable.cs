﻿#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

using UniRx.Triggers;
using UnityEngine;

// for enable gameObject.EventAsObservbale()

namespace UniRx.Examples
{
    public class Sample03_GameObjectAsObservable : MonoBehaviour
    {
        private void Start()
        {
            // All events can subscribe by ***AsObservable if enables UniRx.Triggers
            this.OnMouseDownAsObservable()
                .SelectMany(_ => gameObject.UpdateAsObservable())
                .TakeUntil(gameObject.OnMouseUpAsObservable())
                .Select(_ => Input.mousePosition)
                .RepeatUntilDestroy(this)
                .Subscribe(x => Debug.Log(x), () => Debug.Log("!!!" + "complete"));
        }
    }
}

#endif