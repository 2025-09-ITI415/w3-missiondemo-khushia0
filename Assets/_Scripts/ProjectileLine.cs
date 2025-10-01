using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileLine : MonoBehaviour {
    // Shared across all ProjectileLine instances
    static List<ProjectileLine> PROJ_LINES = new List<ProjectileLine>();   // a
    private const float DIM_MULT = 0.75f;

    private LineRenderer _line;
    private bool _drawing = true;
    private Projectile _projectile;

    void Start() {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 1;
        _line.SetPosition(0, transform.position);

        _projectile = GetComponentInParent<Projectile>();

        AddLine(this);   // b
    }

    void FixedUpdate() {
        if (_drawing) {
            _line.positionCount++;
            _line.SetPosition(_line.positionCount - 1, transform.position);

            if (_projectile != null) {
                if (!_projectile.awake) {
                    _drawing = false;
                    _projectile = null;
                }
            }
        }
    }

    private void OnDestroy() {                       // c
        // Remove this ProjectileLine from PROJ_LINES when destroyed
        PROJ_LINES.Remove(this);
    }

    static void AddLine(ProjectileLine newLine) {
        Color col;                                  // d
        // Dim existing trails
        foreach (ProjectileLine pl in PROJ_LINES) {
            col = pl._line.startColor;              // e
            col = col * DIM_MULT;
            pl._line.startColor = pl._line.endColor = col;   // f
        }
        // Add the new trail
        PROJ_LINES.Add(newLine);                    // g
    }
}
