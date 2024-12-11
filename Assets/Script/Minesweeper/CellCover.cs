//CellCover.cs
using UnityEngine;
using UnityEngine.UI;

public enum CellCoverState
{
    None = 0,
    Frag = 1,
    Clear = 2
}
public class CellCover : MonoBehaviour
{

    [SerializeField]
    private Text _view = null;

    [SerializeField]
    private CellCoverState _cellcoverState = CellCoverState.None;
    public CellCoverState _CellCoverState
    {
        get => _cellcoverState;
        set
        {
            _cellcoverState = value;
            OnCellStateChanged();
        }
    }
    private void OnValidate()
    {
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        if (_view == null) { return; }

        else if (_cellcoverState == CellCoverState.None)
        {
            _view.text = "";
            _view.color = Color.white;
        }
        else if (_cellcoverState == CellCoverState.Frag)
        {
            _view.text = "F";
            _view.color = Color.red;
        }
        else if(_cellcoverState == CellCoverState.Clear) {
            _view.text = "";
            this.gameObject.GetComponent<Image>().color = Color.clear;
        }
        
    }
}