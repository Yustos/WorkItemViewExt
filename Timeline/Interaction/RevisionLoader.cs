using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.Timeline.Interaction
{
	public class RevisionLoader
	{
		public delegate Field[] LoadInfoDelegate(int workItemId, int rev);
	}
}
