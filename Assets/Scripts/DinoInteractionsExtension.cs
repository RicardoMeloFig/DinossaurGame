using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public static class DinoInteractionsExtension
    {
        public static T Eat<T>(this T dinossaur) where T : BaseDinossaur
        {
            dinossaur.Hunger = dinossaur.Hunger + 20;
            return dinossaur;
        }


        public static T Drink<T>(this T dinossaur) where T : BaseDinossaur
        {
            dinossaur.Thirst = dinossaur.Thirst + 20;
            return dinossaur;
            //return new T() { Thirst = dinossaur.Thirst + 20 };
        }

        public static T HealthStatusUpdate<T>(this T dinossaur) where T : BaseDinossaur
        {
            dinossaur.Thirst = dinossaur.Thirst - dinossaur.ThirstLoss * Time.deltaTime;
            dinossaur.Hunger = dinossaur.Hunger - dinossaur.ThirstLoss * Time.deltaTime;

            return dinossaur;
            
        }

        public static bool GetCanJump<T>(this T dinossaur) where T : BaseDinossaur
        {
            return dinossaur.CanJump;
        }

    }
}
