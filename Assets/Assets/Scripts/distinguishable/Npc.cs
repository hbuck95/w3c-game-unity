using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The different roles an Npc can be.
/// 0 => CrowdMember
/// 1 => Target
/// </summary>
public enum CrowdRole {
    CrowdMember,
    Target
}

#pragma warning disable 649 //_indicationArrow is assigned via the inspector as it has been serialized.

public class Npc : MonoBehaviour {
    private NavMeshAgent _agent;
    public CrowdRole Role { get; private set; }
    public static List<Transform> destinations = new List<Transform>();
    private Transform _currentDestination;
    private static readonly System.Random _rng = new System.Random();
    [SerializeField]
    private Material[] crowdMats = new Material[5];
    [SerializeField]
    private Material targetMat;
    private MeshRenderer _mesh;
    public bool Revealed { get; private set; }
    public bool Dead { get; private set; }
    private GameObject _indicationArrow;

    /// <summary>
    /// Setup variables.
    /// </summary>
    private void Awake()
    {
        _indicationArrow = transform.GetChild(0).gameObject;
        _agent = GetComponent<NavMeshAgent>();
        _mesh = GetComponent<MeshRenderer>();
        Role = CrowdRole.CrowdMember;
    }

    /// <summary>
    /// Setup initial gameplay.
    /// </summary>
    private void Start() {
        StartCoroutine(SetDestination());
    }

   
    /// <summary>
    /// Distinguish this NPC.
    /// Changes the mesh material of this npc: if it is an enemy it is assigned extremely distinguishable enemy material,
    /// if it is a crowd member it is instead assign one of the 4 crowd member materials.
    /// </summary>
    public void Distinguish() {
        _mesh.material = Role == CrowdRole.CrowdMember ? crowdMats[_rng.Next(1, 4)] : targetMat;
        Revealed = true;
    }

    /// <summary>
    /// Manually set the role of the Npc.
    /// </summary>
    /// <param name="role">The CrowdRole for the Npc to be.</param>
    public void SetRole(CrowdRole role) {
        Role = role;
        Debug.Log(string.Format("Role has been set to '{0} for '{1}'.", role.ToString(), gameObject.name));
    }

    /// <summary>
    /// Start the _Indicate coroutine to enable the flashing arrow indicator above the npc.
    /// </summary>
    public void Indicate() {
        StartCoroutine(_Indicate());
    }

    /// <summary>
    /// Set the navmeshagents target. Includes a brief wait of 1-5 seconds between destination switches.
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator SetDestination() {
        yield return new WaitForSeconds(_rng.Next(1, 5));
        _currentDestination = destinations[_rng.Next(0, destinations.Count)];
        _agent.SetDestination(_currentDestination.position);
    }

    /// <summary>
    /// When the Npc has been assassinated we kill it.
    /// </summary>
    public void Die() {
        Dead = true;
        _agent.isStopped = true;
        StopCoroutine(SetDestination());
        Destroy(_agent);
        Destroy(_indicationArrow);
        //transform.eulerAngles.Set(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,-90f);
    }

    /// <summary>
    /// Handles trigger detection for the destinations.
    /// A trigger has been used rather than relying on the navmeshagent and its stopping distance as this way we can be sure that the Npc arrives at the correct
    /// location.
    /// </summary>
    /// <param name="c">The collider triggering this event.</param>
    private void OnTriggerEnter(Collider c) {
        if(_currentDestination == null) return;
        if (c.name == _currentDestination.gameObject.name)
            StartCoroutine(SetDestination());
    }

    /// <summary>
    /// Same use as OnTriggerEnter, however this is required if the selected destination is within the starting position of the agent.
    /// </summary>
    /// <param name="c">The collider triggering this event.</param>
    private void OnTriggerStay(Collider c) {
        if (_currentDestination == null) return;
        if (c.name == _currentDestination.gameObject.name)
            StartCoroutine(SetDestination());
    }

    private IEnumerator _Indicate() {
        while (true) {
            _indicationArrow.SetActive(true);
            yield return new WaitForSeconds(1f);
            _indicationArrow.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }


}