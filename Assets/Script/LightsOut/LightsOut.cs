using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LightsOut : MonoBehaviour, IPointerClickHandler
{
    //�Z���̏c���̑傫��
    [SerializeField]
    private int _cellLength = 3;

    private Image[, ] _cells;
    //�N���A�������ǂ���
    private bool _winjudge;

    //�\������e�L�X�g
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI clearText;

    //�o�ߎ���
    private float _TimeLeft = 0;
    //�^�[����
    private int _turnCount = 0;

    private void Start()
    {
        //�c���̐��̒���
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
    //���t���[���̏���
    public void Update()
    {
        ResetGame();
        UpdateClearText();
        UpdateTurnText();
        if (_winjudge) return;
        UpdateTimeText();
    }
    //esc�������烊�Z�b�g
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

    //�o�ߎ��Ԃ̕\��
    public void UpdateTimeText() {
        _TimeLeft += Time.deltaTime;
        timeText.text = "Time:" + _TimeLeft.ToString("f1");
    }
    //�^�[�����̕\��
    public void UpdateTurnText()
    {
        turnText.text = "Turn:" + _turnCount.ToString("d");
    }
    //�N���A�e�L�X�g�̕\��
    public void UpdateClearText()
    {
        if (_winjudge) clearText.text = "Clear!!!";
        else clearText.text = "";
    }
    //�����_���ɏ����z�u��ݒ肷��
    public void random() {
        int count = 0;
        //�����Z����3�`5�A0�̎��͂܂����I
        while ((count >= 3 && count <= 5) || count == 0) {
            count = 0;
            for (var r = 0; r < _cellLength; r++)
            {
                for (var c = 0; c < _cellLength; c++)
                {
                    int randomColor = Random.Range(0, 2);
                    _cells[r, c].color = (randomColor == 0) ? Color.black : Color.white;
                    //�����Z���̐����J�E���g���Ă���
                    if (randomColor == 1) count++;
                }
            } 
            if(count == 3) {
                //����3���������A�^�񒆂����Ȃ�1��ŏI���Ȃ��̂�break
                if (_cells[(_cellLength - 1) /2, (_cellLength - 1) / 2].color == Color.white) break;
            }
            if(count == 5 || count == 4)
            {
                //����4,5���������A�^�񒆂����Ȃ�1��ŏI���Ȃ��̂�break
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

        //�N���b�N�����Z�����ǂ��̍��W���T��
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
        //�N���b�N�����Z���̐F�𔽓]����
        image.color = (image.color == Color.white )? Color.black:Color.white;

        //1���̃Z��
        if(_selectedRow + 1 < _cellLength)
        _cells[_selectedRow + 1, _selectedColumn].color = 
            (_cells[_selectedRow + 1, _selectedColumn].color == Color.white) ? Color.black : Color.white;

        //1��̃Z��
        if(_selectedRow - 1 > -1)
        _cells[_selectedRow - 1, _selectedColumn].color =
            (_cells[_selectedRow - 1, _selectedColumn].color == Color.white) ? Color.black : Color.white;

        //1�E�̃Z��
        if (_selectedColumn + 1 < _cellLength)
            _cells[_selectedRow, _selectedColumn + 1].color =
            (_cells[_selectedRow, _selectedColumn + 1].color == Color.white) ? Color.black : Color.white;

        //1���̃Z��
        if (_selectedColumn - 1 > -1)
            _cells[_selectedRow , _selectedColumn-1].color =
                (_cells[_selectedRow, _selectedColumn-1].color == Color.white) ? Color.black : Color.white;

        ClearCheck();
        //�^�[�����̒ǉ�
        _turnCount++;
    }

    public void ClearCheck() {
        var count = 0;
        //���̐��𐔂���
        for (var r = 0; r < _cellLength; r++)
        {
            for (var c = 0; c < _cellLength; c++)
            {
                if(_cells[r, c].color == Color.black)
                    count++;

            }
        }
        //���̐����c�����Ȃ�N���A
        if (count == _cellLength * _cellLength) { 
            _winjudge = true;
            //Debug.Log("�N���A�I�I�I�I�I�I�I�I�I�I�I");
        }
    }
}