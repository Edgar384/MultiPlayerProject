using UnityEngine;

/// <summary> 
/// To access the heir by a static field "Instance".
/// </summary>
public abstract class MonoSingleton<T> :MonoBehaviour where T : MonoSingleton<T>
{

	[SerializeField] private bool _dontDestroyOnLoad;

	public static T Instance { get; private set; }

	void Awake ()
	{
		if (Instance == null)
		{
			Instance = this as T;
			if (_dontDestroyOnLoad)
			{
				DontDestroyOnLoad (gameObject);
			}
			AwakeSingleton ();
		}
		else
		{
			Destroy (gameObject.GetComponent<T> ());
		}
	}
	protected abstract void AwakeSingleton ();
}
