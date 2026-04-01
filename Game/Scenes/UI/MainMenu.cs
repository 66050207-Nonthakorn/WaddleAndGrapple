using Microsoft.Xna.Framework;
using ComputerGameFinal.Engine;
using ComputerGameFinal.Engine.Managers;
using System;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using ComputerGameFinal.Engine.Components;

namespace ComputerGameFinal.Game.Scenes;

public class MainMenu : Scene
{
    private Panel _buttonPanel;
    private Panel _optionsPanel;
    private Button _optionButton;
    private Button _optionsBackButton;

    public override void Setup()
    {
        GumService.Default.Root.Children.Clear(); // Clear any existing Gum UI elements

        float screenWidth = ScreenManager.Instance.nativeWidth;
        float screenHeight = ScreenManager.Instance.nativeHeight;

        // Background
        var background = base.AddGameObject<GameObject>("bg");
        var bgSprite = background.AddComponent<SpriteRenderer>();
        bgSprite.Texture = ResourceManager.Instance.GetTexture("UI/MainScreen");
        background.Position = new Vector2(screenWidth / 2f, screenHeight / 2f);
        if (bgSprite.Texture != null)
        {
            background.Scale = new Vector2(
                screenWidth / bgSprite.Texture.Width,
                screenHeight / bgSprite.Texture.Height
            );
        }

        CreateButtonsPanel();
        createOptionsPanel();
    }

    private void CreateButtonsPanel()
    {
        // check the size for Gum
        Console.WriteLine($"Creating button panel with screen size: {GumService.Default.CanvasWidth}x{GumService.Default.CanvasHeight}");
        _buttonPanel = new Panel();
        _buttonPanel.Dock(Gum.Wireframe.Dock.Fill);
        _buttonPanel.AddToRoot();

        var startButton = new Button();
        startButton.Anchor(Gum.Wireframe.Anchor.Center);
        startButton.Y = 80;
        startButton.Width = 240;
        startButton.Height = 30;
        startButton.Text = "START GAME";
        startButton.Click += OnStartGameClick;
        _buttonPanel.AddChild(startButton);

        _optionButton = new Button();
        _optionButton.Anchor(Gum.Wireframe.Anchor.Center);
        _optionButton.Y = 150;
        _optionButton.Width = 240;
        _optionButton.Height = 30;
        _optionButton.Text = "SETTINGS";
        _optionButton.Click += OnSettingsClick;
        _buttonPanel.AddChild(_optionButton);

        var quitButton = new Button();
        quitButton.Anchor(Gum.Wireframe.Anchor.Center);
        quitButton.Y = 220;
        quitButton.Width = 240;
        quitButton.Height = 30;
        quitButton.Text = "QUIT GAME";
        quitButton.Click += OnQuitGameClick;
        _buttonPanel.AddChild(quitButton);
    }

    private void createOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsPanel.IsVisible = false; // Start hidden
        _optionsPanel.AddToRoot();

        var optionsText = new TextRuntime();
        optionsText.X = 10;
        optionsText.Y = 10;
        optionsText.Text = "OPTIONS";
        _optionsPanel.AddChild(optionsText);

        var musicLabel = new Label();
        musicLabel.Text = "Music";
        musicLabel.X = 35;
        musicLabel.Y = 35;
        _optionsPanel.AddChild(musicLabel);

        var musicSlider = new Slider();
        musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
        musicSlider.Y = 30f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = AudioManager.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _optionsPanel.AddChild(musicSlider);

        var sfxLabel = new Label();
        sfxLabel.Text = "SFX";
        sfxLabel.X = 20;
        sfxLabel.Y = 80;
        _optionsPanel.AddChild(sfxLabel);

        var sfxSlider = new Slider();
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = AudioManager.SFXVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new Button();
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);
    }

    private void OnStartGameClick(object sender, EventArgs e)
    {
        // Create a fresh GameScene instance each time
        GumService.Default.Root.Children.Clear(); // Clear Gum UI elements from the main menu
        SceneManager.Instance.LoadScene("GameScene");
    }

    private void OnSettingsClick(object sender, EventArgs e)
    {
        // // Open settings as an overlay
        _buttonPanel.IsVisible = false;
        _optionsPanel.IsVisible = true;
    }

    private void OnQuitGameClick(object sender, EventArgs e)
    {
        // // Exit the game
        // AudioManager.Instance.PlaySound("Button_Click");
        System.Environment.Exit(0);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs e)
    {
        if (sender is Slider slider)
        {
            AudioManager.SongVolume = (float)slider.Value;
        }
    }

    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs e)
    {
        // play a sound to indicate the change
    }

    private void HandleSfxSliderChanged(object sender, EventArgs e)
    {
        if (sender is Slider slider)
        {
            AudioManager.SFXVolume = (float)slider.Value;
        }
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
    {
        // play a sound to indicate the change
    }

    private void HandleOptionsButtonBack(object sender, EventArgs e)
    {
        _optionsPanel.IsVisible = false;
        _buttonPanel.IsVisible = true;
    }
}
