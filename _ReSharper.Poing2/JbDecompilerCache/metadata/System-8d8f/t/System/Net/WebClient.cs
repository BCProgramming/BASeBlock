// Type: System.Net.WebClient
// Assembly: System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.dll

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace System.Net
{
    [ComVisible(true)]
    public class WebClient : Component
    {
        public Encoding Encoding { get; set; }
        public string BaseAddress { get; set; }
        public ICredentials Credentials { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public NameValueCollection QueryString { get; set; }
        public WebHeaderCollection ResponseHeaders { get; }
        public IWebProxy Proxy { get; set; }
        public RequestCachePolicy CachePolicy { get; set; }
        public bool IsBusy { get; }
        protected virtual WebRequest GetWebRequest(Uri address);
        protected virtual WebResponse GetWebResponse(WebRequest request);
        protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result);
        public byte[] DownloadData(string address);
        public byte[] DownloadData(Uri address);
        public void DownloadFile(string address, string fileName);
        public void DownloadFile(Uri address, string fileName);
        public Stream OpenRead(string address);
        public Stream OpenRead(Uri address);
        public Stream OpenWrite(string address);
        public Stream OpenWrite(Uri address);
        public Stream OpenWrite(string address, string method);
        public Stream OpenWrite(Uri address, string method);
        public byte[] UploadData(string address, byte[] data);
        public byte[] UploadData(Uri address, byte[] data);
        public byte[] UploadData(string address, string method, byte[] data);
        public byte[] UploadData(Uri address, string method, byte[] data);
        public byte[] UploadFile(string address, string fileName);
        public byte[] UploadFile(Uri address, string fileName);
        public byte[] UploadFile(string address, string method, string fileName);
        public byte[] UploadFile(Uri address, string method, string fileName);
        public byte[] UploadValues(string address, NameValueCollection data);
        public byte[] UploadValues(Uri address, NameValueCollection data);
        public byte[] UploadValues(string address, string method, NameValueCollection data);
        public byte[] UploadValues(Uri address, string method, NameValueCollection data);
        public string UploadString(string address, string data);
        public string UploadString(Uri address, string data);
        public string UploadString(string address, string method, string data);
        public string UploadString(Uri address, string method, string data);
        public string DownloadString(string address);
        public string DownloadString(Uri address);
        protected virtual void OnOpenReadCompleted(OpenReadCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenReadAsync(Uri address);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenReadAsync(Uri address, object userToken);

        protected virtual void OnOpenWriteCompleted(OpenWriteCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenWriteAsync(Uri address);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenWriteAsync(Uri address, string method);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void OpenWriteAsync(Uri address, string method, object userToken);

        protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadStringAsync(Uri address);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadStringAsync(Uri address, object userToken);

        protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadDataAsync(Uri address);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadDataAsync(Uri address, object userToken);

        protected virtual void OnDownloadFileCompleted(AsyncCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadFileAsync(Uri address, string fileName);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void DownloadFileAsync(Uri address, string fileName, object userToken);

        protected virtual void OnUploadStringCompleted(UploadStringCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadStringAsync(Uri address, string data);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadStringAsync(Uri address, string method, string data);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadStringAsync(Uri address, string method, string data, object userToken);

        protected virtual void OnUploadDataCompleted(UploadDataCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadDataAsync(Uri address, byte[] data);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadDataAsync(Uri address, string method, byte[] data);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadDataAsync(Uri address, string method, byte[] data, object userToken);

        protected virtual void OnUploadFileCompleted(UploadFileCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadFileAsync(Uri address, string fileName);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadFileAsync(Uri address, string method, string fileName);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadFileAsync(Uri address, string method, string fileName, object userToken);

        protected virtual void OnUploadValuesCompleted(UploadValuesCompletedEventArgs e);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadValuesAsync(Uri address, NameValueCollection data);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadValuesAsync(Uri address, string method, NameValueCollection data);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
        public void UploadValuesAsync(Uri address, string method, NameValueCollection data, object userToken);

        public void CancelAsync();
        protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e);
        protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e);
        public event OpenReadCompletedEventHandler OpenReadCompleted;
        public event OpenWriteCompletedEventHandler OpenWriteCompleted;
        public event DownloadStringCompletedEventHandler DownloadStringCompleted;
        public event DownloadDataCompletedEventHandler DownloadDataCompleted;
        public event AsyncCompletedEventHandler DownloadFileCompleted;
        public event UploadStringCompletedEventHandler UploadStringCompleted;
        public event UploadDataCompletedEventHandler UploadDataCompleted;
        public event UploadFileCompletedEventHandler UploadFileCompleted;
        public event UploadValuesCompletedEventHandler UploadValuesCompleted;
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event UploadProgressChangedEventHandler UploadProgressChanged;
    }
}
