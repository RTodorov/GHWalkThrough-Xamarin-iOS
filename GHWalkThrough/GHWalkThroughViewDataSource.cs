using System;
using MonoTouch.UIKit;

namespace MonoControls.GHWalkThrough
{
	public abstract class GHWalkThroughViewDataSource
	{
		public int NumberOfPages { get; set; }

		public abstract UIImage BgImageforPage(int index);

		public abstract void ConfigurePage(GHWalkThroughPageCell cell, int index);
	}
}