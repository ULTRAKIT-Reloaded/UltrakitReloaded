using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

namespace ULTRAKIT.Extensions
{
	public interface IBuff
	{
		string id { get; }
		EnemyIdentifier eid { get; set; }
        bool IsActive { get; }

        void Enable();

		void Disable();

		void Update();
	}
}
