using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonoControls.GHWalkThrough
{
    public class GHWalkThroughView : UIView
    {
        public event EventHandler<EventArgs> WalkthroughDidDismissView;

        public GHWalkThroughViewDataSource DataSource;

		public event Action<NSIndexPath> DidEndedScroll;

        public bool isFixedBackground;

        public UIImage bgImage;

		public UICollectionView collectionView;

		private UIImageView bgFrontLayer;
        
		private UIImageView bgBackLayer;

        private UIPageControl pageControl;
        
		private UIButton skipButton;

        private UICollectionViewFlowLayout layout;

		private string closeTitle;
		public string CloseTitle
		{
			get {
				return closeTitle;
			}
			set {
				closeTitle = value;
				skipButton.SetTitle (closeTitle, UIControlState.Normal);
			}
		}

		public bool PageControlVisible
		{
			get {
				return !pageControl.Hidden;
			}
			set {
				pageControl.Hidden = !value;
			}
		}

		private GHWalkThroughViewDirection walkThroughDirection = GHWalkThroughViewDirection.Horizontal;
		public GHWalkThroughViewDirection WalkThroughDirection
		{
			get {
				return walkThroughDirection;
			}
			set {
				walkThroughDirection = value;

				UICollectionViewScrollDirection dir = (walkThroughDirection == GHWalkThroughViewDirection.Vertical) ? UICollectionViewScrollDirection.Vertical : UICollectionViewScrollDirection.Horizontal;

				layout.ScrollDirection = dir;
				layout.InvalidateLayout();

				orientFooter();
			}
		}

		private UIView floatingHeaderView;
		public UIView FloatingHeaderView
		{
			get {
				return floatingHeaderView;
			}
			set {
				if (floatingHeaderView != null) {
					floatingHeaderView.RemoveFromSuperview ();
				}

				floatingHeaderView = value;
				floatingHeaderView.Frame = new RectangleF (Frame.Width / 2 - Frame.Width / 2, 50, floatingHeaderView.Frame.Width, floatingHeaderView.Frame.Height);

				AddSubview (floatingHeaderView);

				BringSubviewToFront (floatingHeaderView);
			}
		}

        public GHWalkThroughView(RectangleF frame) : base(frame)
        {
            Setup();
        }

        public void Setup()
        {
            BackgroundColor = UIColor.Clear;
            Alpha = 0;

            bgFrontLayer = new UIImageView(Frame);
            bgBackLayer = new UIImageView(Frame);

            UIView bgView = new UIView(Frame);
            bgView.AddSubview(bgBackLayer);
            bgView.AddSubview(bgFrontLayer);

            layout = new UICollectionViewFlowLayout();
            layout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            layout.MinimumInteritemSpacing = 0.0f;
            layout.MinimumLineSpacing = 0.0f;

            collectionView = new UICollectionView(Frame, layout);
            collectionView.BackgroundColor = UIColor.Clear;
            collectionView.BackgroundView = bgView;
            collectionView.ShowsHorizontalScrollIndicator = false;
            collectionView.ShowsVerticalScrollIndicator = false;
            collectionView.WeakDataSource = this;
            collectionView.WeakDelegate = this;
            collectionView.RegisterClassForCell(typeof(GHWalkThroughPageCell), new NSString("GHWalkThroughPageCell"));
            collectionView.PagingEnabled = true;
            AddSubview(collectionView);

            buildFooterView();
        }

		[Export("collectionView:didEndDisplayingCell:forItemAtIndexPath:")]
		public virtual void CellDisplayingEnded(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			if (DidEndedScroll != null)
				DidEndedScroll(indexPath);
		}

		public void HideSkipButton()
		{
			skipButton.RemoveFromSuperview();
		}

		public void ShowInView (UIView view, float duration)
		{
			pageControl.CurrentPage = 0;
			pageControl.Pages = DataSource.NumberOfPages;

			if (isFixedBackground) {
				bgFrontLayer.Image = bgImage;
			} else{
				bgFrontLayer.Image = DataSource.BgImageforPage(0);
			}

			Alpha = 0;
			collectionView.ContentOffset = PointF.Empty;
			view.AddSubview(this);

			UIView.Animate(duration, () => { Alpha = 1; });
		}

        private void orientFooter ()
        {
            if (WalkThroughDirection == GHWalkThroughViewDirection.Vertical) {
                bool isRotated = !CGAffineTransform.Equals(pageControl.Transform, CGAffineTransform.MakeIdentity());
                if (!isRotated) {
                    skipButton.Frame = new RectangleF(skipButton.Frame.X - 30, skipButton.Frame.Y, skipButton.Frame.Width, skipButton.Frame.Height);

                    pageControl.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2);

                    pageControl.Frame = new RectangleF(Frame.Width - Frame.Width - 10, skipButton.Frame.Y + skipButton.Frame.Height - Frame.Height, pageControl.Frame.Width, (DataSource.NumberOfPages + 1 ) * 16);
                }
            } else {
                bool isRotated = !CGAffineTransform.Equals(pageControl.Transform, CGAffineTransform.MakeIdentity());
                if (isRotated) {
                    // Rotate back the page control
                    pageControl.Transform = CGAffineTransform.MakeRotation(((float)Math.PI/2) * -1);
                    pageControl.Frame = new RectangleF(0, Frame.Height - 60, Frame.Width, 20);
                    
                    skipButton.Frame = new RectangleF(Frame.Width - 80, pageControl.Frame.Y - ((30 - pageControl.Frame.Height) / 2), 80, 30);
                }
            }
        }

        private void buildFooterView () {
            pageControl = new UIPageControl(new RectangleF(0, Frame.Height - 60, Frame.Width, 20));
            
            //Set defersCurrentPageDisplay to YES to prevent page control jerking when switching pages with page control. This prevents page control from instant change of page indication.
            pageControl.DefersCurrentPageDisplay = true;
            
            pageControl.AutoresizingMask =  UIViewAutoresizing.FlexibleWidth;
            pageControl.AddTarget(this, new MonoTouch.ObjCRuntime.Selector("showPanelAtPageControl"), UIControlEvent.ValueChanged);
            pageControl.UserInteractionEnabled = false; // TODO: review
            AddSubview(pageControl);
            BringSubviewToFront(pageControl);

            skipButton = new UIButton(UIButtonType.RoundedRect);
            
            skipButton.Frame = new RectangleF(Frame.Width - 80, pageControl.Frame.Y - ((30 - pageControl.Frame.Height) / 2), 80, 30);
            
            skipButton.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
            skipButton.SetTitle("Skip", UIControlState.Normal);
            skipButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            skipButton.AddTarget(this, new MonoTouch.ObjCRuntime.Selector("skipIntroduction"), UIControlEvent.TouchUpInside);

            AddSubview(skipButton);
            BringSubviewToFront(skipButton);
        }

        [Export("skipIntroduction")]
        private void skipIntroduction ()
        {
            UIView.Animate(0.3, () =>
                {
                    Alpha = 0;
                },
                () =>
                {
                    DispatchTime popTime = new DispatchTime(DispatchTime.Now, 0);
                    DispatchQueue.MainQueue.DispatchAfter(popTime, () =>
                        {
                            RemoveFromSuperview();
                            if (WalkthroughDidDismissView != null)
                                WalkthroughDidDismissView(this, null);
                        });
                });
        }

        [Export("showPanelAtPageControl:")]
        public void showPanelAtPageControl (UIPageControl sender)
        {
            pageControl.CurrentPage = sender.CurrentPage;
        }

        [Export("collectionView:numberOfSectionsInCollectionView")]
        public int NumberOfSectionsInCollectionView (UICollectionView  collectionView)
        {
            return 1;
        }

        [Export("collectionView:numberOfItemsInSection:")]
        public int NumberOfItemsInSection(UICollectionView collectionView, int section)
        {
            return DataSource.NumberOfPages;
        }

        [Export("collectionView:cellForItemAtIndexPath:")]
        public UICollectionViewCell CellForItemAtIndexPath (UICollectionView collectionView, NSIndexPath indexPath)
        {
            GHWalkThroughPageCell cell = (GHWalkThroughPageCell)collectionView.DequeueReusableCell(new NSString("GHWalkThroughPageCell"), indexPath);
            
            if (DataSource != null) {
                DataSource.ConfigurePage(cell, indexPath.Row);
            }
            return cell;
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public SizeF SizeForItemAtIndexPath (UICollectionView collectionView, UICollectionViewFlowLayout layout, NSIndexPath indexPath) {
            return Frame.Size;
        }

        [Export("scrollViewDidScroll:")]
        public void scrollViewDidScroll (UIScrollView scrollView) {
            // Get scrolling position, and send the alpha values.
            if (!isFixedBackground) {
                float offset = WalkThroughDirection == GHWalkThroughViewDirection.Horizontal ? collectionView.ContentOffset.X / collectionView.Frame.Width : collectionView.ContentOffset.Y / collectionView.Frame.Height;
                crossDissolveForOffset(offset);
            }
            
            float pageMetric = 0.0f;
            float contentOffset = 0.0f;
            
            switch (WalkThroughDirection) {
                case GHWalkThroughViewDirection.Horizontal:
                    pageMetric = scrollView.Frame.Width;
                    contentOffset = scrollView.ContentOffset.X;
                    break;
                case GHWalkThroughViewDirection.Vertical:
                    pageMetric = scrollView.Frame.Height;
                    contentOffset = scrollView.ContentOffset.Y;
                    break;
            }
            
            int page = Convert.ToInt16(Math.Floor((contentOffset - pageMetric / 2) / pageMetric) + 1);
            pageControl.CurrentPage = page;
        }

        private void crossDissolveForOffset (float offset) {
            int page = (int)(offset);
            float alphaValue = offset - (int)offset;
            
            if (alphaValue < 0 && pageControl.CurrentPage == 0) {
                bgBackLayer.Image = null;
                bgFrontLayer.Alpha = (1 + alphaValue);
                return;
            }
            
            bgFrontLayer.Alpha = 1;
            bgFrontLayer.Image = DataSource.BgImageforPage(page);
            bgBackLayer.Alpha = 0;
            bgBackLayer.Image = DataSource.BgImageforPage(page + 1);
            
            float backLayerAlpha = alphaValue;
            float frontLayerAlpha = (1 - alphaValue);
            
            backLayerAlpha = easeOutValue(backLayerAlpha);
            frontLayerAlpha = easeOutValue(frontLayerAlpha);
            
            bgBackLayer.Alpha = backLayerAlpha;
            bgFrontLayer.Alpha = frontLayerAlpha;
        }

        private float easeOutValue(float value) {
            float inverse = value - 1.0f;
            return 1.0f + inverse * inverse * inverse;
        }
    }
}