using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Authi.App.WinUI.Extensions
{
    public class Animator
    {
        public static class Easing
        {
            public static readonly CubicEase In = new() { EasingMode = EasingMode.EaseIn };
            public static readonly CubicEase Out = new() { EasingMode = EasingMode.EaseOut };
        }

        public static class Length
        {
            public static readonly TimeSpan Short = TimeSpan.FromMilliseconds(125);
            public static readonly TimeSpan Default = TimeSpan.FromMilliseconds(250);
            public static readonly TimeSpan Long = TimeSpan.FromMilliseconds(500);
            public static readonly TimeSpan Second = TimeSpan.FromMilliseconds(1000);
        }

        public enum Property
        {
            CenterX,
            CenterY,
            Rotation,
            ScaleX,
            ScaleY,
            SkewX,
            SkewY,
            TranslateX,
            TranslateY
        }

        private static Task? _blockingTask;

        private readonly List<(UIElement Element, Property Property, double Value)> _fromValues = [];
        private readonly List<(UIElement Element, Property Property, double Value, TimeSpan Duration, EasingFunctionBase? Easing)> _toValues = [];
        private readonly bool _isBlocking;

        private Action? _onStart;
        private Action? _onFinish;

        private Animator(bool isBlocking)
        {
            _isBlocking = isBlocking;
        }

        public static Animator Blocking()
        {
            return new Animator(true);
        }

        public static Animator NonBlocking()
        {
            return new Animator(false);
        }

        public Animator From(UIElement element, Property property, double value)
        {
            return From((element, property, value));
        }

        public Animator From(params (UIElement element, Property property, double value)[] fromValues)
        {
            _fromValues.AddRange(fromValues);
            return this;
        }

        public Animator To(UIElement element, Property property, double value, TimeSpan duration, EasingFunctionBase? easing)
        {
            return To((element, property, value, duration, easing));
        }

        public Animator To(params (UIElement element, Property property, double value, TimeSpan duration, EasingFunctionBase? easing)[] toValues)
        {
            _toValues.AddRange(toValues);
            return this;
        }

        public Animator OnStart(Action onStart)
        {
            _onStart = onStart;
            return this;
        }

        public Animator OnFinish(Action onFinish)
        {
            _onFinish = onFinish;
            return this;
        }

        public async Task RunAsync(CancellationToken? cancellationToken = null)
        {
            bool IsCancelled() => cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested;
            if (_isBlocking && _blockingTask != null)
            {
                await _blockingTask;
                if (IsCancelled())
                {
                    return;
                }
            }
            var tcs = new TaskCompletionSource();
            if (_isBlocking)
            {
                _blockingTask = tcs.Task;
            }

            _fromValues.ForEach(value =>
            {
                SetValue(value.Element, value.Property, value.Value);
            });

            _onStart?.Invoke();

            var storyboard = new Storyboard();
            _toValues.ForEach(value =>
            {
                AppendAnimation(storyboard, value.Element, value.Property, value.Value, value.Duration, value.Easing);
            });

            storyboard.Completed += (s, e) =>
            {
                if (_isBlocking)
                {
                    _blockingTask = null;
                }
                storyboard.Stop();
                _toValues.ForEach(value =>
                {
                    SetValue(value.Element, value.Property, value.Value);
                });
                _onFinish?.Invoke();
                tcs.SetResult();
            };
            cancellationToken?.Register(storyboard.Stop);
            storyboard.Begin();
            await tcs.Task;
        }

        public async void Run(CancellationToken? cancellationToken = null)
        {
            await RunAsync(cancellationToken);
        }

        private static void SetValue(UIElement element, Property property, double value)
        {
            var transform = GetTransform(element);
            switch (property)
            {
                case Property.CenterX: transform.CenterX = value; break;
                case Property.CenterY: transform.CenterY = value; break;
                case Property.Rotation: transform.Rotation = value; break;
                case Property.ScaleX: transform.ScaleX = value; break;
                case Property.ScaleY: transform.ScaleY = value; break;
                case Property.SkewX: transform.SkewX = value; break;
                case Property.SkewY: transform.SkewY = value; break;
                case Property.TranslateX: transform.TranslateX = value; break;
                case Property.TranslateY: transform.TranslateY = value; break;
            }
        }

        private static void AppendAnimation(Storyboard storyboard, UIElement element, Property property, double value, TimeSpan duration, EasingFunctionBase? easing)
        {
            GetTransform(element);

            var animation = new DoubleAnimation
            {
                To = value,
                Duration = duration
            };

            if (easing != null)
            {
                animation.EasingFunction = easing;
            }

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, $"(UIElement.RenderTransform).(CompositeTransform.{property})");
        }

        private static CompositeTransform GetTransform(UIElement element)
        {
            var compositeTransform = element.RenderTransform as CompositeTransform;
            if (compositeTransform == null)
            {
                compositeTransform = new CompositeTransform();
                element.RenderTransform = compositeTransform;
            }
            return compositeTransform;
        }
    }
}
