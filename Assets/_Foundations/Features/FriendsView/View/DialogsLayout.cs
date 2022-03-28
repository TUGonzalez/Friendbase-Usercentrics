using System;
using System.Collections.Generic;
using System.Linq;
using FriendsView.Core.Domain;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class DialogsLayout : MonoBehaviour
    {
        [SerializeField] Button selectBlockBtn;
        [SerializeField] Button selectReportBtn;
        [SerializeField] Button confirmBlockingBtn;
        [SerializeField] Button alsoReportBtn;
        [SerializeField] Button alsoBlockBtn;
        [SerializeField] Button justBlockBtn;
        [SerializeField] Button justReportBtn;
        [SerializeField] Button confirmAddButton;
        [SerializeField] Button otherReasonButton;

        [SerializeField] Transform startBox;
        [SerializeField] Transform blockBox;
        [SerializeField] Transform alsoOk;
        [SerializeField] Transform reasonBox;
        [SerializeField] Transform alsoReport;
        [SerializeField] Transform alsoBlock;
        [SerializeField] Transform done;
        [SerializeField] Transform okGO;
        [SerializeField] Transform unableVisitBox;
        [SerializeField] Transform confirmAddBox;
        [SerializeField] Transform OtherReasonBox;

        [SerializeField] private TMP_InputField otherReasonField;
        
        [SerializeField] UnfriendDialog unfriendDialog;

        [SerializeField] List<TextMeshProUGUI> usernameText;
        [SerializeField] List<Button> reportReasonBtns;

        public IObservable<Unit> OnBlock => selectBlockBtn.OnClickAsObservable();
        public IObservable<Unit> OnReport => selectReportBtn.OnClickAsObservable();
        public IObservable<Unit> OnConfirmBlocking => confirmBlockingBtn.OnClickAsObservable();
        public IObservable<Unit> OnAlsoReport => alsoReportBtn.OnClickAsObservable();
        public IObservable<Unit> OnAlsoBlock => alsoBlockBtn.OnClickAsObservable();
        public IObservable<Unit> OnJustBlock => justBlockBtn.OnClickAsObservable();
        public IObservable<Unit> OnJustReport => justReportBtn.OnClickAsObservable();
        public IObservable<Unit> OnConfirmAdd => confirmAddButton.OnClickAsObservable();
        public IObservable<Unit> OtherReasonButton => otherReasonButton.OnClickAsObservable();
        
        
        public UnfriendDialog UnfriendDialog => unfriendDialog;

        public IEnumerable<IObservable<Unit>> ReportReasonBtns => reportReasonBtns
            .Select(b => b.OnClickAsObservable());

        public TMP_InputField OtherReasonField => otherReasonField;

        readonly List<string> startDialogsTexts = new List<string>();

        public void HideSections()
        {
            UnfriendDialog.gameObject.SetActive(false);
            okGO.gameObject.SetActive(false);
            unableVisitBox.gameObject.SetActive(false);
            startBox.gameObject.SetActive(false);
            blockBox.gameObject.SetActive(false);
            alsoReport.gameObject.SetActive(false);
            alsoOk.gameObject.SetActive(false);
            reasonBox.gameObject.SetActive(false);
            done.gameObject.SetActive(false);
            alsoBlock.gameObject.SetActive(false);
            confirmAddBox.gameObject.SetActive(false);
            OtherReasonBox.gameObject.SetActive(false);
            
        }

        public void ShowSection(ViewSection section)
        {
            switch (section)
            {
                case ViewSection.UnfriendModal:
                    UnfriendDialog.gameObject.SetActive(true);
                    break;
                case ViewSection.OkBox:
                    okGO.gameObject.SetActive(true);
                    break;
                case ViewSection.StartReportBox:
                    startBox.gameObject.SetActive(true);
                    break;
                case ViewSection.BlockBox:
                    blockBox.gameObject.SetActive(true);
                    break;
                case ViewSection.AlsoReportBox:
                    alsoReport.gameObject.SetActive(true);
                    break;
                case ViewSection.AlsoBlockBox:
                    alsoBlock.gameObject.SetActive(true);
                    break;
                case ViewSection.AlsoOkBox:
                    alsoOk.gameObject.SetActive(true);
                    break;
                case ViewSection.ReasonBox:
                    reasonBox.gameObject.SetActive(true);
                    break;
                case ViewSection.ReasonBoxFromStart:
                    reasonBox.gameObject.SetActive(true);
                    break;
                case ViewSection.DoneBox:
                    done.gameObject.SetActive(true);
                    break;
                case ViewSection.UnableVisitOkCard:
                    unableVisitBox.gameObject.SetActive(true);
                    break;
                case ViewSection.UnableVisitOkList:
                    unableVisitBox.gameObject.SetActive(true);
                    break;
                case ViewSection.ConfirmAddFriend:
                    confirmAddBox.gameObject.SetActive(true);
                    break; 
                case ViewSection.OtherBox:
                    OtherReasonBox.gameObject.SetActive(true);
                    break;
                    
            }
        }

        private void Start()
        {
            foreach (var tm in usernameText)
            {
                startDialogsTexts.Add(tm.text);
            }
        }

        public void SetUsernameFields(string username)
        {
            for (int i = 0; i < usernameText.Count; i++)
            {
                if (startDialogsTexts[i].Contains("?"))
                {
                    var index = startDialogsTexts[i].IndexOf('?');
                    string tx = startDialogsTexts[i].Insert(index, " " + username);
                    usernameText[i].SetText(tx);
                }
                else
                {
                    usernameText[i].SetText(startDialogsTexts[i] + " " + username);
                }
            }
        }

        public Dictionary<int, ReportReasons> ReportReason { get; }
            = new Dictionary<int, ReportReasons>()
            {
                {0, ReportReasons.inappropriate_content},
                {1, ReportReasons.spam},
                {2, ReportReasons.fake_account},
                {3, ReportReasons.abusive_behavior},
                {4, ReportReasons.foul_language},
                {5, ReportReasons.other}
            };

    }
}