using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LightsOut : MonoBehaviour, IPointerClickHandler
{
    //セルの縦横の大きさ
    [SerializeField]
    private int _cellLength = 3;

    private Image[, ] _cells;
    //クリアしたかどうか
    private bool _winjudge;

    //表示するテキスト
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI clearText;

    //経過時間
    private float _TimeLeft = 0;
    //ターン数
    private int _turnCount = 0;

    private void Start()
    {
        //縦横の数の調整
        GridLayoutGroup FixedColumnCount = GetComponent<GridLayoutGroup>();
        FixedColumnCount.constraintCount = _cellLength;

        _cells = new Image[_cellLength,_cellLength];
        for (var r = 0; r < _cellLength; r++)
        {
            for (var c = 0; c < _cellLength; c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.parent = transform;
                
                var cell = obj.AddComponent<Image>();
                _cells[r,c] = cell;
            }
        }
        random();
    }
    //毎フレームの処理
    public void Update()
    {
        ResetGame();
        UpdateClearText();
        UpdateTurnText();
        if (_winjudge) return;
        UpdateTimeText();
    }
    //esc押したらリセット
    public void ResetGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _winjudge = false;
            _TimeLeft = 0;
            _turnCount = 0;
            random();
        }
    }

    //経過時間の表示
    public void UpdateTimeText() {
        _TimeLeft += Time.deltaTime;
        timeText.text = "Time:" + _TimeLeft.ToString("f1");
    }
    //ターン数の表示
    public void UpdateTurnText()
    {
        turnText.text = "Turn:" + _turnCount.ToString("d");
    }
    //クリアテキストの表示
    public void UpdateClearText()
    {
        if (_winjudge) clearText.text = "Clear!!!";
        else clearText.text = "";
    }
    //ランダムに初期配置を設定する
    public void random() {
        int count = 0;
        //白いセルが3～5、0の時はまた抽選
        while ((count >= 3 && count <= 5) || count == 0) {
            count = 0;
            for (var r = 0; r < _cellLength; r++)
            {
                for (var c = 0; c < _cellLength; c++)
                {
                    int randomColor = Random.Range(0, 2);
                    _cells[r, c].color = (randomColor == 0) ? Color.black : Color.white;
                    //白いセルの数をカウントしていく
                    if (randomColor == 1) count++;
                }
            } 
            if(count == 3) {
                //白が3つだった時、真ん中が白なら1手で終わらないのでbreak
                if (_cells[(_cellLength - 1) /2, (_cellLength - 1) / 2].color == Color.white) break;
            }
            if(count == 5 || count == 4)
            {
                //白が4,5つだった時、真ん中が黒なら1手で終わらないのでbreak
                if (_cells[(_cellLength - 1) / 2, (_cellLength - 1) / 2].color == Color.black) break;
            }
        }
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_winjudge) return; 
        var cell = eventData.pointerCurrentRaycast.gameObject;
        var image = cell.GetComponent<Image>();

        int _selectedRow = 0;
        int _selectedColumn = 0;

        //クリックしたセルがどこの座標か探す
        for (var r = 0; r < _cellLength; r++)
        {
            for (var c = 0; c < _cellLength; c++)
            {
                if (image == _cells[r, c])
                {
                    _selectedRow = r;
                    _selectedColumn = c;
                }
            }
        }
        //クリックしたセルの色を反転する
        image.color = (image.color == Color.white )? Color.black:Color.white;

        //1個下のセル
        if(_selectedRow + 1 < _cellLength)
        _cells[_selectedRow + 1, _selectedColumn].color = 
            (_cells[_selectedRow + 1, _selectedColumn].color == Color.white) ? Color.black : Color.white;

        //1個上のセル
        if(_selectedRow - 1 > -1)
        _cells[_selectedRow - 1, _selectedColumn].color =
            (_cells[_selectedRow - 1, _selectedColumn].color == Color.white) ? Color.black : Color.white;

        //1個右のセル
        if (_selectedColumn + 1 < _cellLength)
            _cells[_selectedRow, _selectedColumn + 1].color =
            (_cells[_selectedRow, _selectedColumn + 1].color == Color.white) ? Color.black : Color.white;

        //1個左のセル
        if (_selectedColumn - 1 > -1)
            _cells[_selectedRow , _selectedColumn-1].color =
                (_cells[_selectedRow, _selectedColumn-1].color == Color.white) ? Color.black : Color.white;

        ClearCheck();
        //ターン数の追加
        _turnCount++;
    }

    public void ClearCheck() {
        var count = 0;
        //黒の数を数える
        for (var r = 0; r < _cellLength; r++)
        {
            for (var c = 0; c < _cellLength; c++)
            {
                if(_cells[r, c].color == Color.black)
                    count++;

            }
        }
        //黒の数が縦＊横ならクリア
        if (count == _cellLength * _cellLength) { 
            _winjudge = true;
            //Debug.Log("クリア！！！！！！！！！！！");
        }
    }
}