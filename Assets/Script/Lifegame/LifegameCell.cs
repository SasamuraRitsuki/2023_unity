using UnityEngine;
using UnityEngine.UI;

public enum LifegameCellState
{
    Dead,
    Alive,
}

public class LifegameCell : MonoBehaviour
{
    [SerializeField]
    private Image _image = null;

    [SerializeField]
    private Color _aliveColor = Color.black;

    [SerializeField]
    private Color _deadColor = Color.white;

    [SerializeField]
    private LifegameCellState _state = LifegameCellState.Dead;

    public LifegameCellState State
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChanged();
        }
    }

    private void OnValidate()
    {
        OnStateChanged();
    }

    private void OnStateChanged()
    {
        if (_image == null) { return; }
        _image.color = (State == LifegameCellState.Alive) ? _aliveColor : _deadColor;
    }
}
