using System;
using System.Drawing;
using MonoControls.GHWalkThrough;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GHWalkThroughDemo
{
    public partial class GHWalkThroughDemoViewController : UIViewController
    {
        public GHWalkThroughDemoViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIPageControl.Appearance.CurrentPageIndicatorTintColor = UIColor.White;
            UIPageControl.Appearance.PageIndicatorTintColor = UIColor.FromRGB(246, 154, 46);

            GHWalkThroughView ghView = new GHWalkThroughView(View.Bounds);
            ghView.DataSource = new DemoSource();
            ghView.HideSkipButton();
            ghView.ShowInView(View, 0.3f);
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }
    }

    public class DemoSource : GHWalkThroughViewDataSource
    {
        public DemoSource()
        {
            NumberOfPages = 3;
        }

        public override UIImage BgImageforPage (int index)
        {
            UIImage image = null;

			if (index == 1)
				image = UIImage.FromBundle("bg-01.jpg");
			else if (index == 2)
				image = UIImage.FromBundle("bg-02.jpg");
            else
                image = UIImage.FromBundle("bg-03.jpg");

            return image;
        }

        public override void ConfigurePage(GHWalkThroughPageCell cell, int index)
        {
			cell.Title = String.Format("Some title for page {0}", index + 1);
            //cell.TitleImage = UIImage.FromBundle("bg-login");
            cell.Description = "Some Description String";
        }
    }
}