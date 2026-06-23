using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
public class CarCarousel_Ui : MonoBehaviour
{
    [Header("Carousel Renderers")]
    [SerializeField] private Image _mainCarImage;
    [SerializeField] private Image _leftGhostImage;
    [SerializeField] private Image _rightGhostImage;

    [Header("Carousel Navigation Buttons")]
    [SerializeField] private Button _leftArrowButton;
    [SerializeField] private Button _rightArrowButton;

    [Header("Carousel Effect Setting")]
    [SerializeField] private float _tweenDuration = 0.5f;
    [SerializeField] private Ease _animationEase = Ease.OutQuad;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 _centerScale = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private Vector3 _sideScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("Color Settings")]
    [SerializeField] private readonly Color _activeColor = Color.white;
    [SerializeField] private readonly Color _dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private Racing_Main_Menu_Model _model;

    public void Setup(Racing_Main_Menu_Model model)
    {
        _model = model;
        if (model == null) return;

        _leftArrowButton?.onClick.AddListener(() => _model.PreviousCar());
        _rightArrowButton?.onClick?.AddListener(() => _model.NextCar());

        UpdateCarouselVisualsInstant();
        _model.OnCarCarouselChanged += AnimateCarChange;
    }

    public void CleanUp()
    {
        _leftArrowButton?.onClick.RemoveAllListeners();
        _rightArrowButton?.onClick.RemoveAllListeners();

        if (_model != null) _model.OnCarCarouselChanged -= AnimateCarChange;
        KillAllTweens();
    }

    public void UpdateCarouselVisualsInstant()
    {
        if (_model == null) return;

        List<CarConfigData> carData = _model.AvailableCars;
        int totalCars = carData.Count;
        if (totalCars == 0) return;

        int currentId = carData.FindIndex(c => c.CarId == _model.SelectedCarId);
        if(currentId == -1) currentId = 0;

        int leftIndex = (currentId - 1 + totalCars) % totalCars;
        int rightIndex = (currentId + 1) % totalCars;

        if (_mainCarImage != null) _mainCarImage.sprite = carData[currentId].CarUiSprite;
        if (_leftGhostImage != null) _leftGhostImage.sprite = carData[leftIndex].CarUiSprite;
        if (_rightGhostImage != null) _rightGhostImage.sprite = carData[rightIndex].CarUiSprite;

        if (_mainCarImage != null) _mainCarImage.rectTransform.localScale = _centerScale;
        if (_leftGhostImage != null) _leftGhostImage.rectTransform.localScale = _sideScale;
        if (_rightGhostImage != null) _rightGhostImage.rectTransform.localScale = _sideScale;

        if (_mainCarImage != null) _mainCarImage.color = _activeColor;
        if (_leftGhostImage != null) _leftGhostImage.color = _dimmedColor;
        if (_rightGhostImage != null) _rightGhostImage.color = _dimmedColor;
    }

    private void AnimateCarChange(CarConfigData newCar, Racing_Main_Menu_Model.SelectionDirection direction)
    {
        SetCarouselButtonsInteractable(false);

        KillAllTweens();

        _mainCarImage.rectTransform.DOScale(_sideScale, _tweenDuration).SetEase(_animationEase);
        _mainCarImage.DOColor(_dimmedColor, _tweenDuration).SetEase(_animationEase);

        Image targetGhost = (direction == Racing_Main_Menu_Model.SelectionDirection.Previous)
            ? _leftGhostImage
            : _rightGhostImage;

        targetGhost.rectTransform.DOScale(_centerScale, _tweenDuration).SetEase(_animationEase);
        targetGhost.DOColor(_activeColor, _tweenDuration).SetEase(_animationEase)
            .OnComplete(() => {
                UpdateCarouselVisualsInstant();
                SetCarouselButtonsInteractable(true);
            });

    }

    private void SetCarouselButtonsInteractable(bool state)
    {
        if (_leftArrowButton != null) _leftArrowButton.interactable = state;
        if (_rightArrowButton != null) _rightArrowButton.interactable = state;
    }

    private void KillAllTweens()
    {
        if (_mainCarImage != null) { _mainCarImage.rectTransform.DOKill(); _mainCarImage.DOKill(); }
        if (_leftGhostImage != null) { _leftGhostImage.rectTransform.DOKill(); _leftGhostImage.DOKill(); }
        if (_rightGhostImage != null) { _rightGhostImage.rectTransform.DOKill(); _rightGhostImage.DOKill(); }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}

