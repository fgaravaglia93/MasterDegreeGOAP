using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour
{
	public float cost=1f;
	public float eqsCost = 0f;
	public GameObject target;
    public string nameAction;

	public bool InRange { get => _inRange; set => _inRange = value; }
	private bool _inRange;

	public HashSet<KeyValuePair<string, bool>> Preconditions { get => _preConditions;}
	private HashSet<KeyValuePair<string, bool>> _preConditions;

	public HashSet<KeyValuePair<string, bool>> Effects { get => _effects;}
	private HashSet<KeyValuePair<string, bool>> _effects;

	public GoapAction() {
		_preConditions = new HashSet<KeyValuePair<string, bool>>();
		_effects = new HashSet<KeyValuePair<string, bool>>();
	}

	public abstract void OnReset();

	public abstract bool IsDone();

	public abstract bool CheckProceduralPrecondition(GameObject agent);

	public abstract bool Perform(GameObject agent);

	public abstract bool RequiresInRange();

    public abstract bool CalculateSuccess();

	public void Reset() {
		_inRange = false;
		target = null;
		OnReset();
	}

	public void AddPrecondition(string key, bool value) {
		_preConditions.Add(new KeyValuePair<string, bool>(key, value));
	}

	public void RemovePrecondition(string key) {
		foreach(KeyValuePair<string,bool> kvp in _preConditions) {
			if(kvp.Key.Equals(key)) {
                _preConditions.Remove(kvp);
				break;
			}
		}
	}

	public void AddEffect(string key, bool value) {
		Effects.Add(new KeyValuePair<string, bool>(key, value));
	}

	public void RemoveEffect(string key) {
		foreach(KeyValuePair<string, bool> kvp in Effects) {
			if(kvp.Key.Equals(key)) {
				Effects.Remove(kvp);
				break;
			}
		}
	}
}
