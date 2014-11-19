using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MonoControls.GHWalkThrough
{
    [Register("GHWalkThroughPageCell")]
    public class GHWalkThroughPageCell : UICollectionViewCell
    {
		private string title;
		public string Title
		{
			get {
				return title;
			}
			set {
				title = value;

				if (titleLabel != null) {
					titleLabel.Text = title;
				}

				SetNeedsLayout();
			}
		}

		private UIFont titleFont;
		public UIFont TitleFone
		{
			get {
				return titleFont;
			}
			set {
				titleFont = value;
				titleLabel.Font = titleFont;
			}
		}

		private UIColor titleColor;
		public UIColor TitleColor
		{
			get {
				return titleColor;
			}
			set {
				titleColor = value;
				titleLabel.TextColor = titleColor;
			}
		}

		private UIImage titleImage;
		public UIImage TitleImage
		{
			get {
				return titleImage;
			}
			set {
				titleImage = value;

				if (titleImageView != null)
					titleImageView.Image = titleImage;

				SetNeedsLayout();
			}
		}

		private float titlePositionY;
		public float TitlePositionY
		{
			get {
				return titlePositionY;
			}
			set {
				titlePositionY = value;

				layoutTitleLabel();
			}
		}

		public int TitleLines
		{
			get {
				return titleLabel.Lines;
			}
			set {
				titleLabel.Lines = value;
				layoutTitleLabel();
			}
		}

		private string description;
		public string Description
		{
			get {
				return description;
			}
			set {
				description = value;

				if (descriptionLabel != null)
					descriptionLabel.Text = description;

				SetNeedsLayout();
			}
		}

		private UIFont descriptionFont;
		public UIFont DescriptionFont
		{
			get {
				return descriptionFont;
			}
			set {
				descriptionFont = value;

				descriptionLabel.Font = descriptionFont;
			}
		}

		private UIColor descriptionColor;
		public UIColor DescriptionColor
		{
			get {
				return descriptionColor;
			}
			set {
				descriptionColor = value;

				descriptionLabel.TextColor = descriptionColor;
			}
		}

		private float descriptionPositionY;
		public float DescriptionPositionY
		{
			get {
				return descriptionPositionY;
			}
			set {
				descriptionPositionY = value;

				layoutDescriptionLabel ();
			}
		}

		private float imgPositionY;

        private UILabel titleLabel;
		private UITextView descriptionLabel;
        private UIImageView titleImageView;

        public GHWalkThroughPageCell(IntPtr handle) : base(handle)
        {
            applyDefaults();
            buildUI();
        }

        public GHWalkThroughPageCell(RectangleF frame) : base(frame)
        {
            applyDefaults();
            buildUI();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            RectangleF rect1 = titleImageView.Frame;
            rect1.X = (ContentView.Frame.Width - rect1.Width) / 2;
            rect1.Y = Frame.Height - titlePositionY - imgPositionY - rect1.Height;
            titleImageView.Frame = rect1;

            layoutTitleLabel();

			descriptionLabel.Frame = new RectangleF(20, Frame.Height - descriptionPositionY, ContentView.Frame.Width - 40, 500);
        }

        private void layoutTitleLabel()
        {
            float titleHeight;

            /*if (self.title respondsToSelector:@selector(boundingRectWithSize:options:attributes:context:)]) {
                NSAttributedString *attributedText = [[NSAttributedString alloc] initWithString:self.title attributes:@{ NSFontAttributeName: self.titleFont }];
                CGRect rect = [attributedText boundingRectWithSize:(CGSize){self.contentView.frame.size.width - 20, CGFLOAT_MAX} options:NSStringDrawingUsesLineFragmentOrigin context:nil];
                titleHeight = ceilf(rect.size.height);
            } else {
                #pragma clang diagnostic push
                #pragma clang diagnostic ignored "-Wdeprecated-declarations"
                titleHeight = [self.title sizeWithFont:self.titleFont constrainedToSize:CGSizeMake(self.contentView.frame.size.width - 20, CGFLOAT_MAX) lineBreakMode:NSLineBreakByWordWrapping].height;
                #pragma clang diagnostic pop
            }*/

			titleHeight = new NSString(title).StringSize(titleFont, new SizeF(ContentView.Frame.Width - 20, float.MaxValue), UILineBreakMode.WordWrap).Height;

            titleLabel.Frame = new RectangleF(10, Frame.Height - titlePositionY, ContentView.Frame.Width - 20, titleHeight);
        }

		private void layoutDescriptionLabel()
		{
			descriptionLabel.Frame = new RectangleF(20, Frame.Height - descriptionPositionY, ContentView.Frame.Width - 40, 500);
		}

		private void applyDefaults ()
        {
			title = description = "";

            imgPositionY = 50.0f;
            titlePositionY = 180.0f;
            descriptionPositionY = 160.0f;

            titleFont = UIFont.FromName("HelveticaNeue-Bold", 16.0f);
            titleColor = UIColor.White;
            
			descriptionFont = UIFont.FromName("HelveticaNeue", 13.0f);
            descriptionColor = UIColor.White;
        }

        private void buildUI ()
        {
            BackgroundColor = UIColor.Clear;
            BackgroundView = new UIView();
            ContentView.BackgroundColor = UIColor.Clear;

            UIView pageView = new UIView(ContentView.Bounds);

            if (titleImageView == null)
            {
                titleImageView = titleImage != null ? new UIImageView(titleImage) : new UIImageView(new RectangleF(0, 0, 128, 128));
            }

            pageView.AddSubview(titleImageView);

            if (titleLabel == null)
            {
                UILabel _titleLabel = new UILabel();
                _titleLabel.Text = title;
                _titleLabel.Font = titleFont;
                _titleLabel.TextColor = titleColor;
                _titleLabel.BackgroundColor = UIColor.Clear;
                _titleLabel.TextAlignment = UITextAlignment.Center;
                _titleLabel.LineBreakMode = UILineBreakMode.WordWrap;
                pageView.AddSubview(_titleLabel);
                titleLabel = _titleLabel;
            }

            if (descriptionLabel == null)
            {
                UITextView _descLabel = new UITextView();
                _descLabel.Text = description;
                _descLabel.ScrollEnabled = false;
                _descLabel.Font = descriptionFont;
                _descLabel.TextColor = descriptionColor;
                _descLabel.BackgroundColor = UIColor.Clear;
                _descLabel.TextAlignment = UITextAlignment.Center;
                _descLabel.UserInteractionEnabled = false;
                pageView.AddSubview(_descLabel);
                descriptionLabel = _descLabel;
            }

            ContentView.AddSubview(pageView);
        }
    }
}