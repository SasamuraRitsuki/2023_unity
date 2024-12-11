//Minesweeper2.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Minesweeper2 : MonoBehaviour, IPointerClickHandler
{
    private int _rows = 0;
    private int _columns = 0;

    private int _selectedRow = 0;
    private int _selectedColumn = 0;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private CellCover _cellCoverPrefab = null;

    private CellCover[,] cellcovers;

    [SerializeField] Minesweeper _minesweeper;
    private void Start()
    {

        // _rowsの値を取得
        _rows = _minesweeper._Rows;
        // _columnsの値を取得
        _columns = _minesweeper._Columns;

        Debug.Log($"rows = {_rows}");
        Debug.Log($"_columns = {_columns}");

        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        var parent = _gridLayoutGroup.transform;
        cellcovers = new CellCover[_rows, _columns];
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellCoverPrefab);
                cell.transform.SetParent(parent);
                cellcovers[r, c] = cell;
            }
        }
    }

    public void Update()
    {
        if(_minesweeper._Finished)return;

        if (_minesweeper._MineZero)
        {
            _selectedRow = _minesweeper._SelectedRow;
            _selectedColumn = _minesweeper._SelectedColumn;
            _minesweeper._MineZero = false;
            int a = 0;
            if (TryCellCheck(_selectedRow - 1, _selectedColumn - 1));//左上
            if (TryCellCheck(_selectedRow - 1, _selectedColumn)) ;//上
            if (TryCellCheck(_selectedRow - 1, _selectedColumn + 1)) ;//右上
            if (TryCellCheck(_selectedRow, _selectedColumn - 1)) ;//左
            if (TryCellCheck(_selectedRow, _selectedColumn + 1))      ;//右
            if (TryCellCheck(_selectedRow + 1, _selectedColumn - 1)) ;//左下
            if (TryCellCheck(_selectedRow + 1, _selectedColumn));//下
            if (TryCellCheck(_selectedRow + 1, _selectedColumn + 1));//右下     
        }

        bool TryCellCheck(int row, int column)
        {
            //指定された場所にセルがあるかどうかチェック
            if (row > _rows - 1 || row < 0 || column > _columns - 1 || column < 0)
            {
                return false;
            }
            //オープンしてないセルを開ける
            else if (cellcovers[row, column]._CellCoverState != CellCoverState.Clear)
            {
                    cellcovers[row, column]._CellCoverState = CellCoverState.Clear;
                    _minesweeper._Opened = true;
                if (_minesweeper.SpaceCheck(row, column))
                {
                    _minesweeper._SelectedRow = row;
                    _minesweeper._SelectedColumn = column;
                    return true;
                }
                else { return false; }
            }
            return false;
        }
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {

        if (_minesweeper._Finished) return;

        var obj = eventData.pointerCurrentRaycast.gameObject;
        var par = obj.transform.parent.gameObject;
        var image = par.GetComponent<CellCover>();
        //クリックしたセルがどこの座標か探す
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                if (image == cellcovers[r, c])
                {
                    _selectedRow = r;
                    _selectedColumn = c;
                }
            }
        }
        //セルを左クリックで開ける
        if (eventData.button == PointerEventData.InputButton.Left) {
            image._CellCoverState = CellCoverState.Clear;
            _minesweeper._SelectedRow = _selectedRow;
            _minesweeper._SelectedColumn = _selectedColumn;
            _minesweeper._Opened = true;
        }
        //セルを右クリックでフラグにする
        if (eventData.button == PointerEventData.InputButton.Right &&
            image._CellCoverState != CellCoverState.Clear)
        {
            image._CellCoverState = image._CellCoverState == CellCoverState.Frag
                ? CellCoverState.None:CellCoverState.Frag;
        }
        ClearGame(image);
    }

    private void ClearGame(CellCover image)
    {
        int openCell = 0;
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                
                if (cellcovers[r,c]._CellCoverState == CellCoverState.Clear) openCell++;
            }
        }
        if(openCell >= _rows * _columns - _minesweeper._mineCount - 3)
        Debug.Log($"openCell={openCell}");
        if (openCell == _rows * _columns - _minesweeper._mineCount)
        {
            FullFrag();
            _minesweeper._Cleared = true;
        }
        //クリアしたら地雷のマスにフラグマークを付ける
        void FullFrag()
        {
            //Debug.Log("ここまでok");
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _columns; c++)
                {
                    if (cellcovers[r, c]._CellCoverState == CellCoverState.None && 
                        _minesweeper.MineCheck2(r,c))
                    {
                        cellcovers[r, c]._CellCoverState = CellCoverState.Frag;
                    }
                }
            }
        }
    }
}