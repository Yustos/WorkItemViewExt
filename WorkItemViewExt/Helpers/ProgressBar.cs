using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.Helpers
{
	internal class ProgressBar : IDisposable
	{
		private readonly IVsStatusbar _statusBar;
		
		private uint _progressCookie;
		private uint _nodesCount;
		private uint _linksCount;

		internal ProgressBar(IServiceProvider provider)
		{
			_statusBar = (IVsStatusbar)provider.GetService(typeof(SVsStatusbar));
			Progress(0, 0);
		}

		internal void Progress(int nodesCount, int linksCount)
		{
			_nodesCount += (uint)nodesCount;
			_linksCount += (uint)linksCount;
			_statusBar.Progress(ref _progressCookie, 1, string.Format(Resources.ProgressMessage, _nodesCount, _linksCount), 0, 100);
		}

		public void Dispose()
		{
			_statusBar.Progress(ref _progressCookie, 0, string.Format(Resources.SuccessMessage, _nodesCount, _linksCount), 0, 100);
		}
	}
}
