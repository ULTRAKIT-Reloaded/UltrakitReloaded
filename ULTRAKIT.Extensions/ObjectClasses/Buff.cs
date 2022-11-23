using System;

namespace ULTRAKIT.Extensions
{
	public class Buff : IBuff
	{
		private bool active;

        public Action EnableScript;
		public Action DisableScript;
		public Action UpdateScript;

        public bool IsActive => active;

		public string id { get; set; }
		public EnemyIdentifier eid { get; set; }

        public void Enable()
		{
			active = true;
			EnableScript();
		}

		public void Disable()
		{
			active = false;
			DisableScript();
		}

		public void Update()
		{
			UpdateScript();
		}
	}
}
