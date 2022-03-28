using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public interface IOnboarding
    {
        public void NextStep();
        public void WaitAndNextStep(float time);
    }
}