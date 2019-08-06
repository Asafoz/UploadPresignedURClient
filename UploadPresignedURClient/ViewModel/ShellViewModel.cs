using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWSWrapperS3.Contracts;
using AWSWrapperS3.Controller;
using Bolareshet.UploadPresignedURClient.Interface;
using CompressModuel.Contracts.View;
using Prism.Commands;
using Prism.Mvvm;

namespace Bolareshet.UploadPresignedURClient.ViewModel
{

    enum ProgressOrigineType
    {
        Compress,
        UploadFile
    }

    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : BindableBase
    {
        
        private readonly ICompressUserControl m_compressUserControl;
        private string m_strFileToUpload;
        private string m_s3PresignedURL;

        private int m_valueProgress;
        private int m_minimumProgress;
        private int m_maximumProgress;
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

            ShowTotalProcess = false;
            m_strFileToUpload = string.Empty;

            MinimumProgress = 0;
            MaximumProgress = 100;

            m_compressUserControl.FileReadyNotify += new EventHandler<string>(OnFileReadyNotify);
            m_compressUserControl.FileCompressProgressNotify += new EventHandler<Tuple<long, long>>(OnFileCompressProgressNotify);


        }

        private void OnFileReadyNotify(object sender, string e)
        {
            m_strFileToUpload = e;
            UploadButtonCommand.RaiseCanExecuteChanged();
        }

        private void OnProgressChanged(object sender, FileTransferProgress e)
        {
            Task fileCompressProgressTask = Task.Factory.StartNew(() =>
            {
                ShowTotalProcess = true;
                ValueProgress = CalcUploadPresignedURClientProgress((int)e.Offset, e.FileSize, ProgressOrigineType.UploadFile);
            });

            fileCompressProgressTask.Wait();
        }
        private void OnFileCompressProgressNotify(object sender, Tuple<long, long> e)
        {
            
            Task fileCompressProgressTask = Task.Factory.StartNew(() =>
            {
                ShowTotalProcess = false;
                ValueProgress = CalcUploadPresignedURClientProgress((int)e.Item1 , e.Item2 , ProgressOrigineType.Compress);
            });

            fileCompressProgressTask.Wait();
        }

        private int CalcUploadPresignedURClientProgress(int currentProgress, long maxinmumProgressSize, ProgressOrigineType origineType)
        {
            int convertedProgress = 0;

            double pro = (double)((double)currentProgress / (double)maxinmumProgressSize);
            convertedProgress = (int)(pro * 100);

            switch (origineType)
            {
                case ProgressOrigineType.Compress:
                    convertedProgress =(int) (convertedProgress  * 0.3);
                    break;
                case ProgressOrigineType.UploadFile:
                    convertedProgress = 30 + (int)(convertedProgress * 0.7);
                    break;
                default:
                    convertedProgress = 0;
                    break;
            }

            return convertedProgress;
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

        public int MinimumProgress
        {
            get { return m_minimumProgress; }
            set
            {
                SetProperty(ref m_minimumProgress, value);
            }
        }

        public int MaximumProgress
        {
            get { return m_maximumProgress; }
            set
            {
                SetProperty(ref m_maximumProgress, value);
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
            try
            {
                Task compressData = Task.Factory.StartNew(() =>
                {
                    ShowTotalProcess = true;

                    Progress<FileTransferProgress> fileTransferProgress = new Progress<FileTransferProgress>();
                    fileTransferProgress.ProgressChanged += new EventHandler<FileTransferProgress>(OnProgressChanged);
                    S3ClientWrapper.UploadObjectUsingPresignedURL(S3PresignedURL, m_strFileToUpload, fileTransferProgress);


                    ShowTotalProcess = false;
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
