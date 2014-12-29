using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YL.Timeline.Interfaces;

namespace YL.Timeline.Shell
{
	internal class DataService : IDataService
	{
		public string[] GetFields()
		{
			return new[]
			{
				"Field 2",
				"Field 3",
				"Field 1"
			};
		}
	}
}
