using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using UnityEngine;

public class VehicleSelectManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private LocalizedText vehicleNameText;
    [SerializeField] private LocalizedText vehicleStatsText;

    public void DisplayCurrentVehicle(CarConfigData currentVehicle)
    {
        vehicleNameText.SetDynamicKey(currentVehicle.name);

        vehicleStatsText.SetDynamicStats(currentVehicle.MaxSpeed, currentVehicle.MaxTurnSpeed);
    }
}
