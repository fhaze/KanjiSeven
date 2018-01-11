using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gdk;
using Gtk;
using KanjiSeven.Models;
using KanjiSeven.Services;
using KanjiSeven.Widgets;
using Pango;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class FlashCardForm : BaseWindow
    {
        private readonly VBox   _mainVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly VBox   _helpVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly Label  _description       = new Label("ー");
        private readonly Label  _cardLabelKotoba   = new Label("｡ﾟﾟ(」｡≧□≦)」");
        private readonly Label  _cardLabelFurigana = new Label("");
        private readonly Label  _cardLabelRomaji   = new Label("");
        private readonly Label  _cardLabelHonyaku  = new Label("");
        private readonly Button _startButton       = new Button { Label = "始めよ" };
        private readonly Button _backButton        = new Button { Label = "やめろ" }; 
        
        // 推測ゲーム
        private readonly HButtonBox   _guessTopBox = new HButtonBox { Layout = ButtonBoxStyle.Center, Spacing = 5};
        private readonly HButtonBox   _guessBottomBox = new HButtonBox { Layout = ButtonBoxStyle.Center, Spacing = 5 };
        private readonly List<Button> _guessButtonList = new List<Button>
        {
            new Button { Label = "回答１" },
            new Button { Label = "回答２" },
            new Button { Label = "回答３" },
            new Button { Label = "回答４" },
            new Button { Label = "回答５" },
            new Button { Label = "回答６" },
        };

        private readonly Configuration    _configuration = ConfigManager.Current;
        private readonly FlashCardService _flashCardService = FlashCardService.Current;
        
        public FlashCardForm(Window parent) : base("ゲーム")
        {
            SetSizeRequest(900, 550);
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);

            var fontDescription = _cardLabelKotoba.PangoContext.FontDescription;
            fontDescription.Size = Convert.ToInt32(80 * Pango.Scale.PangoScale);
            _cardLabelKotoba.ModifyFont(fontDescription);
            _mainVerticalBox.PackStart(_description, false, false, 0);
            _mainVerticalBox.PackStart(_cardLabelKotoba, true, true, 0);

            if (_configuration.GameStyle == GameStyle.GuessMode)
            {
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
                _description.Text = $"{card.Number}/{_flashCardService.Count}";
                _cardLabelKotoba.Text = card.Kotoba.Namae;
                _cardLabelFurigana.Text = card.Kotoba.Furigana;
                _cardLabelRomaji.Text = card.Kotoba.Romaji;
                _cardLabelHonyaku.Text = card.Kotoba.Honyaku;

                _cardLabelFurigana.Visible = false;
                _cardLabelRomaji.Visible = false;
                _cardLabelHonyaku.Visible = false;

                if (_configuration.GameStyle == GameStyle.GuessMode)
                {
                    for (var i = 0; i < _flashCardService.GuessKotobaList.Count; i++)
                        _guessButtonList[i].Label = _flashCardService.GuessKotobaList[i].Honyaku;
                }
            }

            if (_flashCardService.GameState == GameState.Result)
            {
                _startButton.Clicked -= NextButtonOnClicked;
                _startButton.Clicked += StartButtonOnClicked;
                _startButton.Label = "始めよ";
            }
        }
    }
}