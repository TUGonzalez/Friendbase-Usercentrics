using System;
using Architecture.MVP;
using BurguerMenu.Core.Domain;
using UniRx;

namespace BurguerMenu.Presentation
{
    public interface IBurguerView : IPresentable
    {
        IObservable<Unit> OnEnabled { get; }
        IObservable<Unit> OnHelpButton { get; }
        IObservable<Unit> OnLogOutButton { get; }
        IObservable<Unit> OnCloseButton { get; }
        IObservable<Unit> OnSupportButton { get; }
        IObservable<Unit> OnTermsButton { get; }
        IObservable<Unit> OnCodeOfConductButton { get; }
        IObservable<Unit> OnPrivacyPolicyButton { get; }
        void ShowSection(BurgerSection section);
        void HideSections();
        void VisitExternalSectionUrl(HelpButton buttonKey);
    }
}