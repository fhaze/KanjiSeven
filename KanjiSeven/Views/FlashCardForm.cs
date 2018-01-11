using System;
using System.Collections.Generic;
using Gtk;
using KanjiSeven.Extensions;
using KanjiSeven.Models;
using KanjiSeven.Services;
using KanjiSeven.Widgets;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class FlashCardForm : BaseWindow
    {
        private readonly VBox   _mainVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly VBox   _helpVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly HBox   _descriptionHBox   = new HBox { BorderWidth = 10 };
        private readonly Label  _currentCardIndex  = new Label();
        private readonly Label  _scoreDescription  = new Label();
        private readonly Label  _cardLabelKotoba   = new Label("｡ﾟﾟ(」｡≧□≦)」");
        private readonly Label  _cardLabelFurigana = new Label(string.Empty);
        private readonly Label  _cardLabelRomaji   = new Label(string.Empty);
        private readonly Label  _cardLabelHonyaku  = new Label(string.Empty);
        private readonly Button _startButton       = new Button { Label = "スタート" };
        private readonly Button _backButton        = new Button { Label = "やめろ" }; 
        
        // 推測ゲーム
        private readonly Gdk.Color    _defaultColor = new Gdk.Color(255, 255, 255);
        private readonly Gdk.Color    _correctColor = new Gdk.Color(150, 255, 150);
        private readonly Gdk.Color    _wrongColor = new Gdk.Color(255, 150, 150);
        private readonly HButtonBox   _guessTopBox = new HButtonBox { Layout = ButtonBoxStyle.Center, Spacing = 5};
        private readonly HButtonBox   _guessBottomBox = new HButtonBox { Layout = ButtonBoxStyle.Center, Spacing = 5 };
        private readonly List<Button> _guessButtonList = new List<Button>
        {
            new Button(),
            new Button(),
            new Button(),
            new Button(),
            new Button(),
            new Button(),
        };

        private readonly Configuration    _configuration = ConfigManager.Current;
        private readonly FlashCardService _flashCardService = FlashCardService.Current;
        
        public FlashCardForm(Window parent) : base("ゲーム")
        {
            SetSizeRequest(900, 550);
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);

            _descriptionHBox.PackStart(_currentCardIndex, false, false, 0);
            
            _cardLabelKotoba.SetFontSize(80);
            _mainVerticalBox.PackStart(_descriptionHBox, false, true, 0);            
            _mainVerticalBox.PackStart(_cardLabelKotoba, true, true, 0);

            if (_configuration.GameMode == GameMode.GuessMode)
            {
                _descriptionHBox.PackStart(_scoreDescription);
                
                for (var i = 0; i < _guessButtonList.Count; i++)
                {
                    _guessButtonList[i].SetBackgroundColor(_defaultColor);
                    _guessButtonList[i].Label = $"回答{i}";
                    _guessButtonList[i].HeightRequest = 40;
                    _guessButtonList[i].WidthRequest = 200;
                    _guessButtonList[i].Sensitive = false;
                    _guessButtonList[i].Clicked += (sender, args) =>
                    {
                        var btn = sender as Button;
                        if (_flashCardService.GuessKotoba(btn.Label))
                            btn.SetBackgroundColor(_correctColor);
                        else
                            btn.SetBackgroundColor(_wrongColor);
                        
                        Application.Invoke(delegate { UpdateDescription(); });
                    };
                }

                _guessTopBox.PackStart(_guessButtonList[0], false, false, 10);
                _guessTopBox.PackStart(_guessButtonList[1], false, false, 10);
                _guessTopBox.PackStart(_guessButtonList[2], false, false, 10);
                
                _guessBottomBox.PackStart(_guessButtonList[3], false, false, 10);
                _guessBottomBox.PackStart(_guessButtonList[4], false, false, 10);
                _guessBottomBox.PackStart(_guessButtonList[5], false, false, 10);
                
                _mainVerticalBox.PackStart(_guessTopBox, false, false, 0);
                _mainVerticalBox.PackStart(_guessBottomBox, false, false, 0);
            }
            
            _helpVerticalBox.PackStart(_cardLabelFurigana, false, false, 0);
            _helpVerticalBox.PackStart(_cardLabelRomaji, false, false, 0);
            _helpVerticalBox.PackStart(_cardLabelHonyaku, false, false, 0);
            _helpVerticalBox.HeightRequest = 50;
            _mainVerticalBox.PackStart(_helpVerticalBox, false, true, 0);
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

        private void FlashCardServiceOnHintRequested(object sender, EventArgs eventArgs)
        {
            Application.Invoke(delegate
            {
                _cardLabelFurigana.Visible = true;
                _cardLabelRomaji.Visible = true;
                _cardLabelHonyaku.Visible = true;
            });
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
                _cardLabelKotoba.Text = card.Kotoba.Namae;
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
                        _guessButtonList[i].SetBackgroundColor(_defaultColor);
                        _guessButtonList[i].Sensitive = true;
                        _guessButtonList[i].Label = _flashCardService.GuessKotobaList[i].Honyaku;
                    }
                }
            }
            else
            {
                _currentCardIndex.Text = string.Empty;
                _cardLabelKotoba.Text = "終わり";
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
    }
}