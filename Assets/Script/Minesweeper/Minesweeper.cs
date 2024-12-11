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
    
    //�J�������ǂ���
    private bool _opened = false;
    //�Q�[���I�[�o�[�ɂȂ������ǂ���
    private bool _finished = false;
    //�Q�[���N���A�ɂȂ������ǂ���
    private bool _cleared = false;
    //������0�����ǂ���
    private bool _mineZero = false;
    //�ŏ��̃^�[�����ǂ���
    private bool _firstTurn = false;

    //�\������e�L�X�g
    public TextMeshProUGUI timeText;
    //�o�ߎ���
    private float _TimeLeft = 0;
    //�n���̐�
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
        //�n���z�u
        for (var i = 0; i < _mineCount; i++)
        {
            //�����_���z�u
            //����Ă���ʂ̃Z����
            MinePlaced(out var r, out var c);

            var cell = cells[r, c];
            cell._CellState = CellState.Mine;
        }

        //���ӂ̒n���̐��𐔂���
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
    //�����_���z�u
    private void MinePlaced(out int row, out int colum)
    {
        int r = Random.Range(0, _rows);
        int c = Random.Range(0, _columns);
        //�����_���őI�񂾃Z���ɒn�����������烋�[�v
        //�����_���őI�񂾃Z�����I�������Z���������烋�[�v
        while ((cells[r, c]._CellState == CellState.Mine�@||
            (r == _selectedRow && c == _SelectedColumn)))
        {
            r = Random.Range(0, _rows);
            c = Random.Range(0, _columns);
        }
        row = r;
        colum = c;
    }
    //���ӂ̒n���𐔂���
    private void MineCheck(int row,int column) {
        int minecount = 0;
        if (TryCellCheck(row - 1, column)) minecount++;//��
        if (TryCellCheck(row - 1, column + 1)) minecount++;//�E��
        if (TryCellCheck(row - 1, column - 1)) minecount++;//����
        if (TryCellCheck(row, column - 1)) minecount++;//��
        if (TryCellCheck(row, column + 1)) minecount++;//�E
        if (TryCellCheck(row + 1, column - 1)) minecount++;//����
        if (TryCellCheck(row + 1, column + 1)) minecount++;//�E��
        if (TryCellCheck(row + 1, column)) minecount++;//��
        //�����邩�\��
        cells[row, column]._CellState = (CellState)minecount;
    }
    
    private bool TryCellCheck(int row,int column) {
        //�w�肳�ꂽ�ꏊ�ɃZ�������邩�ǂ����`�F�b�N
        if (row > _rows - 1 || row < 0 || column > _columns - 1 || column < 0)
        {
            return false;
        }
        //�X�ɂ��̃Z���ɒn�������邩�ǂ����`�F�b�N
        else if (cells[row, column]._CellState == CellState.Mine)
        {
            return true;
        }
        else return false;
    }

    public void CellOpen()
    {
        //�ŏ��̃N���b�N�ɉ����Ēn���z�u
        if(!_firstTurn)
        {
            MinePlacedStart();
            _firstTurn = true;
        }
        //�󔒃Z���������������J����t���O��on
        if (cells[_selectedRow, _selectedColumn]._CellState == CellState.None)
        {
            _mineZero = true;
        }
        //�n���Z����������I��
        else if (cells[_selectedRow, _selectedColumn]._CellState == CellState.Mine)
        {
            GameOver();
        }
        else if (_cleared) GameClear();
        _opened = false;
    }
    //���̃Z�����󔒂��`�F�b�N
    public bool SpaceCheck(int r, int c)
    {
        if (cells[r, c]._CellState == CellState.None)
        {
            return true;
        }
            return false;
    }
    //���̃Z�����n�����`�F�b�N
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
        Debug.Log($"�Q�[���I�[�o�[�I");
    }
    private void GameClear()
    {
        _finished = true;
        Debug.Log($"�Q�[���N���A�I");
    }

    //�o�ߎ��Ԃ̕\��
    public void UpdateTimeText()
    {
        _TimeLeft += Time.deltaTime;
        timeText.text = "Time:" + _TimeLeft.ToString("f1");
    }
}