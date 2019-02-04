using System.Collections.Generic;

/// <summary>
/// Keep a collection of every state being tracked throughout the level or on a 
/// script-by-script basis rather than having a collection of bools.
/// </summary>
public interface IStateTracking {
    #region Properties

    /// <summary>
    /// Track specific states to see if they have been triggered.
    /// </summary>
    Dictionary<string, bool> Status { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Update the specified state.
    /// </summary>
    /// <param name="state">The name of the state you want to access</param>
    /// <param name="value">The non-nullable value to set the state to.</param>
    void UpdateState(string state, bool value);
    /// <summary>
    /// Check the specified state.
    /// </summary>
    /// <param name="state">The name of the state to check.</param>
    /// <returns>True if the state has been activated, false if not, and null if the specified state does not exist. </returns>
    bool CheckState(string state);

    #endregion
}
