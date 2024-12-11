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

    //何個のセルが周りに生きていたか
    private int[,] alivecount;

    [SerializeField]
    // セルを更新する時間間隔（秒単位）
    private float _duration = 1.0F;

    // 時間経過の更新が実行中かどうか
    private bool _isPlaying = false; 

    //経過した時間
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
            //elapsedが0の時に更新
            if(_elapsed == 0)
            {
                OnNext();
            }

            _elapsed += Time.deltaTime;

            //elapsedがduration超えたらリセット
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
                //初期化
                alivecount[r, c] = 0;

                //左上のチェック
                if (TryCellCheck(r - 1, c - 1)) if (TryCellAliveCheck(r - 1, c - 1)) alivecount[r,c]++;
                //上のチェック
                if (TryCellCheck(r - 1, c))if(TryCellAliveCheck(r - 1, c)) alivecount[r, c]++;
                //右上のチェック
                if (TryCellCheck(r - 1, c + 1)) if (TryCellAliveCheck(r - 1, c + 1)) alivecount[r, c]++;
                //左のチェック
                if (TryCellCheck(r, c - 1)) if (TryCellAliveCheck(r, c - 1)) alivecount[r, c]++;
                //右のチェック
                if (TryCellCheck(r, c + 1)) if (TryCellAliveCheck(r, c + 1)) alivecount[r, c]++;
                //左下のチェック
                if (TryCellCheck(r + 1, c - 1)) if (TryCellAliveCheck(r + 1, c - 1)) alivecount[r, c]++;
                //左下のチェック
                if (TryCellCheck(r + 1, c)) if (TryCellAliveCheck(r + 1, c)) alivecount[r, c]++;
                //左下のチェック
                if (TryCellCheck(r + 1, c + 1)) if (TryCellAliveCheck(r + 1, c + 1)) alivecount[r, c]++;


            }
        }

        //色チェンジ
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {

                if (cells[r, c].State == LifegameCellState.Dead)
                {
                    //隣接3なら誕生
                    if (alivecount[r, c] == 3)
                    {
                        cells[r, c].State = LifegameCellState.Alive;
                    }
                }
                else
                {
                    //Debug.Log($"alivecount = {alivecount}");
                    //隣接2以下と4以上なら死滅
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
        //指定された場所にセルがあるかどうかチェック
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