using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Lifegame : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int _rows = 1;

    [SerializeField]
    private int _columns = 1;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private LifegameCell _cellPrefab = null;

    private LifegameCell[,] cells;

    //���̃Z��������ɐ����Ă�����
    private int[,] alivecount;

    [SerializeField]
    // �Z�����X�V���鎞�ԊԊu�i�b�P�ʁj
    private float _duration = 1.0F;

    // ���Ԍo�߂̍X�V�����s�����ǂ���
    private bool _isPlaying = false; 

    //�o�߂�������
    private float _elapsed = 0;


    void Start()
    {
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        var parent = _gridLayoutGroup.gameObject.transform;
        cells = new LifegameCell[_rows, _columns];
        alivecount = new int[_rows,_columns];
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPlaying = !_isPlaying;
        }

        if (_isPlaying)
        {
            //elapsed��0�̎��ɍX�V
            if(_elapsed == 0)
            {
                OnNext();
            }

            _elapsed += Time.deltaTime;

            //elapsed��duration�������烊�Z�b�g
            if( _elapsed > _duration)
            {
                _elapsed = 0;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnNext();
            }
        }

    }

    private void OnNext()
    {
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                //������
                alivecount[r, c] = 0;

                //����̃`�F�b�N
                if (TryCellCheck(r - 1, c - 1)) if (TryCellAliveCheck(r - 1, c - 1)) alivecount[r,c]++;
                //��̃`�F�b�N
                if (TryCellCheck(r - 1, c))if(TryCellAliveCheck(r - 1, c)) alivecount[r, c]++;
                //�E��̃`�F�b�N
                if (TryCellCheck(r - 1, c + 1)) if (TryCellAliveCheck(r - 1, c + 1)) alivecount[r, c]++;
                //���̃`�F�b�N
                if (TryCellCheck(r, c - 1)) if (TryCellAliveCheck(r, c - 1)) alivecount[r, c]++;
                //�E�̃`�F�b�N
                if (TryCellCheck(r, c + 1)) if (TryCellAliveCheck(r, c + 1)) alivecount[r, c]++;
                //�����̃`�F�b�N
                if (TryCellCheck(r + 1, c - 1)) if (TryCellAliveCheck(r + 1, c - 1)) alivecount[r, c]++;
                //�����̃`�F�b�N
                if (TryCellCheck(r + 1, c)) if (TryCellAliveCheck(r + 1, c)) alivecount[r, c]++;
                //�����̃`�F�b�N
                if (TryCellCheck(r + 1, c + 1)) if (TryCellAliveCheck(r + 1, c + 1)) alivecount[r, c]++;


            }
        }

        //�F�`�F���W
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {

                if (cells[r, c].State == LifegameCellState.Dead)
                {
                    //�א�3�Ȃ�a��
                    if (alivecount[r, c] == 3)
                    {
                        cells[r, c].State = LifegameCellState.Alive;
                    }
                }
                else
                {
                    //Debug.Log($"alivecount = {alivecount}");
                    //�א�2�ȉ���4�ȏ�Ȃ玀��
                    if (alivecount[r, c] <= 1 || alivecount[r, c] >= 4)
                    {
                        cells[r, c].State = LifegameCellState.Dead;
                    }
                }
            }
        }
    }    

    private bool TryCellCheck(int row,int column)
    {
        //�w�肳�ꂽ�ꏊ�ɃZ�������邩�ǂ����`�F�b�N
        if (row > _rows - 1 || row < 0 || column > _columns - 1 || column < 0)
        {
            //Debug.Log("bbbb");
            return false;
        }
        else return true;
    }

    private bool TryCellAliveCheck(int row, int column)
    {
        if (cells[row,column].State == LifegameCellState.Alive)
        {
            return true;
        }
        else return false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("aaaa");
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (target.TryGetComponent<LifegameCell>(out var cell))
        {
            cell.State = cell.State == LifegameCellState.Alive ? LifegameCellState.Dead : LifegameCellState.Alive;
        }
    }
}