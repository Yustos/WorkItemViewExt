using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YL.Timeline.Interaction
{
	public class LoaderCommand : ICommand
	{
		private readonly RevisionLoader.LoadInfoDelegate _loader;

		public LoaderCommand(RevisionLoader.LoadInfoDelegate loader)
		{
			_loader = loader;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			
		}

		public Field[] LoadInfo(int id, int rev)
		{
			return _loader(id, rev);
		}
	}
}
