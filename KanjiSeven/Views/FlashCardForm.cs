﻿using System;
using Gtk;
using KanjiSeven.Models;
using KanjiSeven.Services;
using KanjiSeven.Widgets;
using Pango;

namespace KanjiSeven.Views
{
    public class FlashCardForm : BaseWindow
    {
        private readonly VBox   _mainVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly Label  _description       = new Label("ー");
        private readonly Label  _cardLabelKotoba   = new Label("字");
        private readonly Label  _cardLabelFurigana = new Label("ふりがな");
        private readonly Label  _cardLabelRomaji   = new Label("ローマ字");
        private readonly Label  _cardLabelHonyaku  = new Label("翻訳");
        private readonly Button _startButton       = new Button { Label = "始めよ！" };
        private readonly Button _nextButton        = new Button { Label = "次へ", Sensitive = false };
        private readonly Button _backButton        = new Button { Label = "やめろ" };  
        
        private readonly FlashCardService _flashCardService = FlashCardService.Current;
        
        public FlashCardForm(Window parent) : base("ゲーム")
        {
            SetSizeRequest(550, 550);
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);

            var fontDescription = _cardLabelKotoba.PangoContext.FontDescription;
            fontDescription.Size = Convert.ToInt32(80 * Pango.Scale.PangoScale);
            _cardLabelKotoba.ModifyFont(fontDescription);
            _mainVerticalBox.PackStart(_description, false, false, 0);
            _mainVerticalBox.PackStart(_cardLabelKotoba, true, true, 0);
            _mainVerticalBox.PackStart(_cardLabelFurigana, false, false, 0);
            _mainVerticalBox.PackStart(_cardLabelRomaji, false, false, 0);
            _mainVerticalBox.PackStart(_cardLabelHonyaku, false, false, 0);
            _mainVerticalBox.PackStart(new HSeparator(), false, true, 5);
            
            var hbbox = new HButtonBox { Spacing = 10, Layout = ButtonBoxStyle.Center };
            hbbox.PackStart(_startButton, false, false, 0);
            hbbox.PackStart(_nextButton, false, false, 0);
            hbbox.PackStart(_backButton, false, false, 0);
            _mainVerticalBox.PackStart(hbbox, false, false, 0);
            _startButton.GrabFocus();
            _startButton.Clicked += StartButtonOnClicked;    
            _nextButton.Clicked += NextButtonOnClicked;
            _backButton.Clicked += BackButtonOnClicked;
            
            Add(_mainVerticalBox);
            ShowAll();
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
            _nextButton.Sensitive = true;
            _nextButton.GrabFocus();
            _startButton.Sensitive = false;
            
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
            }

            if (_flashCardService.GameState == GameState.Result)
            {
                _nextButton.Sensitive = false;
                _startButton.Sensitive = true;
                _startButton.GrabFocus();
            }
        }
    }
}