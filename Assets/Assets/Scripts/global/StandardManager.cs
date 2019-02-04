using System;
using System.Linq;
using UnityEngine;

public struct Standard {
    public readonly StandardType Type;
    public readonly Requirement[] Requirements;

    public Standard(StandardType type, Requirement[] requirements) {
        Type = type;
        Requirements = requirements;
    }

    public Requirement DefaultRequirement() {
        return Requirements[0];
    }

    public Requirement GetRequirement(int requirement) {
        return Requirements[requirement];
    }
}

public static class StandardManager {
    public static Standard A = new Standard(StandardType.A, new[] { Requirement.Perceivable, Requirement.Operable, Requirement.Understandable, Requirement.Robust });
    public static Standard AA = new Standard(StandardType.AA, new[] { Requirement.None });
    public static Standard AAA = new Standard(StandardType.AAA, new []{Requirement.None });
    private static Requirement _selectedRequirement = Requirement.None;
    public static Requirement SelectedRequirement {
        get {
            return _selectedRequirement;
        }
        private set {
            _selectedRequirement = value;
        }

    }
    public static Standard SelectedStandard { get; private set; }

    public static Standard GetStandard(StandardType type){
        switch (type){
            case StandardType.A:
                return A;
            case StandardType.AA:
                return AA;
            case StandardType.AAA:
                return AAA;
            default:
                Debug.LogErrorFormat("Unable to find a valid standard for type '{0}'", type);
                return new Standard();
        }
    }

    /// <summary>
    /// Get an array of all the levels in each requirement.
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public static Level[] GetLevelsForRequirement(Requirement r) {
        switch (r) {
            default:
                Debug.LogErrorFormat("No levels found for Requirement '{0}'.", r);
                return null;
            case Requirement.Perceivable:
                return new[] {
                    Level.TextAlternatives,
                    Level.TimeBasedMedia,
                    Level.Adaptable,
                    Level.Distinguishable
                };
            case Requirement.Operable:
                return new[] {
                    Level.KeyboardAccessible,
                    Level.EnoughTime,
                    Level.Seizures,
                    Level.Navigable
                };
            case Requirement.Understandable:
                return new[] {
                    Level.Readable,
                    Level.Predictable,
                    Level.InputAssistance
                };
            case Requirement.Robust:
                return new[] { Level.Compatible };
        }

    }

    /// <summary>
    /// Gets the next uncompleted requirement for the specified standard.
    /// If all the requirements have been completed loads the last requirement in the standard and logs a warning.
    /// </summary>
    /// <param name="s">The standard to check the requirements of.</param>
    /// <returns></returns>
    public static Requirement GetNextUncompletedRequirement(Standard s) {
        foreach (var requirement in s.Requirements) {
            if (!CheckCompletion(requirement)) {
                Debug.Log(string.Format("{0} has NOT yet been completed.", requirement));
                return requirement;
            }

            Debug.Log(string.Format("{0} HAS been completed.", requirement));
        }

        Debug.LogWarningFormat("All requirements have been completed for {0}. Loading the last requirement.", s.Type);
        return s.Requirements.Last();
    }

    public static void SelectStandard(Standard standard) {
        SelectedStandard = standard;
    }

    public static void SelectRequirement(Requirement requirement) {
        SelectedRequirement = requirement;
    }

    /// <summary>
    /// Temporary function. Replace when DB is added.
    /// </summary>
    /// <param name="r"></param>
    public static bool CheckCompletion(Requirement r) {
        return PlayerPrefs.HasKey(r.ToString()) && Convert.ToBoolean(PlayerPrefs.GetInt(r.ToString()));
    }

    /// <summary>
    /// Temporary function. Replace when DB is added.
    /// </summary>
    /// <param name="r"></param>
    public static bool CheckCompletion(StandardType s) {
        return PlayerPrefs.HasKey(s.ToString()) && Convert.ToBoolean(PlayerPrefs.GetInt(s.ToString()));
    }

    /// <summary>
    /// Temporary function. Replace when DB is added.
    /// </summary>
    /// <param name="r"></param>
    public static bool CheckCompletion(Level l) {
        return PlayerPrefs.HasKey(l.ToString()) && Convert.ToBoolean(PlayerPrefs.GetInt(l.ToString()));
    }

    /// <summary>
    /// Temporary function. Replace when DB is added.
    /// </summary>
    /// <param name="r"></param>
    public static void MarkComplete(Requirement r) {
        PlayerPrefs.SetInt(r.ToString(), 1);
    }

    /// <summary>
    /// Temporary function. Replace when DB is added.
    /// </summary>
    /// <param name="r"></param>
    public static void MarkComplete(StandardType s) {
        PlayerPrefs.SetInt(s.ToString(), 1);
    }

    /// <summary>
    /// Temporary function. Replace when DB is added.
    /// </summary>
    /// <param name="r"></param>
    public static void MarkComplete(Level l) {
        PlayerPrefs.SetInt(l.ToString(), 1);
    }

}
/// <summary>
/// The gameplay level to load.
/// </summary>
public enum Level {
    //Split into WCAG requirements.
    //When making these levels follow the enum naming convention (e.g. name the InputAssistance scene "InputAssistance")

    /*Other*/
    None,
    TitleMenu, //The main menu

    /*Perceivable*/
    TextAlternatives,
    TimeBasedMedia,
    Adaptable,
    Distinguishable,

    /*Operable*/
    KeyboardAccessible,
    EnoughTime,
    Seizures,
    Navigable,

    /*Understandable*/
    Readable, //<--- has not been made
    Predictable,
    InputAssistance, //<--- has not been made

    /*Robust*/
    Compatible //<--- has not been made
}

/// <summary>
/// The W3C requirements used to identify floors (hallways)
/// </summary>
public enum Requirement
{
    None,
    Perceivable,
    Operable,
    Understandable,
    Robust
}

/// <summary>
/// W3C Standard
/// </summary>
public enum StandardType {
    A = 1,
    AA = 2,
    AAA = 3
}