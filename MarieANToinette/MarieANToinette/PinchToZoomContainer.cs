using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarieANToinette.Extensions;
using Xamarin.Forms;

namespace MarieANToinette
{
    public class PinchToZoomContainer : ContentView
    {
        private double startScale;
        private double currentScale;
        private double xOffset;
        private double yOffset;
        private bool isAuthorizedToPan = false;
        double x, y;
        DateTime lastClick;
        Image image;
        AbsoluteLayout layout;
        Image progress;
        double source = 1;
        double target = 1.20;
        double screenWidth;
        double screenHeight;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (screenWidth != width || screenHeight != height)
            {
                screenWidth = width;
                screenHeight = height;
            }
        }

            public PinchToZoomContainer()
        {
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnTapUpdated;
            GestureRecognizers.Add(tapGesture);

            this.layout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            this.progress = new Image();
            this.progress.Source = ImageSource.FromResource("MarieANToinette.loading.png");
            AbsoluteLayout.SetLayoutBounds(this.progress, new Rectangle(.5, .5, .25, .25));
            AbsoluteLayout.SetLayoutFlags(this.progress, AbsoluteLayoutFlags.All);
            this.layout.Children.Add(this.progress);
            
            var animation = new Animation(v => progress.Scale = v, source, target);
            animation.Commit(this, "SimpleAnimation", 16, 1000, Easing.Linear, (v, c) => {
                if (source == 1)
                {
                    source = 1.20;
                    target = 1;
                } else
                {
                    source = 1;
                    target = 1.20;
                }
            }, () => true);

            this.image = new Image()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            
            AbsoluteLayout.SetLayoutBounds(this.image, new Rectangle(.5, .5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(this.image, AbsoluteLayoutFlags.All);
            this.layout.Children.Add(this.image);

            this.Content = layout;
        }

        public static BindableProperty ImageSrcProperty = BindableProperty.Create(
                                                            propertyName: "ImageSrc",
                                                            returnType: typeof(string),
                                                            declaringType: typeof(string),
                                                            defaultValue: "",
                                                            defaultBindingMode: BindingMode.TwoWay,
                                                            propertyChanged: HandleButtonPropertyChanged);

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }

        private async static void HandleButtonPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PinchToZoomContainer container = (PinchToZoomContainer)bindable;
            if (newValue != null && (string)newValue != "")
            {
                System.Uri uri;
                System.Uri.TryCreate((string)newValue, UriKind.Absolute, out uri);
                Task<ImageSource> result = Task<ImageSource>.Factory.StartNew(() => new UriImageSource { Uri = uri, CachingEnabled = true });
                container.image.Source = await result;
            }
        }

        private void OnTapUpdated(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastClick).Milliseconds < 300)
            {
                Content.ScaleTo(1);
                xOffset = 0;
                yOffset = 0;
                x = 0;
                y = 0;
                Content.TranslateTo(0, 0);
            } 
            lastClick = DateTime.Now;
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    isAuthorizedToPan = true;
                    break;
                case GestureStatus.Running:
                    if (isAuthorizedToPan)
                    {
                        // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                        Content.TranslationX =
                          Math.Max(Math.Min(0, x + e.TotalX), -Math.Abs(Content.Width * Content.Scale - screenWidth));
                        Content.TranslationY =
                          Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs(Content.Height * Content.Scale - screenHeight));
                    }
                    break;
                case GestureStatus.Completed:
                    if (isAuthorizedToPan)
                    {
                        // Store the translation applied during the pan
                        x = Content.TranslationX;
                        y = Content.TranslationY;
                        xOffset = Content.TranslationX;
                        yOffset = Content.TranslationY;
                        currentScale = 1;
                    }
                    break;
            }
        }

        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                isAuthorizedToPan = false;
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the X pixel coordinate.
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.
                Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
                Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

                // Apply scale factor.
                Content.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                x = Content.TranslationX;
                y = Content.TranslationY;
            }
        }
    }
}
