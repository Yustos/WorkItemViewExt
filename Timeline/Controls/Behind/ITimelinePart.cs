using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YL.Timeline.Controls.Behind
{
	public interface ITimelinePart
	{
		void SetController(ViewportController controller);

		void UpdateViewport();
	}
}
