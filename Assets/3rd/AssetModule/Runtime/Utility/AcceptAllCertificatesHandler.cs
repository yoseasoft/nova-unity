using UnityEngine.Networking;

namespace AssetModule
{
    /// <summary>
    /// 使UnityWebRequest直接通过所有证书验证(即不进行任何验证)
    /// Android使用UnityWebRequest需使用自定义验证, 此处直接通过, 可根据项目具体需要修改验证方法
    /// https://forum.unity.com/threads/unitywebrequest-report-an-error-ssl-ca-certificate-error.617521/
    /// </summary>
    public class AcceptAllCertificatesHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}