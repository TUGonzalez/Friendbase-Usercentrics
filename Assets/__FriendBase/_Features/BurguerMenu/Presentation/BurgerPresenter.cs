using AuthFlow;
using BurguerMenu.Core.Domain;
using JetBrains.Annotations;
using UniRx;

namespace BurguerMenu.Presentation
{
    [UsedImplicitly]
    public class BurgerPresenter
    {
        readonly IBurguerView view;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable sectionDisposables = new CompositeDisposable();

        public BurgerPresenter(IBurguerView view)
        {
            this.view = view;
            
            sectionDisposables.AddTo(disposables);

            this.view.OnDisposeView.Subscribe(Dispose).AddTo(disposables);

            view.OnEnabled.Subscribe(Present).AddTo(disposables);
        }

        void Present()
        {
            view.OnHelpButton.Subscribe(PresentHelp).AddTo(sectionDisposables);
            view.OnLogOutButton
                .Do(Hide)
                .Subscribe(LogOut).AddTo(sectionDisposables);
            view.OnCloseButton.Subscribe(Hide).AddTo(sectionDisposables);
            view.ShowSection(BurgerSection.Menu);
        }

        private void PresentHelp()
        {
            view.OnSupportButton.Subscribe(PresentSupport).AddTo(sectionDisposables);
            view.OnTermsButton.Subscribe(PresentTerms).AddTo(sectionDisposables);
            view.OnCodeOfConductButton.Subscribe(PresentConduct).AddTo(sectionDisposables);
            view.OnPrivacyPolicyButton.Subscribe(PresentPrivacy).AddTo(sectionDisposables);

            view.ShowSection(BurgerSection.Help);
        }

        private void PresentSupport()
        {
            view.VisitExternalSectionUrl(HelpButton.Support);
        }

        void PresentTerms()
        {
            view.VisitExternalSectionUrl(HelpButton.TermsAndConditions);
        }

        void PresentConduct()
        {
            view.VisitExternalSectionUrl(HelpButton.CodeOfConduct);
        }

        void PresentPrivacy()
        {
            view.VisitExternalSectionUrl(HelpButton.PrivacyPolicy);
        }

        void LogOut()
        {
            JesseUtils.Logout();
        }

        void Hide()
        {
            view.HideSections();
            sectionDisposables.Clear();
        }

        void Dispose()
        {
            disposables.Clear();
        }
    }
}