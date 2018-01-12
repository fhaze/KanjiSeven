using System;
using System.Collections.Generic;
using Gdk;
using Gtk;
using KanjiSeven.Extensions;
using KanjiSeven.Models;
using KanjiSeven.Services;
using KanjiSeven.Widgets;
using Key = Gtk.Key;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class FlashCardForm : BaseWindow
    {
        private readonly VBox   _mainVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly VBox   _frameVerticalBox  = new VBox { BorderWidth = 10 };
        private readonly VBox   _hintVerticalBox   = new VBox { BorderWidth = 10, HeightRequest = 80};
        private readonly HBox   _descriptionHBox   = new HBox { BorderWidth = 10 };
        private readonly Label  _currentCardIndex  = new Label();
        private readonly Label  _scoreDescription  = new Label();
        private readonly Label  _cardLabelTango    = new Label("｡ﾟﾟ(」｡≧□≦)」").SetFontSize(92);
        private readonly Label  _cardLabelFurigana = new Label(string.Empty).SetFontSize(20);
        private readonly Label  _cardLabelRomaji   = new Label(string.Empty);
        private readonly Label  _cardLabelHonyaku  = new Label(string.Empty);
        private readonly Button _startButton       = new Button { Label = "スタート" };
        private readonly Button _backButton        = new Button { Label = "やめろ" };

        private readonly AccelGroup _accelGroup = new AccelGroup();
        
        // 推測ゲーム
        private readonly Color          _defaultColor    = new Color(255, 255, 255);
        private readonly Color          _correctColor    = new Color(150, 255, 150);
        private readonly Color          _wrongColor      = new Color(255, 150, 150);
        private readonly HButtonBox     _guessTopBox     = new HButtonBox { Layout = ButtonBoxStyle.Center, Spacing = 5};
        private readonly HButtonBox     _guessBottomBox  = new HButtonBox { Layout = ButtonBoxStyle.Center, Spacing = 5 };
        private readonly List<FhButton> _guessButtonList = new List<FhButton>
        {
            new FhButton(),
            new FhButton(),
            new FhButton(),
            new FhButton(),
            new FhButton(),
            new FhButton(),
        };

        private readonly Configuration    _configuration = ConfigManager.Current;
        private readonly FlashCardService _flashCardService = FlashCardService.Current;
        
        public FlashCardForm(Window parent) : base("ゲーム")
        {
            SetSizeRequest(900, 550);
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);
            AddAccelGroup(_accelGroup);

            _descriptionHBox.PackStart(_currentCardIndex, false, false, 0);
            
            _mainVerticalBox.PackStart(_descriptionHBox, false, true, 0);
            
            _frameVerticalBox.PackStart(new Label(), true, false, 0);
            _frameVerticalBox.PackStart(_cardLabelTango, false, false, 20);
            _hintVerticalBox.PackStart(_cardLabelFurigana, false, false, 0);
            _hintVerticalBox.PackStart(_cardLabelRomaji, false, false, 0);
            _hintVerticalBox.PackStart(_cardLabelHonyaku, false, false, 0);
            _frameVerticalBox.PackStart(_hintVerticalBox, false, false, 0);
            _frameVerticalBox.PackStart(new Label(), true, false, 0);
            
            _mainVerticalBox.PackStart(_frameVerticalBox, true, true, 0);

            switch (_configuration.GameMode)
            {
                case GameMode.Simple:
                    break;
                case GameMode.GuessMode:
                    _descriptionHBox.PackStart(_scoreDescription);
                    var key = new List<Gdk.Key>
                    {
                        Gdk.Key.q,
                        Gdk.Key.w,
                        Gdk.Key.e,
                        Gdk.Key.a,
                        Gdk.Key.s,
                        Gdk.Key.d
                    };
                
                    for (var i = 0; i < _guessButtonList.Count; i++)
                    {
                        _guessButtonList[i].SetButtonColor(_defaultColor);
                        _guessButtonList[i].Label = $"回答{i}";
                        _guessButtonList[i].HeightRequest = 40;
                        _guessButtonList[i].WidthRequest = 200;
                        _guessButtonList[i].Sensitive = false;
                        _guessButtonList[i].AddAccelerator("activate", _accelGroup,
                            new AccelKey(key[i], ModifierType.None, AccelFlags.Visible));
                        _guessButtonList[i].Clicked += OnClicked;
                    }
                    _guessTopBox.PackStart(_guessButtonList[0], false, false, 10);
                    _guessTopBox.PackStart(_guessButtonList[1], false, false, 10);
                    _guessTopBox.PackStart(_guessButtonList[2], false, false, 10);
                
                    _guessBottomBox.PackStart(_guessButtonList[3], false, false, 10);
                    _guessBottomBox.PackStart(_guessButtonList[4], false, false, 10);
                    _guessBottomBox.PackStart(_guessButtonList[5], false, false, 10);
                
                    _mainVerticalBox.PackStart(_guessTopBox, false, false, 0);
                    _mainVerticalBox.PackStart(_guessBottomBox, false, false, 0);
                    break;
                case GameMode.InputMode:
                    break;
            }

            _mainVerticalBox.PackStart(new HSeparator(), false, true, 5);
            
            var hbbox = new HButtonBox { Spacing = 10, Layout = ButtonBoxStyle.Center };
            hbbox.PackStart(_startButton, false, false, 0);
            hbbox.PackStart(_backButton, false, false, 0);
            _mainVerticalBox.PackStart(hbbox, false, false, 0);
            _startButton.GrabFocus();
            _startButton.Clicked += StartButtonOnClicked;    
            _backButton.Clicked += BackButtonOnClicked;
            
            Add(_mainVerticalBox);
            ShowAll();
            
            _flashCardService.HintRequested += FlashCardServiceOnHintRequested;
        }

        private void OnClicked(object sender, EventArgs eventArgs)
        {
            if (sender is FhButton btn)
            {
                Application.Invoke(delegate
                {   
                    var correct = _flashCardService.GuessTango(btn.Label);
                    btn.SetButtonColor(correct ? _correctColor : _wrongColor);
                    btn.Sensitive = false;

                    if (correct)
                    {
                        _guessButtonList.ForEach(gBtn => { gBtn.Sensitive = false; });
                        ShowAnswer();
                    }

                    UpdateDescription();
                });
            }
        }

        private void FlashCardServiceOnHintRequested(object sender, EventArgs eventArgs)
        {
            Application.Invoke(delegate{ ShowAnswer(); });
        }

        private void BackButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Destroy();
        }

        private void NextButtonOnClicked(object sender, EventArgs eventArgs)
        {
            NextCard();
        }

        private void StartButtonOnClicked(object sender, EventArgs eventArgs)
        {
            _startButton.Clicked -= StartButtonOnClicked;
            _startButton.Clicked += NextButtonOnClicked;
            _startButton.Label = "次へ";
            
            _flashCardService.Init();
            NextCard();
        }

        private void NextCard()
        {
            if (_flashCardService.NextFlashCard(out var card))
            {
                _currentCardIndex.Text = $"質問：{card.Number} / {_flashCardService.CardCount}";
                _cardLabelTango.Text = card.Kotoba.Namae;
                _cardLabelFurigana.Text = card.Kotoba.Furigana;
                _cardLabelRomaji.Text = card.Kotoba.Romaji;
                _cardLabelHonyaku.Text = card.Kotoba.Honyaku;

                _cardLabelFurigana.Visible = false;
                _cardLabelRomaji.Visible = false;
                _cardLabelHonyaku.Visible = false;

                if (_configuration.GameMode == GameMode.GuessMode)
                {
                    UpdateDescription();
                    for (var i = 0; i < _flashCardService.GuessKotobaList.Count; i++)
                    {
                        _guessButtonList[i].SetButtonColor(_defaultColor);
                        _guessButtonList[i].Sensitive = true;
                        _guessButtonList[i].Label = _flashCardService.GuessKotobaList[i].Honyaku;
                    }
                }
            }
            else
            {
                _currentCardIndex.Text = string.Empty;
                _cardLabelTango.Text = "終わり";
                _cardLabelFurigana.Text = string.Empty;
                _cardLabelRomaji.Text = string.Empty;
                _cardLabelHonyaku.Text = string.Empty;
                
                _startButton.Clicked -= NextButtonOnClicked;
                _startButton.Clicked += StartButtonOnClicked;
                _startButton.Label = "スタート";

                if (_configuration.GameMode == GameMode.GuessMode)
                {
                    for (var i = 0; i < _guessButtonList.Count; i++)
                    {
                        _guessButtonList[i].Sensitive = false;
                        _guessButtonList[i].Label = $"回答{i + 1}";
                    }
                }
            }
        }

        private void UpdateDescription()
        {
            _scoreDescription.Text = $"正解：{_flashCardService.CorrectNumber} " +
                                     $"間違：{_flashCardService.WrongNumber} " +
                                     $"スキップ：{_flashCardService.SkipNumber}";
        }

        private void ShowAnswer()
        {
            _cardLabelFurigana.Visible = true;
            _cardLabelRomaji.Visible = true;
            _cardLabelHonyaku.Visible = true;
        }
    }
}