using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWSWrapperS3.Controller;
using Bolareshet.UploadPresignedURClient.Interface;
using CompressModuel.Contracts.View;
using Prism.Commands;
using Prism.Mvvm;

namespace Bolareshet.UploadPresignedURClient.ViewModel
{


    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : BindableBase
    {
        
        private readonly ICompressUserControl m_compressUserControl;
        private string m_strFileToUpload;
        private string m_s3PresignedURL;
        private int m_valueProgress;
        private bool m_showTotalProcess;

        public DelegateCommand UploadButtonCommand { get; set; }

        [ImportingConstructor]
        public ShellViewModel([Import(AllowDefault = true)] ICompressUserControl compressUserControl)
        {
            if (compressUserControl != null)
            {
                m_compressUserControl = compressUserControl;
                RaisePropertyChanged("CompressUserControl");
            }
            else
            {                
            }

            UploadButtonCommand = new DelegateCommand(UploadButtonExecute, UploadButtonCanExecute);

            try
            {
                S3PresignedURL = System.Configuration.ConfigurationManager.AppSettings["S3PresignedURL"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ShowTotalProcess = true;
            m_strFileToUpload = string.Empty;

            m_compressUserControl.FileReadyNotify += new EventHandler<string>(OnFileReadyNotify);
            m_compressUserControl.FileCompressProgressNotify += new EventHandler<int>(OnFileCompressProgressNotify);


        }

        private void OnFileReadyNotify(object sender, string e)
        {
            m_strFileToUpload = e;
            UploadButtonCommand.RaiseCanExecuteChanged();
        }

        private void OnFileCompressProgressNotify(object sender, int e)
        {
            
            Task fileCompressProgressTask = Task.Factory.StartNew(() =>
            {
                ValueProgress = e;

                OnPropertyChanged("ValueProgress");
            });

            fileCompressProgressTask.Wait();
        }

        public string S3PresignedURL
        {
            get { return m_s3PresignedURL; }
            set
            {
                SetProperty(ref m_s3PresignedURL, value);
                UploadButtonCommand.RaiseCanExecuteChanged();
            }
        }

        public int ValueProgress
        {
            get { return m_valueProgress; }
            set
            {
                SetProperty(ref m_valueProgress, value);
            }
        }
        
        public bool ShowTotalProcess
        {
            get { return m_showTotalProcess; }
            set
            {
                SetProperty(ref m_showTotalProcess, value);
            }
        }

        private bool UploadButtonCanExecute()
        {
            return ((!String.IsNullOrEmpty(m_strFileToUpload)) && (!String.IsNullOrEmpty(S3PresignedURL)));
        }

        private void UploadButtonExecute()
        {
            S3ClientWrapper.UploadObjectUsingPresignedURL(S3PresignedURL, m_strFileToUpload);
        }

        public object CompressUserControl
        {
            get
            {
                return m_compressUserControl;
            }
        }
    }
}
