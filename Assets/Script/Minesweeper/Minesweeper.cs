//Minesweeper.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minesweeper : MonoBehaviour
{
    [SerializeField]
    private int _rows = 10;

    [SerializeField]
    private int _columns = 10;


    private int _selectedRow = 100;
    private int _selectedColumn = 0;
    
    //開けたかどうか
    private bool _opened = false;
    //ゲームオーバーになったかどうか
    private bool _finished = false;
    //ゲームクリアになったかどうか
    private bool _cleared = false;
    //数字が0たかどうか
    private bool _mineZero = false;
    //最初のターンかどうか
    private bool _firstTurn = false;

    //表示するテキスト
    public TextMeshProUGUI timeText;
    //経過時間
    private float _TimeLeft = 0;
    //地雷の数
    [SerializeField]
    public int _mineCount = 1;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private Cell _cellPrefab = null;

    private Cell[,] cells;


    public int _MineCount {
        get => _mineCount;
    }
    public int _Rows
    {
        get => _rows;
        set
        {
            _rows = value;
        }
    }
    public int _Columns
    {
        get => _columns;
        set
        {
            _columns = value;
        }
    }

    public int _SelectedRow
    {
        get => _selectedRow;
        set
        {
            _selectedRow = value;
        }
    }
    public int _SelectedColumn
    {
        get => _selectedColumn;
        set
        {
            _selectedColumn = value;
        }
    }

    public bool _Finished
    {
        get => _finished;
        set
        {
            _finished = value;
        }
    }

    public bool _Opened
    {
        get => _opened;
        set
        {
            _opened = value;
        }
    }

    public bool _Cleared
    {
        get => _cleared;
        set
        {
            _cleared = value;
        }
    }

    public bool _MineZero
    {
        get => _mineZero;
        set
        {
            _mineZero = value;
        }
    }

    

    private void Start()
    {

        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        cells = new Cell[_rows, _columns];
        var parent = _gridLayoutGroup.transform;
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cells[r, c] = cell;
            }
        }
    }

    private void Update()
    {
        if(_finished) return;
        UpdateTimeText();
        if (_opened) CellOpen();
    }

    private void MinePlacedStart()
    {
        //地雷配置
        for (var i = 0; i < _mineCount; i++)
        {
            //ランダム配置
            //被ってたら別のセルへ
            MinePlaced(out var r, out var c);

            var cell = cells[r, c];
            cell._CellState = CellState.Mine;
        }

        //周辺の地雷の数を数える
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                if (cells[r, c]._CellState != CellState.Mine)
                {
                    MineCheck(r, c);
                }
            }
        }
    }
    //ランダム配置
    private void MinePlaced(out int row, out int colum)
    {
        int r = Random.Range(0, _rows);
        int c = Random.Range(0, _columns);
        //ランダムで選んだセルに地雷があったらループ
        //ランダムで選んだセルが選択したセルだったらループ
        while ((cells[r, c]._CellState == CellState.Mine　||
            (r == _selectedRow && c == _SelectedColumn)))
        {
            r = Random.Range(0, _rows);
            c = Random.Range(0, _columns);
        }
        row = r;
        colum = c;
    }
    //周辺の地雷を数える
    private void MineCheck(int row,int column) {
        int minecount = 0;
        if (TryCellCheck(row - 1, column)) minecount++;//上
        if (TryCellCheck(row - 1, column + 1)) minecount++;//右上
        if (TryCellCheck(row - 1, column - 1)) minecount++;//左上
        if (TryCellCheck(row, column - 1)) minecount++;//左
        if (TryCellCheck(row, column + 1)) minecount++;//右
        if (TryCellCheck(row + 1, column - 1)) minecount++;//左下
        if (TryCellCheck(row + 1, column + 1)) minecount++;//右下
        if (TryCellCheck(row + 1, column)) minecount++;//下
        //何個あるか表示
        cells[row, column]._CellState = (CellState)minecount;
    }
    
    private bool TryCellCheck(int row,int column) {
        //指定された場所にセルがあるかどうかチェック
        if (row > _rows - 1 || row < 0 || column > _columns - 1 || column < 0)
        {
            return false;
        }
        //更にそのセルに地雷があるかどうかチェック
        else if (cells[row, column]._CellState == CellState.Mine)
        {
            return true;
        }
        else return false;
    }

    public void CellOpen()
    {
        //最初のクリックに応じて地雷配置
        if(!_firstTurn)
        {
            MinePlacedStart();
            _firstTurn = true;
        }
        //空白セルだったら周りを開けるフラグをon
        if (cells[_selectedRow, _selectedColumn]._CellState == CellState.None)
        {
            _mineZero = true;
        }
        //地雷セルだったら終了
        else if (cells[_selectedRow, _selectedColumn]._CellState == CellState.Mine)
        {
            GameOver();
        }
        else if (_cleared) GameClear();
        _opened = false;
    }
    //そのセルが空白かチェック
    public bool SpaceCheck(int r, int c)
    {
        if (cells[r, c]._CellState == CellState.None)
        {
            return true;
        }
            return false;
    }
    //そのセルが地雷かチェック
    public bool MineCheck2(int r, int c)
    {
        if (cells[r, c]._CellState == CellState.Mine)
        {
            return true;
        }
        return false;
    }
    private void GameOver()
    {
        _finished = true;
        Debug.Log($"ゲームオーバー！");
    }
    private void GameClear()
    {
        _finished = true;
        Debug.Log($"ゲームクリア！");
    }

    //経過時間の表示
    public void UpdateTimeText()
    {
        _TimeLeft += Time.deltaTime;
        timeText.text = "Time:" + _TimeLeft.ToString("f1");
    }
}